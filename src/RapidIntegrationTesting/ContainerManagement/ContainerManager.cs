namespace RapidIntegrationTesting.ContainerManagement;

/// <summary>
///     Base class for Container Management of the <see cref="TestingWebAppFactory{TEntryPoint}" />
/// </summary>
public static class ContainerManager
{
    private const string ContainerModificationError = $"Containers may not be added after starting them. Call {nameof(ShutdownContainers)} first if this is what you intended to do";
    private static readonly List<RunningContainerInfo> RunningContainers = [];
    private static readonly List<Func<Task<RunningContainerInfo>>> Builders = [];
    private static readonly SemaphoreSlim Semaphore = new(1, 1);

    /// <summary>
    ///     Whether or not the Manager currently has any running containers
    /// </summary>
    public static bool HasRunningContainers => RunningContainers.Count != 0;

    /// <summary>
    ///     Configurations of all currently running containers
    /// </summary>
    public static ContainerConfigurations RunningContainerConfigurations => new(RunningContainers.SelectMany(x => x.Configurations));

    /// <summary>
    ///     Shuts down all managed containers
    /// </summary>
    /// <returns></returns>
    public static async Task ShutdownContainers()
    {
        await Semaphore.WaitAsync().ConfigureAwait(false);
        try
        {
            if (!HasRunningContainers) return;

            await Task.WhenAll(RunningContainers.Select(x => x.Container.DisposeAsync().AsTask())).ConfigureAwait(false);
            RunningContainers.Clear();
        }
        finally
        {
            Semaphore.Release();
        }
    }

    /// <summary>
    ///     Add containers to be started. The given builders MUST be fully configured
    /// </summary>
    /// <param name="builderCallbacks">Callbacks used to build and start the given containers</param>
    /// <exception cref="InvalidOperationException"></exception>
    public static void AddContainers(params Func<Task<RunningContainerInfo>>[] builderCallbacks)
    {
        if (HasRunningContainers) throw new InvalidOperationException(ContainerModificationError);
        Builders.AddRange(builderCallbacks);
    }

    /// <summary>
    ///     Clears all container builders from the internal state. This does NOT stop the containers. If any containers are running they need to be stopped
    ///     by calling <see cref="ShutdownContainers" /> first.
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public static void ClearContainers()
    {
        if (HasRunningContainers) throw new InvalidOperationException(ContainerModificationError);
        Builders.Clear();
    }


    /// <summary>
    ///     Function to start all containers and provide configuration values based on them.
    ///     Function
    /// </summary>
    /// <returns>A list of functions to start the containers</returns>
    public static async Task<ContainerConfigurations> StartContainers()
    {
        await Semaphore.WaitAsync().ConfigureAwait(false);
        try
        {
            if (HasRunningContainers)
                return RunningContainerConfigurations;

            RunningContainerInfo[] infos = await Task.WhenAll(Builders.Select(build => build())).ConfigureAwait(false);
            RunningContainers.AddRange(infos);
            return RunningContainerConfigurations;
        }
        finally
        {
            Semaphore.Release();
        }
    }
}