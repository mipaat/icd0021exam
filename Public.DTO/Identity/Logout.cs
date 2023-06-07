namespace Public.DTO.Identity;

/// <summary>
/// Required data for logging out a user by deleting their refresh token.
/// </summary>
public class Logout
{
    /// <summary>
    /// The JWT that the refresh token belongs to.
    /// </summary>
    public string Jwt { get; set; } = default!;
    /// <summary>
    /// The refresh token to delete.
    /// </summary>
    public string RefreshToken { get; set; } = default!;
}