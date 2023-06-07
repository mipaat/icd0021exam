using System.ComponentModel.DataAnnotations;

namespace Public.DTO.Identity;

/// <summary>
/// Required data for logging in a user using password authentication.
/// </summary>
public class Login
{
    /// <summary>
    /// The unique username of the user.
    /// </summary>
    [StringLength(maximumLength:128, MinimumLength = 1, ErrorMessage = "Wrong length on username")] 
    public string Username { get; set; } = default!;
    /// <summary>
    /// The password of the user.
    /// </summary>
    public string Password { get; set; } = default!;
}