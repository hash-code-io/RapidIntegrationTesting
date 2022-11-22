using DotNet.Testcontainers.Containers;
using RapidIntegrationTesting.Integration.Options;

namespace RapidIntegrationTesting.Integration.ContainerManagement;

/// <summary>
///     Base class for Container Management of the <see cref="TestingWebAppFactory{TEntryPoint}" />
/// </summary>
public class ContainerManager : IAsyncDisposable
{
    private readonly List<ITestcontainersContainer> _containers = new();

    /// <summary>
    ///     Initializes a new instance of the type
    /// </summary>
    /// <param name="options"></param>
    public ContainerManager(WebAppFactoryContainerOptions options) => Options = options ?? throw new ArgumentNullException(nameof(options));

    /// <summary>
    ///     Options
    /// </summary>
    protected WebAppFactoryContainerOptions Options { get; }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        await Task.WhenAll(_containers.Select(x => x.DisposeAsync().AsTask()));
        _containers.Clear();
    }

    /// <summary>
    ///     Function to start all containers and provide configuration values based on them
    /// </summary>
    /// <returns>A list of functions to start the containers</returns>
    public async Task<ContainerConfigurations> StartContainers()
    {
        var configs = new ContainerConfigurations();

        RunningContainerInfo[] infos = await Task.WhenAll(Options.Configurations.Select(configure => configure()));
        foreach ((ITestcontainersContainer testcontainersContainer, ContainerConfigurations containerConfigurations) in infos)
        {
            _containers.Add(testcontainersContainer);
            configs.AddRange(containerConfigurations);
        }

        return configs;
    }
}