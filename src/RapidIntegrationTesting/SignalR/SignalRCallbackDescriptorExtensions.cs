using Microsoft.AspNetCore.SignalR.Client;

namespace RapidIntegrationTesting.SignalR;

/// <summary>
///     Extensions
/// </summary>
public static class SignalRCallbackDescriptorExtensions
{
    /// <summary>
    ///     Adds the given <paramref name="descriptors" /> to the <paramref name="hubConnection" />
    /// </summary>
    /// <param name="hubConnection">The connection to add the descriptors to</param>
    /// <param name="descriptors">The descriptors to add</param>
    /// <exception cref="ArgumentNullException"></exception>
    public static void RegisterDescriptors(this HubConnection hubConnection, params ISignalRCallbackDescriptor[] descriptors)
    {
        ArgumentNullException.ThrowIfNull(descriptors);
        foreach (ISignalRCallbackDescriptor signalrCallbackDescriptor in descriptors)
            signalrCallbackDescriptor.Setup(hubConnection);
    }

    /// <summary>
    ///     Builds the <see cref="HubConnection" /> using the given <paramref name="builder" /> and then adds the given
    ///     <paramref name="descriptors" />
    /// </summary>
    /// <param name="builder">The builder to use for building the <see cref="HubConnection" /></param>
    /// <param name="descriptors">The descriptors to add</param>
    /// <returns>The built <see cref="HubConnection" /></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static HubConnection Build(this IHubConnectionBuilder builder, params ISignalRCallbackDescriptor[] descriptors)
    {
        ArgumentNullException.ThrowIfNull(builder);
        HubConnection hubConnection = builder.Build();
        hubConnection.RegisterDescriptors(descriptors);
        return hubConnection;
    }
}