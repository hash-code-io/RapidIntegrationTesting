using Microsoft.Data.SqlClient;
using RapidIntegrationTesting.ContainerManagement;
using System.Runtime.CompilerServices;
using Testcontainers.MsSql;
using Testing.Integration.TestWebApi;

namespace RapidIntegrationTesting.Integration.Tests;

//TODO: Remove when https://github.com/testcontainers/testcontainers-dotnet/pull/855 is merged.
internal static class MsSqlContainerExtensions
{
    public static string GetConnectionString(this MsSqlContainer container, string database)
    {
        var sqlBuilder = new SqlConnectionStringBuilder(container.GetConnectionString());
        Dictionary<string, string> dictionary = new()
        {
            ["Server"] = sqlBuilder.DataSource,
            ["Database"] = database,
            ["User Id"] = sqlBuilder.UserID,
            ["Password"] = sqlBuilder.Password,
            ["TrustServerCertificate"] = sqlBuilder.TrustServerCertificate.ToString()
        };

        return string.Join(";", dictionary.Select(kvp => string.Join("=", kvp.Key, kvp.Value)));
    }
}

public class Initializer
{
    private static Func<Task<RunningContainerInfo>> GetSqlBuilder() =>
        async () =>
        {
            MsSqlContainer container = new MsSqlBuilder()
                .WithPassword("My@Cool$PassWord123")
                .Build();

            await container.StartAsync();
            var configs = new ContainerConfigurations { new(AppConstants.SqlConnectionStringKey, container.GetConnectionString("Testing")) };

            return new RunningContainerInfo(container, configs);
        };

    [ModuleInitializer]
    public static void Initialize()
    {
        Func<Task<RunningContainerInfo>> sqlBuilder = GetSqlBuilder();

        ContainerManager.AddContainers(sqlBuilder);
    }
}