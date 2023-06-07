using System.ComponentModel.DataAnnotations;

namespace Public.DTO.Identity;

/// <summary>
/// Required data for registering a new user account.
/// </summary>
public class Register
{
    /// <summary>
    /// The unique username for the user account.
    /// </summary>
    [StringLength(128, MinimumLength = 1, ErrorMessage = "Incorrect length")]
    public string Username { get; set; } = default!;

    /// <summary>
    /// The password that will be used to log in to the user account.
    /// </summary>
    [StringLength(128, MinimumLength = 1, ErrorMessage = "Incorrect length")]
    public string Password { get; set; } = default!;
}