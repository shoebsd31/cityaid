using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.OpenApi.Models;
using CityAid.Api.Infrastructure;
using CityAid.Api.Persistence;

var builder = WebApplication.CreateBuilder(args);

// --- AuthN/Z (Microsoft Entra ID) ---
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CaseRead", p => p.RequireClaim("scp", "case:read").RequireAuthenticatedUser());
    options.AddPolicy("CaseCreate", p => p.RequireClaim("scp", "case:create").RequireAuthenticatedUser());
    options.AddPolicy("CaseSubmit", p => p.RequireClaim("scp", "case:submit").RequireAuthenticatedUser());
    options.AddPolicy("FileManage", p => p.RequireClaim("scp", "file:manage").RequireAuthenticatedUser());
    options.AddPolicy("ApprovalFinance", p => p.RequireClaim("scp", "approval:finance").RequireAuthenticatedUser());
    options.AddPolicy("ApprovalPMO", p => p.RequireClaim("scp", "approval:pmo").RequireAuthenticatedUser());
});

builder.Services.AddHttpContextAccessor();

// --- EF Core + Session Context Interceptor for RLS ---
builder.Services.AddScoped<SessionContextInterceptor>();
builder.Services.AddDbContext<CityAidDbContext>((sp, opts) =>
{
    var interceptor = sp.GetRequiredService<SessionContextInterceptor>();
    var connStr = builder.Configuration.GetConnectionString("Sql");
    opts.UseSqlServer(connStr);
    opts.AddInterceptors(interceptor);
});

// --- Swagger (serve static YAML + UI) ---
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // OAuth2 scheme to enable "Authorize" in UI
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "CityAid API", Version = "v1" });
    var oauthScheme = new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows
        {
            AuthorizationCode = new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri("https://login.microsoftonline.com/{tenant}/oauth2/v2.0/authorize"),
                TokenUrl = new Uri("https://login.microsoftonline.com/{tenant}/oauth2/v2.0/token"),
                Scopes = new Dictionary<string, string>
                {
                    ["case:create"] = "Create cases",
                    ["case:read"] = "Read cases",
                    ["case:update"] = "Update cases",
                    ["case:submit"] = "Submit cases",
                    ["file:manage"] = "Attach/list files",
                    ["approval:finance"] = "Finance approval",
                    ["approval:pmo"] = "PMO approval"
                }
            }
        }
    };
    c.AddSecurityDefinition("oauth2", oauthScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        [ new OpenApiSecurityScheme{ Reference = new OpenApiReference
          { Type = ReferenceType.SecurityScheme, Id = "oauth2" } } ] = new[] { "case:read" }
    });
});

var app = builder.Build();

// Middlewares
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();

// Swagger UI: point to static YAML in wwwroot/swagger/cityaid_openapi.yaml
app.UseSwagger(c =>
{
    c.RouteTemplate = "swagger/{documentName}/swagger.json"; // still serves generated (unused)
});
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/cityaid_openapi.yaml", "CityAid v1 (YAML)");
    options.OAuthUsePkce();
});

// --- Minimal endpoints (stubs) ---
app.MapGet("/health", () => Results.Ok(new { status = "ok", time = DateTimeOffset.UtcNow }));

app.MapGet("/cases", () => Results.Ok(Array.Empty<object>()))
   .RequireAuthorization("CaseRead");

app.MapPost("/cases", () => Results.Created("/cases/CS-2025-PUN-AL-001", new { id = "CS-2025-PUN-AL-001" }))
   .RequireAuthorization("CaseCreate");

app.Run();

// Make Program class accessible for testing
public partial class Program { }
