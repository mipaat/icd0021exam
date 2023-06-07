using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Base.WebHelpers;
using BLL.DTO;
using BLL.DTO.Exceptions;
using BLL.Identity.Config;
using DAL;
using Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace BLL.Identity.Services;

public class TokenService
{
    private readonly IdentityUow _identityUow;
    private readonly JwtSettings _jwtSettings;

    public TokenService(IdentityUow identityUow, IConfiguration configuration)
    {
        _identityUow = identityUow;
        _jwtSettings = configuration.GetRequiredSection(JwtSettings.SectionKey).Get<JwtSettings>() ??
                       throw new ConfigurationErrorsException($"{nameof(JwtSettings)} not found in config!");
    }

    private AppDbContext DbContext => _identityUow.DbContext;

    public async Task DeleteRefreshTokenAsync(string jwt, string refreshToken)
    {
        ClaimsPrincipal principal;
        try
        {
            principal = IdentityHelpers.GetClaimsPrincipal(jwt, _jwtSettings.Key, _jwtSettings.Issuer, _jwtSettings.Audience);
        }
        catch (Exception)
        {
            throw new InvalidJwtException();
        }

        var userId = principal.GetUserIdIfExists() ?? throw new InvalidJwtException();

        await DbContext.RefreshTokens
            .Where(r => r.UserId == userId &&
                r.Token == refreshToken || r.PreviousToken == refreshToken)
            .ExecuteDeleteAsync();
    }

    public async Task DeleteExpiredRefreshTokensAsync(Guid userId)
    {
        await DbContext.RefreshTokens.Where(r => r.UserId == userId && r.IsFullyExpired).ExecuteDeleteAsync();
    }

    private int ValidateExpiresInSeconds(int? expiresInSeconds)
    {
        if (expiresInSeconds == null)
        {
            if (_jwtSettings.ExpiresInSeconds <= 0)
            {
                throw new ConfigurationErrorsException(
                    $"Configured JWT expiration time must be greater than 0 seconds, but was {_jwtSettings.ExpiresInSeconds}");
            }
            
            expiresInSeconds = _jwtSettings.ExpiresInSeconds;
        } else if (expiresInSeconds <= 0)
        {
            throw new InvalidJwtExpirationRequestedException();
        }

        return expiresInSeconds.Value;
    }

    public string GenerateJwt(ClaimsPrincipal claimsPrincipal, int? expiresInSeconds)
    {
        return IdentityHelpers.GenerateJwt(
            claimsPrincipal.Claims,
            _jwtSettings.Key,
            _jwtSettings.Issuer,
            _jwtSettings.Audience,
            ValidateExpiresInSeconds(expiresInSeconds)
        );
    }

    public RefreshToken CreateAndAddRefreshToken(Guid userId)
    {
        var refreshToken = new RefreshToken(_jwtSettings.RefreshTokenExpiresInDays)
        {
            UserId = userId
        };
        DbContext.RefreshTokens.Add(refreshToken);
        return refreshToken;
    }

    public async Task<ClaimsPrincipal> GetUserFromJwt(string jwt)
    {
        JwtSecurityToken jwtToken;
        try
        {
            jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(jwt) ?? throw new InvalidJwtException();
        }
        catch (ArgumentException)
        {
            throw new InvalidJwtException();
        }

        if (!IdentityHelpers.ValidateToken(jwt, _jwtSettings.Key, _jwtSettings.Issuer, _jwtSettings.Audience, false))
        {
            throw new InvalidJwtException();
        }

        var userName = jwtToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value ??
                       throw new InvalidJwtException();
        var user = await _identityUow.UserManager.FindByNameAsync(userName) ??
                   throw new UserNotFoundException();
        var claimsPrincipal = await _identityUow.SignInManager.CreateUserPrincipalAsync(user);
        return claimsPrincipal;
    }

    public async Task<JwtResult> RefreshToken(string jwt, string refreshToken, int? expiresInSeconds = null)
    {
        JwtSecurityToken jwtToken;
        // get user info from jwt
        try
        {
            jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(jwt) ?? throw new InvalidJwtException();
        }
        catch (ArgumentException)
        {
            throw new InvalidJwtException();
        }

        if (!IdentityHelpers.ValidateToken(jwt, _jwtSettings.Key, _jwtSettings.Issuer, _jwtSettings.Audience))
        {
            throw new InvalidJwtException();
        }

        var userName = jwtToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value ?? throw new InvalidJwtException();

        var user = await _identityUow.UserManager.FindByNameAsync(userName) ?? throw new UserNotFoundException();

        var userRefreshTokens = await DbContext.RefreshTokens.Where(r => r.UserId == user.Id && (
            (r.Token == refreshToken && r.ExpiresAt > DateTime.UtcNow) ||
            (r.PreviousToken == refreshToken && r.PreviousExpiresAt > DateTime.UtcNow)
            )).ToListAsync();

        if (userRefreshTokens.Count == 0)
        {
            throw new NoRefreshTokensException();
        }

        if (userRefreshTokens.Count > 1)
        {
            throw new TooManyRefreshTokensException();
        }

        var claimsPrincipal = await _identityUow.SignInManager.CreateUserPrincipalAsync(user);
        jwt = GenerateJwt(claimsPrincipal, expiresInSeconds);

        var userRefreshToken = userRefreshTokens.First();
        userRefreshToken.Refresh(TimeSpan.FromMinutes(_jwtSettings.ExtendOldRefreshTokenExpirationByMinutes),
            TimeSpan.FromDays(_jwtSettings.RefreshTokenExpiresInDays));
        DbContext.RefreshTokens.Update(userRefreshToken);

        return new JwtResult
        {
            Jwt = jwt,
            RefreshToken = userRefreshToken,
        };
    }
}