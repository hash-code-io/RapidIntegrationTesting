using DotNet.Testcontainers.Containers;
using RapidIntegrationTesting.Docker;

namespace RapidIntegrationTesting.Integration.ContainerManagement.Bootstrapper;

internal class ContainerCache : IContainerCache
{
    private readonly BootstrapperCache _cache = new();

    public Task<T> Bootstrap<T>() where T : ITestcontainersContainer => _cache.GetCachedContainer<T>();

    public ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        return _cache.DisposeAsync();
    }
}