using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using RapidIntegrationTesting.ContainerManagement;
using System.Runtime.CompilerServices;
using Testing.Integration.TestWebApi;

namespace RapidIntegrationTesting.Integration.Tests;

public class Initializer
{
    private static ContainerConfigurator<MsSqlTestcontainer> BuildSqlConfigurator()
    {
        ITestcontainersBuilder<MsSqlTestcontainer> sqlBuilder = new TestcontainersBuilder<MsSqlTestcontainer>()
            .WithDatabase(new MsSqlTestcontainerConfiguration { Password = "My@Cool$PassWord123", Database = "Testing" });

        return new ContainerConfigurator<MsSqlTestcontainer>(sqlBuilder, ConfigRetriever);

        static ContainerConfigurations ConfigRetriever(MsSqlTestcontainer container)
        {
            return new ContainerConfigurations { new(AppConstants.SqlConnectionStringKey, container.ConnectionString + "TrustServerCertificate=true;") };
        }
    }

    [ModuleInitializer]
    public static void Initialize()
    {
        ContainerConfigurator<MsSqlTestcontainer> sqlConfigurator = BuildSqlConfigurator();

        ContainerManager.AddContainers(sqlConfigurator);
    }
}