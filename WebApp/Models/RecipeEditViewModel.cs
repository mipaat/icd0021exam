using System.ComponentModel.DataAnnotations;
using Domain;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace WebApp.Models;

public class RecipeEditViewModel
{
    public Recipe Recipe { get; set; } = default!;
    [BindNever] [ValidateNever] public List<Product> Products { get; set; } = default!;
    [Display(Prompt = "Search product name")] public string? ProductNameQuery { get; set; }
}