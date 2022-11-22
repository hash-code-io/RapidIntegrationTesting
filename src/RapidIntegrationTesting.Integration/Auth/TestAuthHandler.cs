using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RapidIntegrationTesting.Integration.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace RapidIntegrationTesting.Integration.Auth;

internal class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private const string JwtNameClaim = "name";
    private const string JwtRoleClaim = "role";
    private readonly WebAppFactoryAuthOptions _options;
    private static List<Claim>? _current;

    public TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, WebAppFactoryAuthOptions authOptions)
        : base(options, logger, encoder, clock)
    {
        _options = authOptions ?? throw new ArgumentNullException(nameof(authOptions));

        Default = _options.DefaultClaims.ToList();
        Default.Add(new Claim(JwtNameClaim, _options.DefaultTestUserName));
    }

    private List<Claim> Default { get; }
    private List<Claim> UserClaims => _current ?? Default;

    private static void AssignClaims(List<Claim> claims) => _current = claims;
    private static void UnassignClaims() => _current = null;

    public static async Task RunAsUser(string userName, Func<Task> testCode, IEnumerable<Claim>? additionalClaims)
    {
        try
        {
            var claims = new List<Claim>(additionalClaims ?? Enumerable.Empty<Claim>()) { new(JwtNameClaim, userName) };
            AssignClaims(claims);
            await testCode().ConfigureAwait(false);
        }
        finally
        {
            UnassignClaims();
        }
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var identity = new ClaimsIdentity(UserClaims, _options.SchemeName, JwtNameClaim, JwtRoleClaim);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, _options.SchemeName);

        var result = AuthenticateResult.Success(ticket);

        return Task.FromResult(result);
    }
}