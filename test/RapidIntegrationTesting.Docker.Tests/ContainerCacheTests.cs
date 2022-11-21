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
}