namespace Public.DTO;

/// <summary>
/// Enum identifying some possible types of errors that can occur when executing an API method.
/// </summary>
public enum EErrorType
{
    /// <summary>
    /// User with provided username is already registered.
    /// </summary>
    UserAlreadyRegistered,
    /// <summary>
    /// Provided credentials for logging in were invalid.
    /// Could mean that the user doesn't exist or that the password was wrong.
    /// </summary>
    InvalidLoginCredentials,
    /// <summary>
    /// Provided details for registering an account were invalid.
    /// </summary>
    InvalidRegistrationData,

    /// <summary>
    /// Requested token expiration time was invalid.
    /// </summary>
    InvalidTokenExpirationRequested,
    /// <summary>
    /// Provided JWT was invalid.
    /// </summary>
    InvalidJwt,
    /// <summary>
    /// Provided refresh token was invalid.
    /// </summary>
    InvalidRefreshToken,
}