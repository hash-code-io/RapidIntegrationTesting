using Microsoft.AspNetCore.SignalR.Client;

namespace RapidIntegrationTesting.Options;

/// <summary>
///     SignalR Options
/// </summary>
public record WebAppFactorySignalROptions
{
    /// <summary>
    ///     The relative URI to the SignalR Hub to use for the connection. Defaults to "hubs/frontendIntegration"
    /// </summary>
    public string DefaultHubToUse { get; set; } = "hubs/frontendIntegration";

    /// <summary>
    ///     Further configuration of the <see cref="IHubConnectionBuilder" />. Only to be used in advanced scenarios.
    ///     Could be used, for instance, to configure "AddNewtonsoftJsonProtocol".
    ///     <para> Do NOT call "WithUrl", as that part is already correctly configured</para>
    /// </summary>
    public Action<IHubConnectionBuilder> FurtherConfig { get; set; } = o => { };
}