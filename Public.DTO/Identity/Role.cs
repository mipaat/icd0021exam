namespace Public.DTO.Identity;

/// <summary>
/// A role that a user may be in.
/// Some actions can only be performed by users in certain roles.
/// </summary>
public class Role
{
    /// <summary>
    /// The unique ID of the role.
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// The (unique) name of the role.
    /// </summary>
    public string Name { get; set; } = default!;
}