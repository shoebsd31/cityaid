# CityAid API

CityAid MCP Access & Approvals is an API-first system supporting city-level teams submitting aid cases with file attachments, reviewed by Finance and PMO roles under scoped RBAC.

## Project Structure

- `CityAid.Api/` - Main API project (.NET 8)
- `CityAid.Api.Tests/` - Unit tests
- `requirement-specs/` - Project specifications and documentation

## Prerequisites

### Local Development
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Azure CLI](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli) (for Azure integrations)
- SQL Server or SQL Server LocalDB
- Visual Studio 2022 or VS Code with C# Dev Kit extension

### DevContainer Development
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- [Visual Studio Code](https://code.visualstudio.com/)
- [Dev Containers extension](https://marketplace.visualstudio.com/items?itemName=ms-vscode-remote.remote-containers)

## Running Locally

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd cityaid
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore cityaid.sln
   ```

3. **Configure app settings**
   - Update `CityAid.Api/appsettings.json` with your database connection string and Azure AD settings
   - Ensure your SQL Server instance is running

4. **Build the solution**
   ```bash
   dotnet build cityaid.sln
   ```

5. **Run the API**
   ```bash
   cd CityAid.Api
   dotnet run
   ```

6. **Access the API**
   - HTTP: http://localhost:5000
   - HTTPS: https://localhost:5001
   - Swagger UI: https://localhost:5001/swagger (development only)

## Running with DevContainer

1. **Open in VS Code**
   ```bash
   code .
   ```

2. **Reopen in Container**
   - Press `Ctrl+Shift+P` (or `Cmd+Shift+P` on Mac)
   - Select "Dev Containers: Reopen in Container"
   - Wait for the container to build and start

3. **The devcontainer will automatically:**
   - Install .NET 8 SDK
   - Install Azure CLI, PowerShell, Git, and GitHub CLI
   - Restore NuGet packages
   - Trust development certificates
   - Forward ports 5000 and 5001

4. **Run the API**
   ```bash
   cd CityAid.Api
   dotnet run
   ```

## Testing

Run unit tests:
```bash
dotnet test CityAid.Api.Tests/
```

## API Documentation

- OpenAPI specification: `requirement-specs/cityaid_openapi.yaml`
- Postman collection: `requirement-specs/cityaid_postman_collection.json`
- Environment variables: `requirement-specs/CityAid.postman_environment.json`

## Key Features

- **RBAC Implementation**: Role-based access control with city/team scoping
- **OAuth 2.0 Authentication**: Using Azure Entra ID
- **Row-Level Security**: Azure SQL RLS for data isolation
- **SharePoint Integration**: File management with proper permissions
- **MCP Compatible**: Designed for Model Context Protocol integration

## Environment Configuration

The API uses the following base URL configurations:
- Development: `https://localhost:5001`
- Example case ID format: `CS-2025-PUN-AL-001`

For production deployment, update the base URL and ensure proper Azure AD app registration configuration.