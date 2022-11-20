using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using RabbitMQ.Client;
using System.Data.SqlClient;

namespace RapidIntegrationTesting.Docker.Tests;

public class SqlContainerBootstrapper : ContainerBootstrapper<MsSqlTestcontainer>
{
    protected override ITestcontainersBuilder<MsSqlTestcontainer> ConfigureContainer(TestcontainersBuilder<MsSqlTestcontainer> builder)
        => builder.WithDatabase(new MsSqlTestcontainerConfiguration { Password = "My@Cool$PassWord123" });
}

public class AzuriteContainerBootstrapper : ContainerBootstrapper<AzuriteTestcontainer>
{
    protected override ITestcontainersBuilder<AzuriteTestcontainer> ConfigureContainer(TestcontainersBuilder<AzuriteTestcontainer> builder)
        => builder.WithAzurite(new AzuriteTestcontainerConfiguration("mcr.microsoft.com/azure-storage/azurite:3.20.1") { DebugModeEnabled = true }).WithAutoRemove(false);
}

public class RabbitMqContainerBootstrapper : ContainerBootstrapper<RabbitMqTestcontainer>
{
    protected override ITestcontainersBuilder<RabbitMqTestcontainer> ConfigureContainer(TestcontainersBuilder<RabbitMqTestcontainer> builder)
        => builder.WithMessageBroker(new RabbitMqTestcontainerConfiguration { Password = "guest", Username = "guest" });
}

public class ContainerBootstrapperTest
{
    [Fact]
    public async Task Should_Create_Sql_Container()
    {
        // Arrange
        await using var bootstrapper = new SqlContainerBootstrapper();
        MsSqlTestcontainer container = await bootstrapper.Bootstrap();

        await using var con = new SqlConnection(container.ConnectionString);
        await con.OpenAsync();

        await using var cmd = new SqlCommand("SELECT 1", con);

        // Act

        object? result = await cmd.ExecuteScalarAsync();
        bool isInt = int.TryParse(result?.ToString(), out int resultInt);

        // Assert
        Assert.NotNull(result);
        Assert.True(isInt);
        Assert.Equal(1, resultInt);
    }

    [Fact]
    public async Task Should_Create_Rabbit_Mq()
    {
        // Arrange
        await using var bootstrapper = new RabbitMqContainerBootstrapper();
        RabbitMqTestcontainer container = await bootstrapper.Bootstrap();

        var fac = new ConnectionFactory { Uri = new Uri(container.ConnectionString), AutomaticRecoveryEnabled = true, UseBackgroundThreadsForIO = true };
        using IConnection connection = fac.CreateConnection();

        // Act
        using IModel model = connection.CreateModel();

        // Assert
        Assert.NotNull(model);
    }

    // Test is failing for some reason: https://github.com/testcontainers/testcontainers-dotnet/issues/682
    //[Fact]
    public async Task Should_Create_Azurite_Container()
    {
        // Arrange
        await using var bootstrapper = new AzuriteContainerBootstrapper();
        AzuriteTestcontainer container = await bootstrapper.Bootstrap();

        var blobServiceClient = new BlobServiceClient(container.ConnectionString);
        string containerName = Guid.NewGuid().ToString().ToLowerInvariant();

        BlobContainerClient containerClient = await blobServiceClient.CreateBlobContainerAsync(containerName);

        BlobClient blobClient = containerClient.GetBlobClient("testdata");
        byte[] data = { 1, 2, 3 };

        // Act
        _ = await blobClient.UploadAsync(new MemoryStream(data), true);
        Response<BlobDownloadInfo> downloaded = await blobClient.DownloadAsync();

        // Assert
        Assert.NotNull(downloaded?.Value);
        var ms = new MemoryStream();
        await downloaded.Value.Content.CopyToAsync(ms);
        byte[] downloadedData = ms.ToArray();

        Assert.Equal(data, downloadedData);
    }
}