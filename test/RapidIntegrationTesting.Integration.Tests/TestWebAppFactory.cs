using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using RapidIntegrationTesting.ContainerManagement;
using RapidIntegrationTesting.Options;
using RapidIntegrationTesting.xUnit;
using Testing.Integration.TestWebApi;
using Testing.Integration.TestWebApi.Controllers;

namespace RapidIntegrationTesting.Integration.Tests;

public class TestWebAppFactory : XUnitTestingWebAppFactory<UsersController>
{
    protected override Action<WebAppFactoryOptions> ConfigureOptions => o =>
    {
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
}