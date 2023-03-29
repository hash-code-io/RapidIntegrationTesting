using System.Diagnostics.CodeAnalysis;

namespace RapidIntegrationTesting.Utility.Extensions;

/// <summary>
///     Extensions
/// </summary>
public static class IntegrationTestExtensions
{
    /// <summary>
    ///     Gets the relative path to the controller (endpoints). Path to reach the controller is expected to be in "api/v{apiVersion}/{ControllerName}" format
    /// </summary>
    /// <param name="controllerName">Simple Name of the controller, i.e. "WeatherForecastController"</param>
    /// <param name="apiVersion">api version to use</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static string GetRelativePathToController(this string controllerName, string apiVersion = "1.0")
    {
        if (controllerName == null) throw new ArgumentNullException(nameof(controllerName));
        return $"api/v{apiVersion}/{controllerName.Replace("Controller", "", StringComparison.OrdinalIgnoreCase)}";
    }

    /// <summary>
    ///     Returns the relative uri to the given url
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    [SuppressMessage("Design", "CA1054:URI-like parameters should not be strings", Justification = "Pointless if it was URI")]
    public static Uri AsRelativeUri(this string url)
    {
        if (url == null) throw new ArgumentNullException(nameof(url));
        return new Uri(url, UriKind.Relative);
    }

    /// <summary>
    ///     Returns the relative uri to the given url
    /// </summary>
    /// <param name="url"></param>
    /// <param name="queryParams"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    [SuppressMessage("Design", "CA1054:URI-like parameters should not be strings", Justification = "Pointless if it was URI")]
    public static Uri AsRelativeUri(this string url, Dictionary<string, string> queryParams)
    {
        if (url == null) throw new ArgumentNullException(nameof(url));
        if (queryParams == null) throw new ArgumentNullException(nameof(queryParams));
        string paramString = queryParams.Count == 0
            ? string.Empty
            : "?" + string.Join('&', queryParams.Select(x => $"{x.Key}={x.Value}"));


        return new Uri(url + paramString, UriKind.Relative);
    }

    /// <summary>
    ///     Returns the relative uri to the given url
    /// </summary>
    /// <param name="url"></param>
    /// <param name="queryParams"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    [SuppressMessage("Design", "CA1054:URI-like parameters should not be strings", Justification = "Pointless if it was URI")]
    public static Uri AsRelativeUri(this string url, params (string key, string value)[] queryParams)
    {
        if (url == null) throw new ArgumentNullException(nameof(url));
        if (queryParams == null) throw new ArgumentNullException(nameof(queryParams));
        string paramString = queryParams.Length == 0
            ? string.Empty
            : "?" + string.Join('&', queryParams.Select(x => $"{x.key}={x.value}"));


        return new Uri(url + paramString, UriKind.Relative);
    }
}