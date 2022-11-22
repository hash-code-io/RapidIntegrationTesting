using DotNet.Testcontainers.Containers;
using RapidIntegrationTesting.Docker.Tests.Setup;

namespace RapidIntegrationTesting.Docker.Tests;

public class ContainerCacheTests
{
    [Fact]
    public async Task Should_Cache_Single_Container()
    {
        // Arrange
        var sut = new BootstrapperCache(new[] { typeof(RedisContainerBootstrapper) });
        try
        {
            // Act
            Task<RedisTestcontainer> res1Task = sut.GetCachedContainer<RedisTestcontainer>();
            Task<RedisTestcontainer> res2Task = sut.GetCachedContainer<RedisTestcontainer>();
            Task<RedisTestcontainer> res3Task = sut.GetCachedContainer<RedisTestcontainer>();
            Task<RedisTestcontainer> res4Task = sut.GetCachedContainer<RedisTestcontainer>();

            RedisTestcontainer[] result = await Task.WhenAll(res1Task, res2Task, res3Task, res4Task);

            // Assert
            Assert.Equal(result[0], result[1]);
            Assert.Equal(result[1], result[2]);
            Assert.Equal(result[2], result[3]);
        }
        finally
        {
            await sut.DisposeAsync();
        }
    }

    [Fact]
    public async Task Should_Cache_Multiple_Container()
    {
        // Arrange
        var sut = new BootstrapperCache(new[] { typeof(RedisContainerBootstrapper) });
        try
        {
            // Act
            Task<RabbitMqTestcontainer> res1Task = sut.GetCachedContainer<RabbitMqTestcontainer>();
            Task<RabbitMqTestcontainer> res2Task = sut.GetCachedContainer<RabbitMqTestcontainer>();
            Task<RedisTestcontainer> res3Task = sut.GetCachedContainer<RedisTestcontainer>();
            Task<RedisTestcontainer> res4Task = sut.GetCachedContainer<RedisTestcontainer>();

            RabbitMqTestcontainer[] rabbitResult = await Task.WhenAll(res1Task, res2Task);
            RedisTestcontainer[] azuriteResult = await Task.WhenAll(res3Task, res4Task);

            // Assert
            Assert.Equal(rabbitResult[0], rabbitResult[1]);
            Assert.Equal(azuriteResult[0], azuriteResult[1]);
        }
        finally
        {
            await sut.DisposeAsync();
        }
    }

    [Fact]
    public async Task Should_Not_Dispose_For_Ref_Count_Bigger_One()
    {
        // Arrange
        var sut = new BootstrapperCache(new[] { typeof(RedisContainerBootstrapper) });
        try
        {
            // Act
            Task<RabbitMqTestcontainer> res1Task = sut.GetCachedContainer<RabbitMqTestcontainer>();
            Task<RabbitMqTestcontainer> res2Task = sut.GetCachedContainer<RabbitMqTestcontainer>();

            RabbitMqTestcontainer[] rabbitResult = await Task.WhenAll(res1Task, res2Task);

            await sut.ReleaseCachedContainer<RabbitMqTestcontainer>();

            RabbitMqTestcontainer additionalRabbitResult = await sut.GetCachedContainer<RabbitMqTestcontainer>();

            // Assert
            Assert.Equal(rabbitResult[0], rabbitResult[1]);
            Assert.Equal(rabbitResult[1], additionalRabbitResult);
        }
        finally
        {
            await sut.DisposeAsync();
        }
    }

    [Fact]
    public async Task Should_Dispose_At_Ref_Count_Zero()
    {
        // Arrange
        var sut = new BootstrapperCache(new[] { typeof(RedisContainerBootstrapper) });
        try
        {
            // Act
            Task<RabbitMqTestcontainer> res1Task = sut.GetCachedContainer<RabbitMqTestcontainer>();
            Task<RabbitMqTestcontainer> res2Task = sut.GetCachedContainer<RabbitMqTestcontainer>();
            Task<RabbitMqTestcontainer> res3Task = sut.GetCachedContainer<RabbitMqTestcontainer>();
            Task<RabbitMqTestcontainer> res4Task = sut.GetCachedContainer<RabbitMqTestcontainer>();
            Task<RabbitMqTestcontainer> res5Task = sut.GetCachedContainer<RabbitMqTestcontainer>();
            Task<RabbitMqTestcontainer> res6Task = sut.GetCachedContainer<RabbitMqTestcontainer>();

            RabbitMqTestcontainer[] rabbitResult = await Task.WhenAll(res1Task, res2Task, res3Task, res4Task, res5Task, res6Task);

            Task releaseTask1 = sut.ReleaseCachedContainer<RabbitMqTestcontainer>();
            Task releaseTask2 = sut.ReleaseCachedContainer<RabbitMqTestcontainer>();
            Task releaseTask3 = sut.ReleaseCachedContainer<RabbitMqTestcontainer>();
            Task releaseTask4 = sut.ReleaseCachedContainer<RabbitMqTestcontainer>();
            Task releaseTask5 = sut.ReleaseCachedContainer<RabbitMqTestcontainer>();
            Task releaseTask6 = sut.ReleaseCachedContainer<RabbitMqTestcontainer>();

            await Task.WhenAll(releaseTask1, releaseTask2, releaseTask3, releaseTask4, releaseTask5, releaseTask6);

            RabbitMqTestcontainer newRabbit = await sut.GetCachedContainer<RabbitMqTestcontainer>();

            // Assert
            Assert.Equal(rabbitResult[0], rabbitResult[1]);
            Assert.Equal(rabbitResult[1], rabbitResult[2]);
            Assert.Equal(rabbitResult[2], rabbitResult[3]);
            Assert.Equal(rabbitResult[3], rabbitResult[4]);
            Assert.Equal(rabbitResult[4], rabbitResult[5]);
            Assert.NotEqual(newRabbit, rabbitResult[0]);
        }
        finally
        {
            await sut.DisposeAsync();
        }
    }

    [Fact]
    public async Task Should_Dispose_Correctly_For_Multiple_Cached_Container_Types()
    {
        // Arrange
        var sut = new BootstrapperCache(new[] { typeof(RedisContainerBootstrapper) });
        try
        {
            // Act
            Task<RedisTestcontainer> azurite1Task = sut.GetCachedContainer<RedisTestcontainer>();
            Task<RedisTestcontainer> azurite2Task = sut.GetCachedContainer<RedisTestcontainer>();
            Task<RabbitMqTestcontainer> rabbitTask = sut.GetCachedContainer<RabbitMqTestcontainer>();

            RedisTestcontainer initialAzurite1 = await azurite1Task;
            RedisTestcontainer initialAzurite2 = await azurite2Task;
            RabbitMqTestcontainer initialRabbit = await rabbitTask;

            await sut.ReleaseCachedContainer<RabbitMqTestcontainer>();
            await sut.ReleaseCachedContainer<RedisTestcontainer>();

            RabbitMqTestcontainer newRabbit = await sut.GetCachedContainer<RabbitMqTestcontainer>();
            RedisTestcontainer newAzurite = await sut.GetCachedContainer<RedisTestcontainer>();

            // Assert
            Assert.NotEqual(initialRabbit, newRabbit);
            Assert.Equal(initialAzurite1, initialAzurite2);
            Assert.Equal(initialAzurite1, newAzurite);
        }
        finally
        {
            await sut.DisposeAsync();
        }
    }
}