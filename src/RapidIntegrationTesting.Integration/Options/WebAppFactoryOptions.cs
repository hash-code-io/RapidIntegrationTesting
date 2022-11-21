using RapidIntegrationTesting.Integration.Configuration;

namespace RapidIntegrationTesting.Integration.Options;

/// <summary>
///     Options to configure the <see cref="TestingWebAppFactory{TEntryPoint}" />
/// </summary>
public record WebAppFactoryOptions
{
    /// <summary>
    ///     Authorization Options
    /// </summary>
    public WebAppFactoryAuthOptions Auth { get; set; } = new();

    /// <summary>
    ///     Container and Container Manager options
    /// </summary>
    public WebAppFactoryContainerOptions Container { get; set; } = new();

    /// <summary>
    ///     Options for configuring SignalR
    /// </summary>
    public WebAppFactorySignalROptions SignalR { get; set; } = new();

    /// <summary>
    ///     The ASPNETCORE_ENVIRONMENT to use
    /// </summary>
    public string EnvironmentName { get; set; } = "IntegrationTesting";

    /// <summary>
    ///     Additional configurations values to be added to the <see cref="TestingWebAppFactory{TEntryPoint}" />'s Configuration
    /// </summary>
    public List<WebAppConfigurationValue> AdditionalConfigurations { get; set; } = new();
}