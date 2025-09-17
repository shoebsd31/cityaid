using CityAid.Application;
using CityAid.Infrastructure;
using CityAid.Api.Middleware;
using CityAid.Api.Endpoints;
using Microsoft.Identity.Web;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Add authentication and authorization with Entra ID
builder.Services.AddAuthentication(Microsoft.AspNetCore.Authentication.OpenIdConnect.OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

// Microsoft Graph can be added later when needed for role queries

// Add authorization with Azure roles
builder.Services.AddAuthorization(options =>
{
    // Require authenticated user for all endpoints by default
    options.FallbackPolicy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();

    // Define role-based policies
    options.AddPolicy("AdminPolicy", policy =>
        policy.RequireRole("CityAid.Admin"));

    options.AddPolicy("CaseManagerPolicy", policy =>
        policy.RequireRole("CityAid.CaseManager", "CityAid.Admin"));

    options.AddPolicy("CitizenPolicy", policy =>
        policy.RequireRole("CityAid.Citizen", "CityAid.CaseManager", "CityAid.Admin"));
});

// Add HTTP context accessor for user service
builder.Services.AddHttpContextAccessor();

// Add controllers
builder.Services.AddControllers();

// Add OpenAPI/Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "CityAid API",
        Version = "v1",
        Description = "API for CityAid MCP Access & Approvals with RBAC enforcement"
    });

    // Add OAuth2 authentication to Swagger for Entra ID
    c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows
        {
            AuthorizationCode = new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri($"https://login.microsoftonline.com/{builder.Configuration["AzureAd:TenantId"]}/oauth2/v2.0/authorize"),
                TokenUrl = new Uri($"https://login.microsoftonline.com/{builder.Configuration["AzureAd:TenantId"]}/oauth2/v2.0/token"),
                Scopes = new Dictionary<string, string>
                {
                    {$"api://{builder.Configuration["AzureAd:ClientId"]}/access_as_user", "Access the API as user"}
                }
            }
        }
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "oauth2"
                }
            },
            new[] { $"api://{builder.Configuration["AzureAd:ClientId"]}/access_as_user" }
        }
    });
});

// Add problem details
builder.Services.AddProblemDetails();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "CityAid API v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();

// Add global exception handling middleware
app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

// Map endpoints
app.MapCaseEndpoints();
app.MapFileEndpoints();

app.Run();

// Make Program class accessible for testing
public partial class Program { }
