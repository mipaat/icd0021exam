using Domain;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace WebApp.Models;

public class ProductExistencesCreateEditViewModel
{
    [BindNever] [ValidateNever] public Product Product { get; set; } = default!;
    public ProductExistence ProductExistence { get; set; } = default!;
    public string? ReturnUrl { get; set; }
}