using Base.Domain;
using Microsoft.AspNetCore.Identity;

namespace Domain.Identity;

public class User : IdentityUser<Guid>, IIdDatabaseEntity
{
    public ICollection<UserRole>? UserRoles { get; set; }
    public ICollection<RefreshToken>? RefreshTokens { get; set; }

    public ICollection<ProductExistence>? ProductExistences { get; set; }
    public ICollection<Recipe>? Recipes { get; set; }
}