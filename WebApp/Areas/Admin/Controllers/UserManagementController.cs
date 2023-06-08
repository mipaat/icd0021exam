using Base.WebHelpers;
using BLL.Identity;
using DAL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.Areas.Admin.ViewModels;

#pragma warning disable CS1591

namespace WebApp.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = RoleNames.Admin)]
public class UserManagementController : Controller
{
    private readonly IdentityUow _identityUow;
    private readonly AppDbContext _dbContext;

    public UserManagementController(IdentityUow identityUow, AppDbContext dbContext)
    {
        _identityUow = identityUow;
        _dbContext = dbContext;
    }

    [BindProperty(SupportsGet = true)] public string? NameQuery { get; set; }

    public async Task<IActionResult> Index()
    {
        return View(new UserManagementViewModel
        {
            Users = (await _identityUow.UserService.GetUsersWithRoles(nameQuery: NameQuery))
                .Where(u => u.Id != User.GetUserId()).ToList(),
            NameQuery = NameQuery,
        });
    }

    public async Task<IActionResult> ManageRoles(Guid userId)
    {
        var user = await _identityUow.UserService.GetUserWithRoles(userId);
        if (user == null) return NotFound();
        return View(new ManageRolesViewModel
        {
            User = user
        });
    }

    [HttpPost]
    public async Task<IActionResult> AddRole(Guid userId, string roleName)
    {
        if (!User.IsAllowedToManageRole(roleName)) return Forbid();
        await _identityUow.UserService.AddUserToRole(userId, roleName);
        await _dbContext.SaveChangesAsync();
        return RedirectToAction(nameof(ManageRoles), new { userId });
    }

    [HttpPost]
    public async Task<IActionResult> RemoveRole(Guid userId, string roleName)
    {
        if (!User.IsAllowedToManageRole(roleName)) return Forbid();
        await _identityUow.UserService.RemoveUserFromRole(userId, roleName);
        // No SaveChanges, this already calls ExecuteDeleteAsync(). Reassess if not using EF Core.
        return RedirectToAction(nameof(ManageRoles), new { userId });
    }
}