using DotNet.Testcontainers.Containers;

namespace RapidIntegrationTesting.ContainerManagement;

/// <summary>
///     Wrapper class for a started container along with it configuration
/// </summary>
/// <param name="Container">The started container</param>
/// <param name="Configurations">The container's configuration</param>
public record RunningContainerInfo(ITestcontainersContainer Container, ContainerConfigurations Configurations);