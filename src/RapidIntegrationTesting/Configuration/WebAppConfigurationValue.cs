namespace RapidIntegrationTesting.Configuration;

/// <summary>
///     Configuration Value to be added to the <see cref="TestingWebAppFactory{TEntryPoint}" />'s Configuration
/// </summary>
/// <param name="Key">The key to use</param>
/// <param name="Value">The value to tuse</param>
public record WebAppConfigurationValue(string Key, string Value);