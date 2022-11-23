using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using RapidIntegrationTesting.Auth;
using RapidIntegrationTesting.ContainerManagement;
using RapidIntegrationTesting.Options;
using RapidIntegrationTesting.xUnit;
using System.Security.Claims;
using Testing.Integration.TestWebApi;
using Testing.Integration.TestWebApi.Controllers;

namespace RapidIntegrationTesting.Integration.Tests;

public class TestWebAppFactory : XUnitTestingWebAppFactory<UsersController>
{
    private const string AdminUserName = "adminUser";

    protected override Action<WebAppFactoryOptions> ConfigureOptions => o =>
    {
        o.Auth.UserClaimsMapping.Add(AdminUserName, new List<Claim> { new(AuthConstants.JwtNameClaim, AdminUserName), new(AuthConstants.JwtRoleClaim, AppConstants.AdminRoleName) });

        o.Container.Configurations = new List<ContainerStartCallback>
        {
            async () =>
            {
                MsSqlTestcontainer container = new TestcontainersBuilder<MsSqlTestcontainer>()
                    .WithDatabase(new MsSqlTestcontainerConfiguration { Password = "My@Cool$PassWord123", Database = "Testing" })
                    .Build();

                await container.StartAsync();

                var configs = new ContainerConfigurations { new(AppConstants.SqlConnectionStringKey, container.ConnectionString + "TrustServerCertificate=true;") };

                return new RunningContainerInfo(container, configs);
            }
        };
    };

    public Task RunAsAdmin(Func<HttpClient, Task> testCode) => RunAsUser(AdminUserName, testCode);
}