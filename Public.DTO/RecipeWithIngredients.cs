namespace Public.DTO;

public class RecipeWithIngredients : Recipe
{
    public List<RecipeProduct> RecipeProducts { get; set; } = default!;
}