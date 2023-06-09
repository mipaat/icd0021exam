using System.Security.Claims;
using Base.WebHelpers;
using Domain;

namespace BLL.Identity;

public static class AuthHelpers
{
    public static bool IsAllowedToManageRole(this ClaimsPrincipal user, string roleName)
    {
        return user.IsInRole(RoleNames.Admin) && roleName != RoleNames.Admin;
    }

    public static bool IsAdmin(this ClaimsPrincipal user) => user.IsInRole(RoleNames.Admin);

    public static bool IsAllowedToManageRecipe(this ClaimsPrincipal user, Recipe recipe) => user.IsAdmin() ||
        (recipe.CreatorId != null && recipe.CreatorId == user.GetUserIdIfExists());
}