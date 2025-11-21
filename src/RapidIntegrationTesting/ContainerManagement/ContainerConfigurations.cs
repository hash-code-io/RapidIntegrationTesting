using RapidIntegrationTesting.Configuration;
using System.Collections;

namespace RapidIntegrationTesting.ContainerManagement;

/// <summary>
///     Holder of configurations for a container
/// </summary>
public sealed class ContainerConfigurations(IEnumerable<WebAppConfigurationValue> collection) : IReadOnlyList<WebAppConfigurationValue>
{
    private readonly List<WebAppConfigurationValue> _inner = [.. collection];

    /// <summary>
    ///     Initializes a new <see cref="ContainerConfigurations" />
    /// </summary>
    /// <param name="collection">The items to initialize <see cref="ContainerConfigurations" /> with</param>
    public ContainerConfigurations(params WebAppConfigurationValue[] collection) : this((IEnumerable<WebAppConfigurationValue>)collection) { }

    /// <summary>
    ///     Empty list of configuration
    /// </summary>
    public static ContainerConfigurations Empty { get; } = new([]);

    /// <inheritdoc />
    public IEnumerator<WebAppConfigurationValue> GetEnumerator() => _inner.GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_inner).GetEnumerator();

    /// <inheritdoc />
    public int Count => _inner.Count;

    /// <inheritdoc />
    public WebAppConfigurationValue this[int index] => _inner[index];
}