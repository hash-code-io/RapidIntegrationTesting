using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using RapidIntegrationTesting.Docker;

namespace RapidIntegrationTesting.Integration.Tests;

public class SqlContainerBootstrapper : ContainerBootstrapper<MsSqlTestcontainer>
{
    protected override MsSqlTestcontainer BuildContainer(ITestcontainersBuilder<MsSqlTestcontainer> builder)
        => builder.WithDatabase(new MsSqlTestcontainerConfiguration { Password = "My@Cool$PassWord123", Database = "Testing"}).Build();
}