using Public.DTO.Identity;

namespace Public.DTO;

public class Recipe
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public bool IsPrivate { get; set; }
    public float Servings { get; set; }
    public int PrepareTimeMinutes { get; set; }
    public string? Instructions { get; set; }
    public User Creator { get; set; } = default!;
}