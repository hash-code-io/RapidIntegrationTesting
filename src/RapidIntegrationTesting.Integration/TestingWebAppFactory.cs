using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using RapidIntegrationTesting.Integration.Auth;
using RapidIntegrationTesting.Integration.Configuration;
using RapidIntegrationTesting.Integration.ContainerManagement;
using RapidIntegrationTesting.Integration.Options;
using RapidIntegrationTesting.SignalR;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;

namespace RapidIntegrationTesting.Integration;

/// <summary>
///     WebAppFactory to use for tests
/// </summary>
/// <typeparam name="TEntryPoint"></typeparam>
public abstract class TestingWebAppFactory<TEntryPoint> : WebApplicationFactory<TEntryPoint>
    where TEntryPoint : class
{
    private readonly ContainerManager _containerManager;
    private readonly WebAppFactoryOptions _options = new();
    private ContainerConfigurations? _containerConfigurations;

    /// <inheritdoc />
    protected TestingWebAppFactory()
    {
        ConfigureOptions(_options);
        _containerManager = new ContainerManager(_options.Container);
    }

    /// <summary>
    ///     Callback to configure options
    /// </summary>
    protected virtual Action<WebAppFactoryOptions> ConfigureOptions => o => { };

    //TODO: implement
    /// <inheritdoc />
    public async Task InitializeAsync()
    {
        IEnumerable<Task<ContainerConfigurations>> containerConfigTasks = _containerManager.StartContainers().Select(x => x());
        ContainerConfigurations[] configsArray = await Task.WhenAll(containerConfigTasks).ConfigureAwait(false);
        _containerConfigurations = new ContainerConfigurations(configsArray.SelectMany(x => x));
    }

    //async Task IAsyncLifetime.DisposeAsync() => await DisposeAsync().ConfigureAwait(false);

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
        if (descriptors == null) throw new ArgumentNullException(nameof(descriptors));

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
    ///     Disposable Implementation
    /// </summary>
    public new void Dispose()
    {
        DisposeAsync().AsTask().GetAwaiter().GetResult();
        base.Dispose();
    }

    /// <summary>
    ///     Runs the given <paramref name="testCode" /> as the given <paramref name="userName" />.
    ///     The <paramref name="userName" /> ist used as the "name" claim. If you wish to
    ///     add any more claims supply them as <paramref name="additionalClaims" />
    /// </summary>
    /// <param name="userName">The user to impersonate</param>
    /// <param name="testCode">The code to run</param>
    /// <param name="additionalClaims">Any additional claims besides the userName to add to the user's claims</param>
    /// <returns></returns>
    [SuppressMessage("Design", "CA1000:Do not declare static members on generic types", Justification = "<Pending>")]
    public static Task RunAsUser(string userName, Func<Task> testCode, IEnumerable<Claim>? additionalClaims = null)
    {
        if (testCode == null) throw new ArgumentNullException(nameof(testCode));
        return TestAuthHandler.RunAsUser(userName, testCode, additionalClaims);
    }

    private IEnumerable<WebAppConfigurationValue> BuildConfigurations()
    {
        foreach (WebAppConfigurationValue val in _containerConfigurations ?? Enumerable.Empty<WebAppConfigurationValue>())
            yield return val;
        foreach (WebAppConfigurationValue val in _options.AdditionalConfigurations)
            yield return val;
    }

    /// <inheritdoc />
    public override async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        await base.DisposeAsync().ConfigureAwait(false);
        await _containerManager.DisposeAsync().ConfigureAwait(false);
    }

    /// <inheritdoc />
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        if (builder == null) throw new ArgumentNullException(nameof(builder));
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
        services.AddSingleton(options.Container);
        services.AddSingleton(options.SignalR);
    }
}