using Base.Domain;

namespace Domain;

public class RecipeCategory : AbstractIdDatabaseEntity
{
    public Guid RecipeId { get; set; }
    public Recipe? Recipe { get; set; }

    public Guid CategoryId { get; set; }
    public Category? Category { get; set; }
}