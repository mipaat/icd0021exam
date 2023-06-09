using Domain;

namespace WebApp.Models;

public class SetRecipeProductData
{
    public RecipeProduct RecipeProduct { get; set; } = default!;
    public string? ReturnUrl { get; set; }
}