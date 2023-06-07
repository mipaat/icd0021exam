using BLL.DTO;
using BLL.DTO.Exceptions;
using DAL;
using Domain.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BLL.Identity.Services;

public class UserService
{
    private readonly IdentityUow _identityUow;
    private readonly Random _rnd = new();

    public UserService(IdentityUow identityUow)
    {
        _identityUow = identityUow;
    }

    private AppDbContext DbContext => _identityUow.DbContext;

    private SignInManager<User> SignInManager => _identityUow.SignInManager;
    private UserManager<User> UserManager => _identityUow.UserManager;
    private RoleManager<Role> RoleManager => _identityUow.RoleManager;

    public async Task<(IdentityResult identityResult, User user)> CreateUser(string username,
        string password)
    {
        var user = new User
        {
            UserName = username,
        };

        var result = await SignInManager.UserManager.CreateAsync(user, password);
        return (result, user);
    }

    public async Task<JwtResult> SignInJwtAsync(string username, string password, int? expiresInSeconds = null,
        bool lockOutOnFailure = false)
    {
        var user = await UserManager.FindByNameAsync(username);
        if (user == null)
        {
            throw new UserNotFoundException();
        }

        var result = await SignInManager.CheckPasswordSignInAsync(user, password, lockOutOnFailure);
        if (!result.Succeeded)
        {
            await DelayRandom();
            throw new WrongPasswordException();
        }

        await _identityUow.TokenService.DeleteExpiredRefreshTokensAsync(user.Id);
        var refreshToken = _identityUow.TokenService.CreateAndAddRefreshToken(user.Id);

        var claimsPrincipal = await SignInManager.CreateUserPrincipalAsync(user);
        var jwt = _identityUow.TokenService.GenerateJwt(claimsPrincipal, expiresInSeconds);

        return new JwtResult
        {
            Jwt = jwt,
            RefreshToken = refreshToken,
        };
    }

    private async Task DelayRandom(int minValueMs = 100, int maxValueMs = 1000)
    {
        await Task.Delay(_rnd.Next(minValueMs, maxValueMs));
    }

    public async Task SignOutTokenAsync(string jwt, string refreshToken)
    {
        // Delete the refresh token - so user is kicked out after jwt expiration
        // We do not invalidate the jwt - that would require pipeline modification and checking against db on every request
        // So client can actually continue to use the jwt until it expires (keep the jwt expiration time short ~1 min)
        await _identityUow.TokenService.DeleteRefreshTokenAsync(jwt, refreshToken);
    }

    public async Task<JwtResult?> RegisterUserAsync(string username, string password, int? expiresInSeconds)
    {
        var user = await UserManager.FindByNameAsync(username);
        if (user != null)
        {
            throw new UserAlreadyRegisteredException();
        }

        var (result, _) = await CreateUser(username, password);
        if (!result.Succeeded)
        {
            throw new IdentityOperationFailedException(result.Errors);
        }

        user = await UserManager.FindByNameAsync(username);
        if (user == null)
        {
            throw new IdentityOperationFailedException($"User with username {username} not found after registration");
        }

        var claimsPrincipal = await SignInManager.CreateUserPrincipalAsync(user);
        var jwt = _identityUow.TokenService.GenerateJwt(claimsPrincipal, expiresInSeconds);
        var refreshToken = _identityUow.TokenService.CreateAndAddRefreshToken(user.Id);

        return new JwtResult
        {
            Jwt = jwt,
            RefreshToken = refreshToken,
        };
    }

    public async Task<IList<AuthenticationScheme>> GetExternalAuthenticationSchemesAsync() =>
        (await SignInManager.GetExternalAuthenticationSchemesAsync()).ToList();

    public async Task<User?> GetUserWithRoles(Guid id)
    {
        return await DbContext.Users
            .Include(e => e.UserRoles!)
            .ThenInclude(e => e.Role)
            .Where(e => e.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task<ICollection<User>> GetUsersWithRoles(string? nameQuery = null)
    {
        if (nameQuery != null && nameQuery.Trim().Length == 0) nameQuery = null;
        IQueryable<User> query = DbContext.Users
            .Include(e => e.UserRoles!)
            .ThenInclude(e => e.Role);

        if (nameQuery != null && nameQuery.Trim().Length > 0)
        {
            nameQuery = "%" + nameQuery.ToUpper() + "%";
            query = query.Where(e => e.NormalizedUserName != null &&
                                     EF.Functions.Like(e.NormalizedUserName, nameQuery));
        }

        return await query.ToListAsync();
    }

    public async Task AddUserToRole(Guid userId, string roleName)
    {
        var role = await RoleManager.FindByNameAsync(roleName) ?? throw new NotFoundException();
        if (await DbContext.UserRoles.AnyAsync(e => e.UserId == userId && e.RoleId == role.Id)) return;
        DbContext.UserRoles.Add(new UserRole
        {
            RoleId = role.Id,
            UserId = userId,
        });
    }

    public async Task RemoveUserFromRole(Guid userId, string roleName)
    {
        var role = await RoleManager.FindByNameAsync(roleName) ?? throw new NotFoundException();
        await DbContext.UserRoles.Where(e => e.UserId == userId && e.RoleId == role.Id).ExecuteDeleteAsync();
    }
}