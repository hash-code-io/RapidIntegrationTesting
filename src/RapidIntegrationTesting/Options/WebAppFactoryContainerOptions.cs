using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using RapidIntegrationTesting.ContainerManagement;

namespace RapidIntegrationTesting.Options;

/// <summary>
///     Wrapper class for containers and their configurations
/// </summary>
/// <param name="Container">Already running container</param>
/// <param name="Configuration">Configuration from the container</param>
public record RunningContainerInfo(ITestcontainersContainer Container, ContainerConfigurations Configuration);

/// <summary>
///     Callback used to bootstrap and start the container. Use <see cref="TestcontainersBuilder{TDockerContainer}" /> to bootstrap the container, then start it and return it along with its configuration
/// </summary>
public delegate Task<RunningContainerInfo> ContainerStartCallback();

/// <summary>
///     Options to configure the Container Manager
/// </summary>
public record WebAppFactoryContainerOptions
{
    /// <summary>
    ///     Configurations that should contain all containers used for testing
    /// </summary>
    public List<ContainerStartCallback> Configurations { get; set; } = new();
}