using DotNet.Testcontainers.Containers;
using RapidIntegrationTesting.Integration.ContainerManagement;
using RapidIntegrationTesting.Integration.Options;
using RapidIntegrationTesting.Integration.xUnit;
using Testing.Integration.TestWebApi;
using Testing.Integration.TestWebApi.Controllers;

namespace RapidIntegrationTesting.Integration.Tests;

public class TestWebAppFactory : XUnitTestingWebAppFactory<UsersController>
{
    protected override Action<WebAppFactoryOptions> ConfigureOptions => o =>
    {
        o.Container.ContainerBootstrapperAssemblyMarkers = new List<Type> { typeof(SqlContainerBootstrapper) };

        o.Container.Configurations = new List<ContainerConfigureCallback>
        {
            async bootstrapper =>
            {
                MsSqlTestcontainer container = await bootstrapper.Bootstrap<MsSqlTestcontainer>();
                return new ContainerConfigurations { new(AppConstants.SqlConnectionStringKey, container.ConnectionString + "TrustServerCertificate=true;") };
            }
        };
    };
}