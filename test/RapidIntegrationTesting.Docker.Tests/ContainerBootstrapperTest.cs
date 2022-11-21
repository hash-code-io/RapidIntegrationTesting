using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using DotNet.Testcontainers.Containers;
using RabbitMQ.Client;
using RapidIntegrationTesting.Docker.Tests.Setup;
using StackExchange.Redis;
using System.Data.SqlClient;

namespace RapidIntegrationTesting.Docker.Tests;

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

    [Fact]
    public async Task Should_Create_Redis()
    {
        // Arrange
        await using var bootstrapper = new RedisContainerBootstrapper();
        RedisTestcontainer container = await bootstrapper.Bootstrap();

        ConnectionMultiplexer muxer = await ConnectionMultiplexer.ConnectAsync(container.ConnectionString);
        IDatabase conn = muxer.GetDatabase();

        // Act
        conn.StringSet("foo", "bar");
        await muxer.DisposeAsync();

        muxer = await ConnectionMultiplexer.ConnectAsync(container.ConnectionString);
        conn = muxer.GetDatabase();

        RedisValue result = conn.StringGet("foo");

        // Assert
        Assert.Equal("bar", result);
    }

    // Test is failing for some reason: https://github.com/testcontainers/testcontainers-dotnet/issues/682
    //[Fact]
    private async Task Should_Create_Azurite_Container()
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