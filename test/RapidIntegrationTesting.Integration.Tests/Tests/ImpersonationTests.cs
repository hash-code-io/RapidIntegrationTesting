using RapidIntegrationTesting.Utility.Extensions;
using System.Net;
using System.Net.Http.Json;
using Testing.Integration.TestWebApi.Data;

namespace RapidIntegrationTesting.Integration.Tests.Tests;

public class ImpersonationTests : IClassFixture<TestWebAppFactory>
{
    private readonly HttpClient _client;
    private readonly TestWebAppFactory _factory;

    public ImpersonationTests(TestWebAppFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Should_Work_With_Impersonation()
    {
        async Task Run(HttpClient client)
        {
            // Arrange
            Uri getAdminData = UrlHelper.GetAdminsDataRoute.AsRelativeUri();

            // Act
            List<AdminData>? data = await client.GetFromJsonAsync<List<AdminData>>(getAdminData);

            // Assert
            Assert.NotNull(data);
            Assert.True(data.Count > 0);
        }

        await _factory.RunAsAdmin(Run);
    }

    [Fact]
    public async Task Should_Fail_For_Secure_Enpoint_Without_Impersonation()
    {
        // Arrange
        Uri getAdminData = UrlHelper.GetAdminsDataRoute.AsRelativeUri();

        // Act
        HttpRequestException exception = await Assert.ThrowsAsync<HttpRequestException>(() => _client.GetFromJsonAsync<List<AdminData>>(getAdminData));

        // Assert
        Assert.Equal(exception.StatusCode, HttpStatusCode.Forbidden);
    }
}