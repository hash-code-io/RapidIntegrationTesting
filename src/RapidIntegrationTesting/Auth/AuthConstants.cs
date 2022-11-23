namespace RapidIntegrationTesting.Auth;

/// <summary>
///     Constants used for Authorization
/// </summary>
public static class AuthConstants
{
    /// <summary>
    ///     The key used for the Name claim
    /// </summary>
    public const string JwtNameClaim = "name";

    /// <summary>
    ///     The key used for the Role claim
    /// </summary>
    public const string JwtRoleClaim = "role";

    /// <summary>
    ///     The header used to communicate imerpsonation
    /// </summary>
    public const string TestUserNameHeaderName = "x-testing-user-name";
}