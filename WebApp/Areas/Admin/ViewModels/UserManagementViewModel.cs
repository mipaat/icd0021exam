using System.ComponentModel.DataAnnotations;
using Domain.Identity;

#pragma warning disable CS1591

namespace WebApp.Areas.Admin.ViewModels;

public class UserManagementViewModel
{
    public ICollection<User> Users { get; set; } = default!;

    [Display(Name = "Search by username", Prompt = "Enter name query...")]
    public string? NameQuery { get; set; }
}