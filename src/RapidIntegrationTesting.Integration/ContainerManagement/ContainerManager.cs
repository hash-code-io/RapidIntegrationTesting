using RapidIntegrationTesting.Integration.ContainerManagement.Bootstrapper;
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
    public ContainerManager(WebAppFactoryContainerOptions options) => Options = options ?? throw new ArgumentNullException(nameof(options));

    /// <summary>
    ///     Options
    /// </summary>
    protected WebAppFactoryContainerOptions Options { get; }

    /// <summary>
    ///     Bootstrapper used to bootstrap containers
    /// </summary>
    protected IContainerCache ContainerBootstrapper { get; } = new ContainerCache();

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
    public IEnumerable<ContainerStartCallback> StartContainers()
        => Options.Configurations.Select(containerConfigureCallback => containerConfigureCallback(ContainerBootstrapper));
}