using System.ComponentModel.DataAnnotations;

namespace WebApp.Areas.Identity.ViewModels;

public class RegisterModel
{
    [Required(ErrorMessage = "Username is required")]
    [Display(Name = "Username", Prompt = "Enter username")]
    public string UserName { get; set; } = "";

    [Required(ErrorMessage = "Password is required")]
    [StringLength(100, MinimumLength = 6)]
    [Display(Name = "Password", Prompt = "Enter password")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = "";

    [Compare(nameof(Password), ErrorMessage = "Passwords do not match")]
    [Display(Name = "Confirm password", Prompt = "Repeat your password")]
    [DataType(DataType.Password)]
    public string ConfirmPassword { get; set; } = "";

    public string? ReturnUrl { get; set; }
}