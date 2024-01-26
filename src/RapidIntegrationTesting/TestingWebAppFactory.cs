using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using RapidIntegrationTesting.Auth;
using RapidIntegrationTesting.Configuration;
using RapidIntegrationTesting.ContainerManagement;
using RapidIntegrationTesting.Options;
using RapidIntegrationTesting.SignalR;

namespace RapidIntegrationTesting;

/// <summary>
///     WebAppFactory to use for tests
/// </summary>
/// <typeparam name="TEntryPoint"></typeparam>
public abstract class TestingWebAppFactory<TEntryPoint> : WebApplicationFactory<TEntryPoint>
    where TEntryPoint : class
{
    private readonly WebAppFactoryOptions _options = new();
    private ContainerConfigurations? _containerConfigurations;

    /// <inheritdoc />
    protected TestingWebAppFactory() => ConfigureOptions(_options);

    /// <summary>
    ///     Callback to configure options
    /// </summary>
    protected virtual Action<WebAppFactoryOptions> ConfigureOptions => o => { };

    /// <summary>
    ///     Initializes the Factory. Needs to be called for every instance of this factory BEFORE using it (i.e. via XUnit's IAsyncLifetime)
    /// </summary>
    /// <returns></returns>
    public async Task Initialize() => _containerConfigurations = await ContainerManager.StartContainers().ConfigureAwait(false);

    /// <summary>
    ///     Method to build a <see cref="HubConnection" /> that correctly connects to the server using the DefaultHubToUse configured in the options
    ///     and listents to the given <paramref name="descriptors" />
    /// </summary>
    /// <param name="descriptors">The descriptors to add to the connection</param>
    /// <returns>The configured connection</returns>
    public HubConnection ConfigureSignalRCallbacks(params ISignalRCallbackDescriptor[] descriptors)
        => ConfigureSignalRCallbacks(_options.SignalR.DefaultHubToUse, descriptors);

    /// <summary>
    ///     Method to build a <see cref="HubConnection" /> that correctly connects to the server and listents to the given <paramref name="descriptors" />
    /// </summary>
    /// <param name="relativeHubPath">The relative path to the hub to connect to, i.e. "hubs/frontendIntegration" </param>
    /// <param name="descriptors">The descriptors to add to the connection</param>
    /// <returns>The configured connection</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public HubConnection ConfigureSignalRCallbacks(string relativeHubPath, params ISignalRCallbackDescriptor[] descriptors)
    {
        ArgumentNullException.ThrowIfNull(descriptors);

        var signalRHubUri = new Uri(Server.BaseAddress, relativeHubPath);
        IHubConnectionBuilder builder = new HubConnectionBuilder()
            .WithUrl(signalRHubUri, o =>
            {
                o.HttpMessageHandlerFactory = _ => Server.CreateHandler();
            });
        _options.SignalR.FurtherConfig(builder);
        HubConnection connection = builder.Build();

        foreach (ISignalRCallbackDescriptor descriptor in descriptors)
            descriptor.Setup(connection);

        return connection;
    }

    /// <summary>
    ///     Runs the given <paramref name="testCode" /> as the given <paramref name="userName" />.
    ///     You must use the provided <see cref="HttpClient" /> for impersonation to work.
    ///     Claims to add to the user will be looked up via <see cref="WebAppFactoryAuthOptions.UserClaimsMapping" />
    /// </summary>
    /// <param name="userName">The user to impersonate</param>
    /// <param name="testCode">The code to run. Use the provided <see cref="HttpClient" /> to make calls to the server</param>
    /// <returns></returns>
    public async Task RunAsUser(string userName, Func<HttpClient, Task> testCode)
    {
        ArgumentNullException.ThrowIfNull(testCode);

        HttpClient client = CreateClient();
        client.DefaultRequestHeaders.Add(AuthConstants.TestUserNameHeaderName, userName);

        await testCode(client).ConfigureAwait(false);
    }

    private IEnumerable<WebAppConfigurationValue> BuildConfigurations()
    {
        foreach (WebAppConfigurationValue val in _containerConfigurations ?? Enumerable.Empty<WebAppConfigurationValue>())
            yield return val;
        foreach (WebAppConfigurationValue val in _options.AdditionalConfigurations)
            yield return val;
    }

    /// <inheritdoc />
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.UseEnvironment(_options.EnvironmentName);

        foreach ((string key, string value) in BuildConfigurations())
            builder.UseSetting(key, value);

        builder.ConfigureServices(services =>
        {
            SetUpRequiredTestServices(services, _options);
            SetUpTestAuth(services, _options.Auth);
        });
    }

    private static void SetUpTestAuth(IServiceCollection services, WebAppFactoryAuthOptions options)
    {
        if (!options.UseTestAuth) return;

        string schemeName = options.SchemeName;

        services.AddAuthentication(opts =>
        {
            opts.DefaultScheme = schemeName;
            opts.DefaultAuthenticateScheme = schemeName;
            opts.DefaultChallengeScheme = schemeName;
            opts.DefaultForbidScheme = schemeName;
            opts.DefaultSignInScheme = schemeName;
            opts.DefaultSignOutScheme = schemeName;
        }).AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(schemeName, options =>
        {
        });
    }

    private static void SetUpRequiredTestServices(IServiceCollection services, WebAppFactoryOptions options)
    {
        services.AddSingleton(options);
        services.AddSingleton(options.Auth);
        services.AddSingleton(options.SignalR);
    }
}