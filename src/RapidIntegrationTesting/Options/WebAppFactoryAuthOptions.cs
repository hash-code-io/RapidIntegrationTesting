using System.Security.Claims;

namespace RapidIntegrationTesting.Options;

/// <summary>
///     Auth Options
/// </summary>
public record WebAppFactoryAuthOptions
{
    /// <summary>
    ///     The default user name to use for Authentication
    /// </summary>
    public const string DefaultTestUserName = "testuser1";

    /// <summary>
    ///     Mappings from UserNames to Claims.
    /// </summary>
    public Dictionary<string, List<Claim>> UserClaimsMapping { get; } = [];

    /// <summary>
    ///     Whether or not to set up a call to AddAuthentication with given options
    /// </summary>
    public bool UseTestAuth { get; set; } = true;

    /// <summary>
    ///     The scheme to use
    /// </summary>
    public string SchemeName { get; set; } = "test";
}