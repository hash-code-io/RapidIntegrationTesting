using RapidIntegrationTesting.Docker;
using RapidIntegrationTesting.Integration.ContainerManagement;
using RapidIntegrationTesting.Integration.ContainerManagement.Bootstrapper;

namespace RapidIntegrationTesting.Integration.Options;

/// <summary>
///     Callback used to give the user the ability to asynchronously start a container and get Configuration values out of it
/// </summary>
public delegate Task<ContainerConfigurations> ContainerStartCallback();

/// <summary>
///     Callback used to bootstrap the container via the given <see cref="IContainerCache" /> and return a callback to run the bootstapping
/// </summary>
public delegate ContainerStartCallback ContainerConfigureCallback(IContainerCache bootstrapper);

/// <summary>
///     Options to configure the Container Manager
/// </summary>
public record WebAppFactoryContainerOptions
{
    /// <summary>
    ///     Configurations that should contain all containers used for testing
    /// </summary>
    public List<ContainerConfigureCallback> Configurations { get; set; } = new();

    /// <summary>
    ///     Marker types for the assemblies containing implementations of <see cref="IContainerBootstrapper{TContainer}" />
    /// </summary>
    public List<Type> ContainerBootstrapperAssemblyMarkers { get; set; } = new();
}