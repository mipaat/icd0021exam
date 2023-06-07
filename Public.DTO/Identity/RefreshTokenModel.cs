namespace Public.DTO.Identity;

/// <summary>
/// Required data for refreshing a JWT and refresh token.
/// </summary>
public class RefreshTokenModel
{
    /// <summary>
    /// The JWT to refresh.
    /// </summary>
    public string Jwt { get; set; } = default!;
    /// <summary>
    /// The refresh token to use for refreshing, and to replace with a new refresh token.
    /// </summary>
    public string RefreshToken { get; set; } = default!;
}