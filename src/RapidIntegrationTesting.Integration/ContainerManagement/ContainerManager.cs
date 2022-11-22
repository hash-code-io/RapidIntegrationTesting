﻿using RapidIntegrationTesting.Integration.ContainerManagement.Bootstrapper;
using RapidIntegrationTesting.Integration.Options;

namespace RapidIntegrationTesting.Integration.ContainerManagement;

/// <summary>
///     Base class for Container Management of the <see cref="TestingWebAppFactory{TEntryPoint}" />
/// </summary>
public class ContainerManager : IAsyncDisposable
{
    /// <summary>
    ///     Initializes a new instance of the type
    /// </summary>
    /// <param name="options"></param>
    public ContainerManager(WebAppFactoryContainerOptions options)
    {
        Options = options ?? throw new ArgumentNullException(nameof(options));
        ContainerBootstrapper = new ContainerCache(Options.ContainerBootstrapperAssemblyMarkers);
    }

    /// <summary>
    ///     Options
    /// </summary>
    protected WebAppFactoryContainerOptions Options { get; }

    /// <summary>
    ///     Bootstrapper used to bootstrap containers
    /// </summary>
    protected IContainerCache ContainerBootstrapper { get; }

    /// <inheritdoc />
    public ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        return ContainerBootstrapper.DisposeAsync();
    }

    /// <summary>
    ///     Function to start all containers and provide configuration values based on them
    /// </summary>
    /// <returns>A list of functions to start the containers</returns>
    public IEnumerable<Task<ContainerConfigurations>> StartContainers()
        => Options.Configurations.Select(containerConfigureCallback => containerConfigureCallback(ContainerBootstrapper));
}