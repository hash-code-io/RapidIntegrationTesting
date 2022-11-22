using Microsoft.AspNetCore.SignalR.Client;
using RapidIntegrationTesting.Integration.SignalR;
using RapidIntegrationTesting.SignalR;
using RapidIntegrationTesting.Utility.Extensions;
using Testing.Integration.TestWebApi.Controllers;
using Testing.Integration.TestWebApi.SignalR;

namespace RapidIntegrationTesting.Integration.Tests.Tests;

[Collection(WebAppFactoryCollectionFixture.Name)]
public class SignalRTests
{
    private readonly HttpClient _client;
    private readonly TestWebAppFactory _factory;

    public SignalRTests(TestWebAppFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Should_Work_For_Signal_R_Callbacks()
    {
        // Arrange
        Uri postNotifySignalRUri = UrlHelper.PostDoSomethingAndNotifySignalR.AsRelativeUri();

        ISignalRCallbackDescriptor descriptor = TestSignalRCallbackDescriptorFactory.Create<ISignalRClient, SignalRModel>(
            (client, arg) => client.SomethingHappened(arg), out Task<SignalRModel> signalReceived, 8_000);

        await using HubConnection connection = _factory.ConfigureSignalRCallbacks(descriptor);
        await connection.StartAsync();

        // Act
        await _client.PostAsync(postNotifySignalRUri, null);
        SignalRModel result = await signalReceived;

        // Assert
        Assert.NotNull(result?.Message);
        Assert.Equal(SignalRController.Message, result.Message);
    }
}