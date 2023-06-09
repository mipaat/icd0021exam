using System.ComponentModel.DataAnnotations;
using Domain;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace WebApp.Models;

public class RecipeDetailsViewModel
{
    [BindNever] [ValidateNever] public Recipe Recipe { get; set; } = default!;
    [Display(Name = "Custom servings", Prompt = "Custom servings amount")] public float? Servings { get; set; }
}