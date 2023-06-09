using System.ComponentModel.DataAnnotations;
using Base.Domain;

namespace Domain;

public class Product : AbstractIdDatabaseEntity
{
    [MaxLength(256)] public string Name { get; set; } = default!;
    [MaxLength(32)] public string? Unit { get; set; }

    [Display(Name = "Product existences")]
    public ICollection<ProductExistence>? ProductExistences { get; set; }
    public ICollection<ProductCategory>? ProductCategories { get; set; }
    public ICollection<RecipeProduct>? RecipeProducts { get; set; }
}