namespace Public.DTO.Identity;

/// <summary>
/// Information about a user in the archive, including their roles.
/// </summary>
public class UserWithRoles
{
    /// <summary>
    /// The unique ID of the user.
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// A unique human-friendly string identifying the user.
    /// </summary>
    public string UserName { get; set; } = default!;
    /// <summary>
    /// The roles that this user is in.
    /// </summary>
    public ICollection<Role> Roles { get; set; } = default!;
}