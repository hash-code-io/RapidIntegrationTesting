using DotNet.Testcontainers.Containers;

namespace RapidIntegrationTesting.Integration.ContainerManagement.Bootstrapper;

/// <summary>
///     Bootstrapper for Containers
/// </summary>
public interface IContainerCache : IAsyncDisposable
{
    /// <summary>
    ///     Function to start a container
    /// </summary>
    /// <typeparam name="T">A valid container type</typeparam>
    /// <returns>A task containing the started container</returns>
    Task<T> Bootstrap<T>() where T : ITestcontainersContainer;
}