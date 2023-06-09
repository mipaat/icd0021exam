using Base.Domain;

namespace Domain;

public class RecipeProduct : AbstractIdDatabaseEntity
{
    public Guid RecipeId { get; set; }
    public Recipe? Recipe { get; set; }

    public Guid ProductId { get; set; }
    public Product? Product { get; set; }

    public float Amount { get; set; }
}