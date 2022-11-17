using DotNet.Testcontainers.Containers;

namespace RapidIntegrationTesting.Docker;

/// <summary>
///     Base Interface for all Container Bootstrappers
/// </summary>
/// <typeparam name="TContainer">The type of container to bootstrap</typeparam>
public interface IContainerBootstrapper<TContainer> : IAsyncDisposable
    where TContainer : ITestcontainersContainer
{
    /// <summary>
    ///     Starts the bootstrapping process
    /// </summary>
    /// <returns>An awaitable task containing the bootstrapped container</returns>
    Task<TContainer> Bootstrap();

    /// <summary>
    ///     The bootstrapped container once it is initialized. Otherwise throws an exception
    /// </summary>
    /// <returns>The container</returns>
    TContainer GetContainer();
}