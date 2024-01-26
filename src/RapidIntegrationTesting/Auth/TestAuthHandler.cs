using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using RapidIntegrationTesting.Options;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace RapidIntegrationTesting.Auth;

[SuppressMessage("Design", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by framework")]
internal sealed class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly List<Claim> _defaultUserClaims = new() { new Claim(AuthConstants.JwtNameClaim, WebAppFactoryAuthOptions.DefaultTestUserName) };
    private readonly WebAppFactoryAuthOptions _options;

    public TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, WebAppFactoryAuthOptions authOptions)
        : base(options, logger, encoder) =>
        _options = authOptions ?? throw new ArgumentNullException(nameof(authOptions));

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        List<Claim> claims = GetClaims();
        var identity = new ClaimsIdentity(claims, _options.SchemeName, AuthConstants.JwtNameClaim, AuthConstants.JwtRoleClaim);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, _options.SchemeName);

        var result = AuthenticateResult.Success(ticket);

        return Task.FromResult(result);
    }

    private List<Claim> GetClaims()
    {
        if (!Request.Headers.TryGetValue(AuthConstants.TestUserNameHeaderName, out StringValues value))
            return GetDefaultUserClaims();

        if (value.Count != 1) throw new InvalidOperationException($"Header value for key {AuthConstants.TestUserNameHeaderName} returned {value.Count} entries instead of 1");
        string userName = value[0]!;

        if (!_options.UserClaimsMapping.TryGetValue(userName, out List<Claim>? claims))
            throw new InvalidOperationException($"{nameof(WebAppFactoryAuthOptions)}.{nameof(WebAppFactoryAuthOptions.UserClaimsMapping)} did not contain an entry for {userName}");
        return claims;
    }

    private List<Claim> GetDefaultUserClaims() =>
        _options.UserClaimsMapping.TryGetValue(WebAppFactoryAuthOptions.DefaultTestUserName, out List<Claim>? claims)
            ? claims
            : _defaultUserClaims;
}