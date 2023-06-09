namespace Public.DTO;

public class RecipeProduct
{
    public Guid Id { get; set; }
    public Product Product { get; set; } = default!;
    public float Amount { get; set; }
}