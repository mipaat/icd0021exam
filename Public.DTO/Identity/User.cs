namespace Public.DTO.Identity;

/// <summary>
/// Basic information about a user in the archive.
/// </summary>
public class User
{
    /// <summary>
    /// The unique ID of the user.
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// A unique human-friendly string identifying the user.
    /// </summary>
    public string UserName { get; set; } = default!;
}