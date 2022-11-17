using DotNet.Testcontainers.Containers;
using System.Collections.Concurrent;

namespace RapidIntegrationTesting.Docker;

/// <summary>
///     Cache for all bootstrappers
/// </summary>
public sealed class BootstrapperCache : IAsyncDisposable
{
    private static readonly ConcurrentDictionary<Type, Type> ContainerToBootstrappers = new();
    private readonly Type[] _assmeblyMarkers;
    private readonly ConcurrentDictionary<Type, object> _bootstrappersForContainerType = new();
    private readonly ConcurrentDictionary<Type, int> _containerRefCounts = new();
    private readonly ConcurrentDictionary<Type, SemaphoreSlim> _semaphoreForType = new();

    /// <summary>
    ///     Construct a new <see cref="BootstrapperCache" /> given an array of marker types pointing to assemblies containing implementations of <see cref="IContainerBootstrapper{T}" />
    /// </summary>
    /// <param name="assmeblyMarkers">assemblies containing implementations of <see cref="IContainerBootstrapper{T}" /></param>
    public BootstrapperCache(params Type[] assmeblyMarkers) => _assmeblyMarkers = assmeblyMarkers;

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        foreach (SemaphoreSlim semaphore in _semaphoreForType.Values) semaphore.Dispose();

        foreach (object bootstrapper in _bootstrappersForContainerType.Values) await ((IAsyncDisposable)bootstrapper).DisposeAsync().ConfigureAwait(false);
    }

    /// <summary>
    ///     Gets a cached container
    /// </summary>
    /// <typeparam name="TContainer">The type of container to get</typeparam>
    /// <returns>The cached contaienr instance</returns>
    public async Task<TContainer> GetCachedContainer<TContainer>() where TContainer : ITestcontainersContainer
    {
        Type containerType = typeof(TContainer);
        SemaphoreSlim semaphore = _semaphoreForType.GetOrAdd(containerType, _ => new SemaphoreSlim(1));
        await semaphore.WaitAsync().ConfigureAwait(false);

        try
        {
            Type bootstrapperType = GetBootstrapperForContainer(containerType);

            var bootstrapperInstance = (IContainerBootstrapper<TContainer>)_bootstrappersForContainerType.GetOrAdd(containerType,
                t => Activator.CreateInstance(bootstrapperType)!);
            _containerRefCounts.AddOrUpdate(containerType, _ => 1, (_, old) => old + 1);

            return await bootstrapperInstance.Bootstrap().ConfigureAwait(false);
        }
        finally
        {
            semaphore.Release();
        }
    }

    /// <summary>
    ///     Releases a cached container to free resources
    /// </summary>
    /// <typeparam name="TContainer">The type of container to release</typeparam>
    /// <returns>An awaitable task</returns>
    public async Task ReleaseCachedContainer<TContainer>()
    {
        Type containerType = typeof(TContainer);
        if (!_semaphoreForType.TryGetValue(containerType, out SemaphoreSlim? semaphore))
            return;

        await semaphore.WaitAsync().ConfigureAwait(false);
        try
        {
            int refCount = _containerRefCounts.AddOrUpdate(containerType, _ => 0, (type, old) => old - 1);
            if (refCount == 0 && _bootstrappersForContainerType.TryRemove(containerType, out object? containerBootstrapper))
                await ((IAsyncDisposable)containerBootstrapper).DisposeAsync().ConfigureAwait(false);
        }
        finally
        {
            semaphore.Release();
        }
    }

    private Type GetBootstrapperForContainer(Type containerType) =>
        ContainerToBootstrappers.GetOrAdd(containerType, t =>
        {
            Type bootstrapperType = typeof(IContainerBootstrapper<>).MakeGenericType(t);
            var bootstrapperlist = _assmeblyMarkers
                .SelectMany(x => x.Assembly.GetTypes())
                .Where(tp => bootstrapperType.IsAssignableFrom(tp))
                .ToList();
            if (bootstrapperlist.Count != 1)
                throw new InvalidOperationException(
                    $"The given type {t} could not be mapped to a single bootstrapper");
            return bootstrapperlist[0];
        });
}