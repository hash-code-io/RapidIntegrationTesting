using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;

namespace RapidIntegrationTesting.ContainerManagement;

internal record ContainerInfo(ITestcontainersContainer Container, ContainerConfigurations Configurations);

/// <summary>
///     Wrapper class for a fully confiugred <see cref="ITestcontainersBuilder{TContainer}" /> and a way to retrieve configurations from the container
/// </summary>
/// <typeparam name="TContainer">The type of container</typeparam>
/// <param name="Builder">The fully configured builder</param>
/// <param name="ConfigurationRetriever">A callback to retrieve configuration</param>
public record ContainerConfigurator<TContainer>(ITestcontainersBuilder<TContainer> Builder, Func<TContainer, ContainerConfigurations> ConfigurationRetriever)
    where TContainer : ITestcontainersContainer
{
    internal Func<Task<ContainerInfo>> ToBuilderCallback() =>
        async () =>
        {
            TContainer container = Builder.Build();
            await container.StartAsync();
            ContainerConfigurations configs = ConfigurationRetriever(container);
            return new ContainerInfo(container, configs);
        };
}