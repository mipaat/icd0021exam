using BLL.Identity;
using Domain.Identity;

#pragma warning disable CS1591

namespace WebApp.Areas.Admin.ViewModels;

public class ManageRolesViewModel
{
    public User User { get; set; } = default!;
    public IEnumerable<string> OtherRoles => RoleNames.AllAsList.Where(r => User.UserRoles!.All(ur => r != ur.Role!.Name));
}