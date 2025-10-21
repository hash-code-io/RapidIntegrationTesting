using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using System.IdentityModel.Tokens.Jwt;
using Testing.Integration.TestWebApi.Data;
using Testing.Integration.TestWebApi.SignalR;

namespace Testing.Integration.TestWebApi;

public static class SetUpExtensions
{
    public static void SetUpAuthentication(this WebApplicationBuilder builder)
    {
        if (builder == null) throw new ArgumentNullException(nameof(builder));
        IServiceCollection services = builder.Services;
        IWebHostEnvironment env = builder.Environment;
        ConfigurationManager configuration = builder.Configuration;

        services.AddAuthentication(opts =>
            {
                opts.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opts.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                opts.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                opts.DefaultForbidScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.Authority = "https://myNonExistingProvider:5000";
                options.SaveToken = true;

                options.Audience = "myAud";

                // Clears Microsoft's annoying JwtClaimType -> XMLClaimType mappings
                options.TokenHandlers.Clear();
                options.TokenHandlers.Add(new JwtSecurityTokenHandler { InboundClaimTypeMap = new Dictionary<string, string>() });

                options.TokenValidationParameters.NameClaimType = "name";
                options.TokenValidationParameters.RoleClaimType = "role";

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        StringValues accessToken = context.Request.Query["access_token"];
                        if (!string.IsNullOrEmpty(accessToken) &&
                            context.HttpContext.Request.Path.StartsWithSegments("/hubs"))
                            context.Token = accessToken;

                        return Task.CompletedTask;
                    }
                };
            });
    }

    public static void SetUpAuthorization(this WebApplicationBuilder builder)
    {
        if (builder == null) throw new ArgumentNullException(nameof(builder));

        builder.Services.AddAuthorization(options =>
        {
            AuthorizationPolicy defaultPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();

            options.FallbackPolicy = defaultPolicy;
            options.DefaultPolicy = defaultPolicy;
            options.AddPolicy(AppConstants.AdminPolicyName, b => b.RequireAuthenticatedUser().RequireRole(AppConstants.AdminRoleName));
        });
    }

    public static void SetUpDbContext(this WebApplicationBuilder builder) =>
        builder.Services.AddDbContext<TestDbContext>(opts =>
        {
            opts.UseSqlServer(
                builder.Configuration[AppConstants.SqlConnectionStringKey], sql =>
                {
                    sql.EnableRetryOnFailure();
                    sql.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery);
                });
        });

    public static void SetUpSignalR(this WebApplicationBuilder builder)
    {
        builder.Services.AddSignalR();
        builder.Services.AddScoped<ISignalRService, SignalRService>();
    }
}