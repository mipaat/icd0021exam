namespace Public.DTO;

/// <summary>
/// Information about an error that occurred while responding to a request to the API.
/// </summary>
public class RestApiErrorResponse
{
    /// <summary>
    /// Custom error code identifying the type of error.
    /// </summary>
    public EErrorType ErrorType { get; set; }
    /// <summary>
    /// Text describing the error.
    /// </summary>
    public string Error { get; set; } = default!;
}