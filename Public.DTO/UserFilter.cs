namespace Public.DTO;

/// <summary>
/// Filtering conditions for listing user accounts.
/// </summary>
public class UserFilter
{
    /// <summary>
    /// If not null, results should include only user accounts with usernames containing this string.
    /// </summary>
    public string? NameQuery { get; set; }
}