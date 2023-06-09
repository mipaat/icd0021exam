using BLL.DTO;

namespace Public.DTO;

public class RecipeSearch
{
    public string? NameQuery { get; set; }
    public string? IncludesIngredientQuery { get; set; }
    public string? ExcludesIngredientQuery { get; set; }
    public int? MinPrepareTime { get; set; }
    public int? MaxPrepareTime { get; set; }
    public float? Servings { get; set; }
    public bool FilterServable { get; set; }

    public ERecipePrivacyFilter PrivacyFilter { get; set; } = ERecipePrivacyFilter.All;
}