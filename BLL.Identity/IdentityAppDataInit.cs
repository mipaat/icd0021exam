using BLL.Identity.Config;
using Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace BLL.Identity;

public class IdentityAppDataInit
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;
    private bool _identitySeeded;
    private readonly IdentityAppDataInitSettings _config;

    public IdentityAppDataInit(UserManager<User> userManager, RoleManager<Role> roleManager,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _config = configuration.GetIdentityAppDataInitSettings();
    }

    public async Task RunInitAsync()
    {
        if (_config.SeedIdentity)
        {
            await SeedIdentityAsync();
        }

        if (_config.SeedDemoIdentity)
        {
            await SeedDemoIdentityAsync();
        }
    }

    private async Task SeedRolesAsync()
    {
        foreach (var roleName in RoleNames.AllAsList)
        {
            await GetOrCreateRoleAsync(roleName);
        }
    }

    private async Task SeedIdentityAsync()
    {
        if (_identitySeeded) return;
        await SeedRolesAsync();
        var adminUserData = new BasicUserData("admin", _config.AdminPassword ?? "admin123", true, RoleNames.Admin);
        await GetOrCreateUserAsync(adminUserData);
        _identitySeeded = true;
    }

    private async Task SeedDemoIdentityAsync()
    {
        await SeedIdentityAsync();

        var demoUserData =
            new BasicUserData("demo", "demo123", true);
        await GetOrCreateUserAsync(demoUserData);
    }

    private async Task GetOrCreateUserAsync(BasicUserData userData)
    {
        var user = await _userManager.FindByNameAsync(userData.UserName);
        if (user == null)
        {
            user = new User
            {
                UserName = userData.UserName,
            };
            var result = await _userManager.CreateAsync(user, userData.Password);
            if (!result.Succeeded)
            {
                throw new ApplicationException(
                    $"Failed to create user from {userData} - {ToLogString(result.Errors)}");
            }
        }

        foreach (var roleName in userData.Roles)
        {
            if (!await _userManager.IsInRoleAsync(user, roleName))
            {
                var result = await _userManager.AddToRoleAsync(user, roleName);
                if (!result.Succeeded)
                {
                    throw new ApplicationException(
                        $"Failed to add role {roleName} to user {user.UserName} - {ToLogString(result.Errors)}");
                }
            }
        }
    }

    private static string ToLogString(IEnumerable<IdentityError> errors)
    {
        return $"[{string.Join(", ", errors.Select(e => $"{e.Code} - {e.Description}"))}]";
    }

    private async Task GetOrCreateRoleAsync(string roleName)
    {
        var role = await _roleManager.FindByNameAsync(roleName);
        if (role == null)
        {
            role = new Role
            {
                Name = roleName,
            };
            var result = await _roleManager.CreateAsync(role);
            if (!result.Succeeded)
            {
                throw new ApplicationException(
                    $"Failed to create role '{roleName}' - {ToLogString(result.Errors)}");
            }
        }
    }
}

internal record BasicUserData(string UserName, string Password, bool IsApproved, params string[] Roles);