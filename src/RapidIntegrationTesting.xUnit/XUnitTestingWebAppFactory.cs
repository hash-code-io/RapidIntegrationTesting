using Xunit;

namespace RapidIntegrationTesting.xUnit;

/// <summary>
///     XUnit Integration for <see cref="TestingWebAppFactory{TEntryPoint}" />
/// </summary>
/// <typeparam name="TEntryPoint"></typeparam>
public abstract class XUnitTestingWebAppFactory<TEntryPoint> : TestingWebAppFactory<TEntryPoint>, IAsyncLifetime
    where TEntryPoint : class
{
    /// <inheritdoc />
    public Task InitializeAsync() => Initialize();

    Task IAsyncLifetime.DisposeAsync() => DisposeAsync().AsTask();
}