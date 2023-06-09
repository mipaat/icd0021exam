using System.ComponentModel.DataAnnotations;
using BLL.DTO;
using Domain;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace WebApp.Models;

public class RecipesIndexModel
{
    [BindNever] [ValidateNever] public List<Recipe> Recipes { get; set; } = default!;
    public string? NameQuery { get; set; }
    [Display(Name = "Include these ingredients", Prompt = "carrot, milk...")]
    public string? IncludesIngredientQuery { get; set; }
    [Display(Name = "Exclude these ingredients", Prompt = "mold, rotting flesh...")]
    public string? ExcludesIngredientQuery { get; set; }
    public int? MinPrepareTime { get; set; }
    public int? MaxPrepareTime { get; set; }
    [Display(Prompt = "Custom servings amount")] [Range(0.1, 9999)] public float? Servings { get; set; }
    [Display(Name = "Include only meals that can be prepared")] public bool FilterServable { get; set; }

    public ERecipePrivacyFilter PrivacyFilter { get; set; } = ERecipePrivacyFilter.All;
}