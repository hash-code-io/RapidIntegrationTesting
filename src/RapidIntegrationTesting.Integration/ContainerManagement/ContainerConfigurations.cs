using RapidIntegrationTesting.Integration.Configuration;

namespace RapidIntegrationTesting.Integration.ContainerManagement;

/// <summary>
///     Holder of configurations for a container
/// </summary>
public sealed class ContainerConfigurations : List<WebAppConfigurationValue>
{
    /// <inheritdoc />
    public ContainerConfigurations()
    {
    }

    /// <inheritdoc />
    public ContainerConfigurations(IEnumerable<WebAppConfigurationValue> collection) : base(collection)
    {
    }

    /// <summary>
    ///     Empty list of configuration
    /// </summary>
    public static ContainerConfigurations Empty { get; } = new();
}