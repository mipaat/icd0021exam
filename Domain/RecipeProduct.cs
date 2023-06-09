using System.ComponentModel.DataAnnotations;
using Base.Domain;

namespace Domain;

public class RecipeProduct : AbstractIdDatabaseEntity
{
    public Guid RecipeId { get; set; }
    public Recipe? Recipe { get; set; }

    public Guid ProductId { get; set; }
    public Product? Product { get; set; }

    [Range(0.1, 9999)] public float Amount { get; set; }
}