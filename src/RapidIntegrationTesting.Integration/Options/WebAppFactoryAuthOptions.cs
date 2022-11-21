using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;

namespace RapidIntegrationTesting.Integration.Options;

/// <summary>
///     Auth Options
/// </summary>
public record WebAppFactoryAuthOptions
{
    /// <summary>
    ///     Whether or not to set up a call to AddAuthentication with given options
    /// </summary>
    public bool UseTestAuth { get; set; } = true;

    /// <summary>
    ///     The scheme to use
    /// </summary>
    public string SchemeName { get; set; } = "test";


    /// <summary>
    ///     The default user name to use for Authentication
    /// </summary>
    public string DefaultTestUserName { get; set; } = "testuser1";

    /// <summary>
    ///     Default claims to use for authentication. Should not include the "name" claim, as that is set by <see cref="WebAppFactoryAuthOptions" />.<see cref="DefaultTestUserName" />
    /// </summary>
    public List<Claim> DefaultClaims { get; set; } = new();
}