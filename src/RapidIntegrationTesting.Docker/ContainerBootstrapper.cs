using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;

namespace RapidIntegrationTesting.Docker;

/// <inheritdoc />
public abstract class ContainerBootstrapper<TContainer> : IContainerBootstrapper<TContainer>
    where TContainer : ITestcontainersContainer
{
    private readonly SemaphoreSlim _semaphore = new(1);
    private TContainer? _container;

    /// <inheritdoc />
    public async Task<TContainer> Bootstrap()
    {
        await _semaphore.WaitAsync().ConfigureAwait(false);
        try
        {
            if (_container != null) return _container;

            var builder = new TestcontainersBuilder<TContainer>();
            ConfigureContainer(builder);
            _container = builder.Build();

            await _container.StartAsync().ConfigureAwait(false);
            return _container;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    /// <inheritdoc />
    public TContainer GetContainer() =>
        _container ??
        throw new InvalidOperationException(
            $"Container uninitialized. Did you forget to call {nameof(Bootstrap)}?");

    /// <inheritdoc />
    public ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        _semaphore.Dispose();
        return _container?.DisposeAsync() ?? new ValueTask();
    }

    /// <summary>
    ///     Configure the container using the given <paramref name="builder" />
    /// </summary>
    /// <param name="builder">The builder to configure the container with</param>
    protected abstract void ConfigureContainer(TestcontainersBuilder<TContainer> builder);
}