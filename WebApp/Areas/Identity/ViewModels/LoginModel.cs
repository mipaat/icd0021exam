using System.ComponentModel.DataAnnotations;

namespace WebApp.Areas.Identity.ViewModels;

public class LoginModel
{
    [Required(ErrorMessage = "Username is required")]
    [Display(Name = "Username", Prompt = "Enter username")]
    public string UserName { get; set; } = "";

    [Required(ErrorMessage = "Password is required")]
    [Display(Name = "Password", Prompt = "Enter password")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = "";

    [Display(Name = "Remember me?")]
    public bool RememberMe { get; set; }
    
    public string? ReturnUrl { get; set; }
}