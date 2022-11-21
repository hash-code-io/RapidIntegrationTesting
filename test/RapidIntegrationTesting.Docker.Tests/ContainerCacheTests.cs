using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RapidIntegrationTesting.Docker.Tests;
public class ContainerCacheTests
{
    [Fact]
    public async Task Should_Cache_Single_Container()
    {
        // Arrange
        var sut = new BootstrapperCache();
        try
        {
            // Act
            Task<CatRabbitMqTestcontainer> res1Task = sut.GetCachedContainer<CatRabbitMqTestcontainer>();
            Task<CatRabbitMqTestcontainer> res2Task = sut.GetCachedContainer<CatRabbitMqTestcontainer>();
            Task<CatRabbitMqTestcontainer> res3Task = sut.GetCachedContainer<CatRabbitMqTestcontainer>();
            Task<CatRabbitMqTestcontainer> res4Task = sut.GetCachedContainer<CatRabbitMqTestcontainer>();

            CatRabbitMqTestcontainer[] result = await Task.WhenAll(res1Task, res2Task, res3Task, res4Task);

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
