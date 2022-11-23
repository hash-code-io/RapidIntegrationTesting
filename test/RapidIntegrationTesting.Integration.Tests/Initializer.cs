using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using RapidIntegrationTesting.ContainerManagement;
using System.Runtime.CompilerServices;
using Testing.Integration.TestWebApi;

namespace RapidIntegrationTesting.Integration.Tests;

public class Initializer
{
    private static Func<Task<RunningContainerInfo>> GetSqlBuilder() =>
        async () =>
        {
            MsSqlTestcontainer container = new TestcontainersBuilder<MsSqlTestcontainer>()
                .WithDatabase(new MsSqlTestcontainerConfiguration { Password = "My@Cool$PassWord123", Database = "Testing" })
                .Build();

            await container.StartAsync();

            var configs = new ContainerConfigurations { new(AppConstants.SqlConnectionStringKey, container.ConnectionString + "TrustServerCertificate=true;") };

            return new RunningContainerInfo(container, configs);
        };

    [ModuleInitializer]
    public static void Initialize()
    {
        Func<Task<RunningContainerInfo>> sqlBuilder = GetSqlBuilder();

        ContainerManager.AddContainers(sqlBuilder);
    }
}