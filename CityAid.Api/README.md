# CityAid.Api starter

## Quickstart
```bash
# (Optional) if using SDK images or devcontainer
dotnet restore
dotnet build

# run with local dev settings (set AzureAd and ConnectionStrings in appsettings.json or env vars)
dotnet run
```

- Swagger UI: https://localhost:5001/swagger (UI) — also serves `wwwroot/swagger/cityaid_openapi.yaml`.
- Auth: Microsoft Entra ID (JWT bearer). Configure `AzureAd` in `appsettings.json`.
- DB: Azure SQL with Managed Identity (see connection string).

## Packages
- Swashbuckle for Swagger/OpenAPI
- Microsoft.Identity.Web for Entra ID
- EF Core + SqlServer
- Microsoft.Data.SqlClient
