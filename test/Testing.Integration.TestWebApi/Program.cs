using Testing.Integration.TestWebApi;
using Testing.Integration.TestWebApi.Data;
using Testing.Integration.TestWebApi.SignalR;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.SetUpDbContext();
builder.SetUpAuthentication();
builder.SetUpAuthorization();
builder.SetUpSignalR();

WebApplication app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<SignalRHub>("hubs/frontendIntegration");

app.MigrateHostedDbContext<TestDbContext>();

app.Run();