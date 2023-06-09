using System.ComponentModel.DataAnnotations;
using Base.Domain;
using Domain.Identity;

namespace Domain;

public class Recipe : AbstractIdDatabaseEntity
{
    [MaxLength(256)] public string Name { get; set; } = default!;
    [Display(Name = "Is private?")]
    public bool IsPrivate { get; set; }
    public float Servings { get; set; }
    [Display(Name = "Preparation time (minutes)")]
    [Range(1, 9999)] public int PrepareTimeMinutes { get; set; }

    public Guid? CreatorId { get; set; }
    public User? Creator { get; set; }

    public ICollection<RecipeProduct>? RecipeProducts { get; set; }
    public ICollection<RecipeCategory>? RecipeCategories { get; set; }
}