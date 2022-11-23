using DotNet.Testcontainers.Containers;

namespace RapidIntegrationTesting.ContainerManagement;

/// <summary>
///     Base class for Container Management of the <see cref="TestingWebAppFactory{TEntryPoint}" />
/// </summary>
public static class ContainerManager
{
    private static readonly List<ContainerInfo> ContainerInfos = new();
    private static readonly List<Func<Task<ContainerInfo>>> Builders = new();
    private static readonly SemaphoreSlim Semaphore = new(1, 1);

    /// <summary>
    ///     Shuts down all managed containers
    /// </summary>
    /// <returns></returns>
    internal static async Task ShutdownContainers()
    {
        try
        {
            await Semaphore.WaitAsync();
            if (!ContainerInfos.Any()) return;

            await Task.WhenAll(ContainerInfos.Select(x => x.Container.DisposeAsync().AsTask()));
            ContainerInfos.Clear();
        }
        finally
        {
            Semaphore.Release();
        }

        await Task.WhenAll(ContainerInfos.Select(x => x.Container.DisposeAsync().AsTask()));
        ContainerInfos.Clear();
    }

    /// <summary>
    ///     Add containers to be started with the WebAppFactory. The given builders MUST be fully configured
    /// </summary>
    /// <typeparam name="TContainer">The type of container</typeparam>
    /// <param name="configurators"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public static void AddContainers<TContainer>(params ContainerConfigurator<TContainer>[] configurators) where TContainer : ITestcontainersContainer
    {
        if (ContainerInfos.Any()) throw new InvalidOperationException("Containers may not be added after starting them");

        IEnumerable<Func<Task<ContainerInfo>>> builders = configurators.Select(x => x.ToBuilderCallback());
        Builders.AddRange(builders);
    }

    /// <summary>
    ///     Function to start all containers and provide configuration values based on them
    /// </summary>
    /// <returns>A list of functions to start the containers</returns>
    internal static async Task<ContainerConfigurations> StartContainers()
    {
        try
        {
            await Semaphore.WaitAsync();

            if (ContainerInfos.Any())
                return BuildConfigurations();

            ContainerInfo[] infos = await Task.WhenAll(Builders.Select(build => build()));
            ContainerInfos.AddRange(infos);
            return BuildConfigurations();
        }
        finally
        {
            Semaphore.Release();
        }
    }

    private static ContainerConfigurations BuildConfigurations() => new(ContainerInfos.SelectMany(x => x.Configurations));
}