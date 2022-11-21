using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;

namespace RapidIntegrationTesting.Docker.Tests.Setup;

public class SqlContainerBootstrapper : ContainerBootstrapper<MsSqlTestcontainer>
{
    protected override MsSqlTestcontainer BuildContainer(ITestcontainersBuilder<MsSqlTestcontainer> builder)
        => builder.WithDatabase(new MsSqlTestcontainerConfiguration { Password = "My@Cool$PassWord123" }).Build();
}

public class AzuriteContainerBootstrapper : ContainerBootstrapper<AzuriteTestcontainer>
{
    protected override AzuriteTestcontainer BuildContainer(ITestcontainersBuilder<AzuriteTestcontainer> builder)
        => builder.WithAzurite(new AzuriteTestcontainerConfiguration("mcr.microsoft.com/azure-storage/azurite:3.20.1")).Build();
}

public class RabbitMqContainerBootstrapper : ContainerBootstrapper<RabbitMqTestcontainer>
{
    protected override RabbitMqTestcontainer BuildContainer(ITestcontainersBuilder<RabbitMqTestcontainer> builder)
        => builder.WithMessageBroker(new RabbitMqTestcontainerConfiguration { Password = "guest", Username = "guest" }).Build();
}

public class RedisContainerBootstrapper : ContainerBootstrapper<RedisTestcontainer>
{
    protected override RedisTestcontainer BuildContainer(ITestcontainersBuilder<RedisTestcontainer> builder)
        => builder.WithDatabase(new RedisTestcontainerConfiguration()).Build();
}