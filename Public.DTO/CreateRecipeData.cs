namespace Public.DTO;

public class CreateRecipeData
{
    public string Name { get; set; } = default!;
    public bool IsPrivate { get; set; }
    public float Servings { get; set; }
    public int PrepareTimeMinutes { get; set; }
    public string? Instructions { get; set; }
}