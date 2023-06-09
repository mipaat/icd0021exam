using Domain;

namespace WebApp.Models;

public class ManageRecipeProductsPartialViewModel
{
    public Guid RecipeId { get; set; }
    public List<RecipeProduct> RecipeProducts { get; set; } = default!;
    public List<Product> Products { get; set; } = default!;
}