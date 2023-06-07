using BLL.DTO.Exceptions;
using BLL.Identity;
using DAL;
using Microsoft.AspNetCore.Mvc;
using Public.DTO;
using Public.DTO.Identity;

namespace WebApp.ApiControllers.Identity;

/// <summary>
/// API controller for user account management endpoints
/// </summary>
[ApiController]
[Route("api/identity/[controller]/[action]")]
public class AccountController : ControllerBase
{
    private readonly IdentityUow _identityUow;
    private readonly AppDbContext _dbContext;

    public AccountController(IdentityUow identityUow, AppDbContext dbContext)
    {
        _identityUow = identityUow;
        _dbContext = dbContext;
    }

    [HttpPost]
    public async Task<ActionResult<JwtResponse>> Register([FromBody] Register registrationData,
        [FromQuery] int? expiresInSeconds = null)
    {
        try
        {
            var jwtResult = await _identityUow.UserService.RegisterUserAsync(
                registrationData.Username,
                registrationData.Password,
                expiresInSeconds);
            await _dbContext.SaveChangesAsync();
            if (jwtResult == null)
            {
                return Accepted();
            }

            return new JwtResponse
            {
                Jwt = jwtResult.Jwt,
                RefreshToken = jwtResult.RefreshToken.Token,
                RefreshTokenExpiresAt = jwtResult.RefreshToken.ExpiresAt,
            };
        }
        catch (UserAlreadyRegisteredException e)
        {
            return BadRequest(new RestApiErrorResponse
            {
                ErrorType = EErrorType.UserAlreadyRegistered,
                Error = e.Message,
            });
        }
        catch (InvalidJwtExpirationRequestedException e)
        {
            return BadRequest(new RestApiErrorResponse
            {
                ErrorType = EErrorType.InvalidTokenExpirationRequested,
                Error = e.Message,
            });
        }
        catch (IdentityOperationFailedException e)
        {
            return BadRequest(new RestApiErrorResponse
            {
                ErrorType = EErrorType.InvalidRegistrationData,
                Error = string.Join(", ", e.Errors),
            });
        }
    }

    [HttpPost]
    public async Task<ActionResult<JwtResponse>> LogIn([FromBody] Login loginData,
        [FromQuery] int? expiresInSeconds = null)
    {
        try
        {
            var jwtResult =
                await _identityUow.UserService.SignInJwtAsync(loginData.Username, loginData.Password, expiresInSeconds);
            await _dbContext.SaveChangesAsync();
            return Ok(new JwtResponse
            {
                Jwt = jwtResult.Jwt,
                RefreshToken = jwtResult.RefreshToken.Token,
                RefreshTokenExpiresAt = jwtResult.RefreshToken.ExpiresAt,
            });
        }
        catch (UserNotFoundException)
        {
            return NotFound(new RestApiErrorResponse
            {
                ErrorType = EErrorType.InvalidLoginCredentials,
                Error = "Username/password problem",
            });
        }
        catch (WrongPasswordException)
        {
            return NotFound(new RestApiErrorResponse
            {
                ErrorType = EErrorType.InvalidLoginCredentials,
                Error = "Username/password problem",
            });
        }
    }

    [HttpPost]
    public async Task<ActionResult<JwtResponse>> RefreshToken(
        [FromBody] RefreshTokenModel refreshTokenModel,
        [FromQuery] int? expiresInSeconds = null)
    {
        try
        {
            var jwtResult = await _identityUow.TokenService.RefreshToken(refreshTokenModel.Jwt,
                refreshTokenModel.RefreshToken,
                expiresInSeconds);
            await _dbContext.SaveChangesAsync();
            return Ok(new JwtResponse
            {
                Jwt = jwtResult.Jwt,
                RefreshToken = jwtResult.RefreshToken.Token,
                RefreshTokenExpiresAt = jwtResult.RefreshToken.ExpiresAt,
            });
        }
        catch (InvalidJwtException)
        {
            return BadRequest(new RestApiErrorResponse
            {
                ErrorType = EErrorType.InvalidJwt,
                Error = "Invalid JWT",
            });
        }
        catch (NoRefreshTokensException)
        {
            return BadRequest(new RestApiErrorResponse
            {
                ErrorType = EErrorType.InvalidRefreshToken,
                Error = "Invalid refresh token (probably expired)",
            });
        }
    }

    [HttpPost]
    public async Task<ActionResult> Logout(
        [FromBody] Logout logout)
    {
        try
        {
            await _identityUow.UserService.SignOutTokenAsync(logout.Jwt, logout.RefreshToken);
            return Ok();
        }
        catch (InvalidJwtException)
        {
            return BadRequest(new RestApiErrorResponse
            {
                ErrorType = EErrorType.InvalidJwt,
                Error = "Provided JWT was invalid",
            });
        }
    }
}