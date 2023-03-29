namespace RapidIntegrationTesting.ContainerManagement;

/// <summary>
///     Base class for Container Management of the <see cref="TestingWebAppFactory{TEntryPoint}" />
/// </summary>
public static class ContainerManager
{
    private static readonly List<RunningContainerInfo> RunningContainers = new();
    private static readonly List<Func<Task<RunningContainerInfo>>> Builders = new();
    private static readonly SemaphoreSlim Semaphore = new(1, 1);

    /// <summary>
    ///     Shuts down all managed containers
    /// </summary>
    /// <returns></returns>
    internal static async Task ShutdownContainers()
    {
        await Semaphore.WaitAsync().ConfigureAwait(false);
        try
        {
            if (!RunningContainers.Any()) return;

            await Task.WhenAll(RunningContainers.Select(x => x.Container.DisposeAsync().AsTask())).ConfigureAwait(false);
            RunningContainers.Clear();
        }
        finally
        {
            Semaphore.Release();
        }
    }

    /// <summary>
    ///     Add containers to be started with the WebAppFactory. The given builders MUST be fully configured
    /// </summary>
    /// <param name="builderCallbacks">Callbacks used to build and start the given containers</param>
    /// <exception cref="InvalidOperationException"></exception>
    public static void AddContainers(params Func<Task<RunningContainerInfo>>[] builderCallbacks)
    {
        if (RunningContainers.Any()) throw new InvalidOperationException("Containers may not be added after starting them");
        Builders.AddRange(builderCallbacks);
    }

    /// <summary>
    ///     Function to start all containers and provide configuration values based on them
    /// </summary>
    /// <returns>A list of functions to start the containers</returns>
    internal static async Task<ContainerConfigurations> StartContainers()
    {
        await Semaphore.WaitAsync().ConfigureAwait(false);
        try
        {
            if (RunningContainers.Any())
                return BuildConfigurations();

            RunningContainerInfo[] infos = await Task.WhenAll(Builders.Select(build => build())).ConfigureAwait(false);
            RunningContainers.AddRange(infos);
            return BuildConfigurations();
        }
        finally
        {
            Semaphore.Release();
        }
    }

    private static ContainerConfigurations BuildConfigurations() => new(RunningContainers.SelectMany(x => x.Configurations));
}