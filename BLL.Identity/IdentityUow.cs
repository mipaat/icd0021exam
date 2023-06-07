using BLL.Identity.Services;
using DAL;
using Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace BLL.Identity;

public class IdentityUow
{
    private readonly IServiceProvider _services;
    public readonly AppDbContext DbContext;

    public IdentityUow(AppDbContext dbContext, IServiceProvider services)
    {
        _services = services;
        DbContext = dbContext;
    }

    private SignInManager<User>? _signInManager;
    public SignInManager<User> SignInManager => _signInManager ??= _services.GetRequiredService<SignInManager<User>>();
    private UserManager<User>? _userManager;
    public UserManager<User> UserManager => _userManager ??= _services.GetRequiredService<UserManager<User>>();

    private RoleManager<Role>? _roleManager;
    public RoleManager<Role> RoleManager => _roleManager ??= _services.GetRequiredService<RoleManager<Role>>();

    private UserService? _userService;
    public UserService UserService => _userService ??= _services.GetRequiredService<UserService>();

    private TokenService? _tokenService;
    public TokenService TokenService => _tokenService ??= _services.GetRequiredService<TokenService>();
}