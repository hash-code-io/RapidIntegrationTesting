using RapidIntegrationTesting.Utility.Extensions;
using System.Net.Http.Json;
using Testing.Integration.TestWebApi.Data;

namespace RapidIntegrationTesting.Integration.Tests.Tests;

public class GeneralTests : IClassFixture<TestWebAppFactory>
{
    private readonly HttpClient _client;
    private readonly TestWebAppFactory _factory;

    public GeneralTests(TestWebAppFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Should_Work_For_Regular_Calls()
    {
        // Arrange
        Uri getUsersData = UrlHelper.GetUsersDataRoute.AsRelativeUri();

        // Act
        List<UserData>? data = await _client.GetFromJsonAsync<List<UserData>>(getUsersData);

        // Assert
        Assert.NotNull(data);
        Assert.True(data.Count > 0);
    }
}