using System.Security.Claims;

namespace BLL.Identity;

public static class AuthHelpers
{
    public static bool IsAllowedToManageRole(this ClaimsPrincipal user, string roleName)
    {
        return user.IsInRole(RoleNames.Admin) && roleName != RoleNames.Admin;
    }
}