using System.ComponentModel.DataAnnotations;
using Base.Domain;

namespace Domain;

public class Category : AbstractIdDatabaseEntity
{
    [MaxLength(256)] public string Name { get; set; } = default!;
    public bool SupportsRecipes { get; set; }
    public bool SupportsProducts { get; set; }

    public ICollection<ProductCategory>? ProductCategories { get; set; }
    public ICollection<RecipeCategory>? RecipeCategories { get; set; }
}