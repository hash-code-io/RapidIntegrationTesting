using RapidIntegrationTesting.Auth;
using RapidIntegrationTesting.Options;
using RapidIntegrationTesting.xUnit;
using System.Security.Claims;
using Testing.Integration.TestWebApi;
using Testing.Integration.TestWebApi.Controllers;

namespace RapidIntegrationTesting.Integration.Tests;

public class TestWebAppFactory : XUnitTestingWebAppFactory<UsersController>
{
    private const string AdminUserName = "adminUser";

    protected override void ConfigureOptions(WebAppFactoryOptions o)
    {
        o.Auth.UserClaimsMapping.Add(AdminUserName, [new(AuthConstants.JwtNameClaim, AdminUserName), new(AuthConstants.JwtRoleClaim, AppConstants.AdminRoleName)]);
    }

    public Task RunAsAdmin(Func<HttpClient, Task> testCode) => RunAsUser(AdminUserName, testCode);
}