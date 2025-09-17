# CityAid API - .NET 9 DDD Solution

A production-ready REST API for CityAid NGO's case management and approval system, built with .NET 9 following Domain-Driven Design principles.

## 🏗️ Architecture Overview

CityAid manages healthcare (Alpha) and education (Beta) cases across multiple cities in India with a structured approval workflow and role-based access control.

### Solution Structure
```
src/
├── CityAid.Domain/         # Domain entities, value objects, domain events
├── CityAid.Application/    # Use cases, DTOs, validators (CQRS with MediatR)
├── CityAid.Infrastructure/ # EF Core, SQL Server, repositories
└── CityAid.Api/           # ASP.NET Core API, OpenAPI, authentication

tests/
├── CityAid.Domain.Tests/      # Domain logic unit tests
├── CityAid.Application.Tests/ # Application service tests
└── CityAid.Api.Tests/        # Integration tests
```

### Key Features
- **Domain-Driven Design**: Rich domain model with proper aggregates
- **CQRS Pattern**: Command/Query separation using MediatR
- **Role-Based Access Control**: City and team-level data isolation
- **Case Workflow**: Initiated → Analysis → Finance → PMO approval flow
- **JWT Authentication**: Claims-based authorization
- **OpenAPI 3.0**: Interactive Swagger documentation
- **EF Core 9**: Code-first migrations with SQL Server
- **Comprehensive Testing**: 22 unit and integration tests

## 🚀 Quick Start

### Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (LocalDB or full instance)
- [Entity Framework Core Tools](https://docs.microsoft.com/en-us/ef/core/cli/dotnet) (installed globally)

### 1. Clone and Build
```bash
git clone <repository-url>
cd cityaid
dotnet build
```

### 2. Install EF Core Tools (if not already installed)
```bash
dotnet tool install --global dotnet-ef
```

### 3. Database Setup
```bash
# Navigate to Infrastructure project
cd src/CityAid.Infrastructure

# Create/update database with migrations
dotnet ef database update --startup-project ../CityAid.Api

# Alternative: Run from solution root
dotnet ef database update --project src/CityAid.Infrastructure --startup-project src/CityAid.Api
```

### 4. Run the API
```bash
# Navigate to API project
cd src/CityAid.Api

# Run the application
dotnet run

# Or run from solution root
dotnet run --project src/CityAid.Api
```

The API will be available at:
- **HTTPS**: https://localhost:7144
- **HTTP**: http://localhost:5144
- **Swagger UI**: https://localhost:7144/swagger

### 5. Run Tests
```bash
# Run all tests (22 tests)
dotnet test

# Run with detailed output
dotnet test --verbosity normal

# Run specific test project
dotnet test tests/CityAid.Domain.Tests
dotnet test tests/CityAid.Api.Tests
```

## 🔐 Authentication & Authorization

### JWT Token Format
The API uses JWT Bearer tokens with the following claims structure:
```json
{
  "sub": "user-id",
  "name": "User Name",
  "email": "user@example.com",
  "city": "PUN",
  "team": "AL",
  "role": "Alpha",
  "scope": "case:create case:read case:submit"
}
```

### Testing with Azure Authentication
The API now uses Azure Entra ID for authentication. Configure test credentials:

```bash
cd tools/ApiTestRunner
cp appsers.json.example appsers.json
# Edit appsers.json with your Azure AD configuration
```

See `AZURE_AUTH_SETUP.md` for detailed Azure AD configuration instructions.

### Using Tokens in Swagger
1. **Start the API**: `dotnet run --project src/CityAid.Api`
2. **Open Swagger**: Navigate to https://localhost:7144/swagger
3. **Generate Token**: Run the token generator above and copy a token
4. **Authorize in Swagger**:
   - Click the 🔒 "Authorize" button (top right)
   - Enter: `Bearer [paste your token here]`
   - Click "Authorize" and close the dialog
5. **Test Endpoints**: All API calls will now include the bearer token

### Sample Test Tokens
**Alpha Team User (Pune)** - Can create/read cases for Pune Alpha team:
```
eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIzZWVmNWQ3OC00OGQ3LTQwYjQtOTdkYi0wZmI0MTQ3MTQ5NTYiLCJqdGkiOiJmM2ExNmNmNS1iODIyLTRjNDEtYmRmMi1lOGNlMGMyMzVlOWIiLCJpYXQiOjE3NTgwNTM1MjQsIm5hbWUiOiJUZXN0IEFscGhhIFVzZXIiLCJlbWFpbCI6InRlc3QuYWxwaGFAY2l0eWFpZC5vcmciLCJjaXR5IjoiUFVOIiwidGVhbSI6IkFMIiwicm9sZSI6IkFscGhhIiwic2NvcGUiOlsiY2FzZTpjcmVhdGUiLCJjYXNlOnJlYWQiLCJjYXNlOnN1Ym1pdCIsImZpbGU6bWFuYWdlIl0sImV4cCI6MTc1ODEzOTkyNCwiaXNzIjoiQ2l0eUFpZEFwaSIsImF1ZCI6IkNpdHlBaWRDbGllbnRzIn0.2FYJF_F2173trEtZ4CXt8ofR3CvxmIR1vrOK7nR9nmo
```

**Finance User (Pune)** - Can approve cases for Pune:
```
eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIzODFhYmU1NS01ZjM3LTRlYTctOTExMy02NWI0NDRhMGU1MzkiLCJqdGkiOiIwZGY0MmU1MS1iY2ZkLTQ3YTYtODA2NC1jYWExNzkxNzhlMTEiLCJpYXQiOjE3NTgwNTM1MjUsIm5hbWUiOiJUZXN0IEZpbmFuY2UgVXNlciIsImVtYWlsIjoidGVzdC5maW5hbmNlQGNpdHlhaWQub3JnIiwiY2l0eSI6IlBVTiIsInRlYW0iOiJGSU4iLCJyb2xlIjoiRmluYW5jZSIsInNjb3BlIjpbImNhc2U6cmVhZCIsImFwcHJvdmFsOmZpbmFuY2UiXSwiZXhwIjoxNzU4MTM5OTI1LCJpc3MiOiJDaXR5QWlkQXBpIiwiYXVkIjoiQ2l0eUFpZENsaWVudHMifQ.uoIGt8RiVbtmW8We_oHR6_lLMx-Wb19LCPLkn8v6umg
```

**PMO User (Country)** - Can approve any case across all cities:
```
eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiI2ZTVmZTNjOS04ODFhLTQ0MDgtOTg4Mi0yN2M2ZjRjNWZjOTIiLCJqdGkiOiJjY2RiYjA5OS0wNzFlLTRhOWEtYjVkNy0zYzBmMTM1MDI5MGEiLCJpYXQiOjE3NTgwNTM1MjUsIm5hbWUiOiJUZXN0IFBNTyBVc2VyIiwiZW1haWwiOiJ0ZXN0LnBtb0BjaXR5YWlkLm9yZyIsImNpdHkiOiJJTiIsInRlYW0iOiJQTU8iLCJyb2xlIjoiUE1PIiwic2NvcGUiOlsiY2FzZTpyZWFkIiwiYXBwcm92YWw6ZmluYW5jZSIsImFwcHJvdmFsOnBtbyJdLCJleHAiOjE3NTgxMzk5MjUsImlzcyI6IkNpdHlBaWRBcGkiLCJhdWQiOiJDaXR5QWlkQ2xpZW50cyJ9.lgwPe_3jYx---c_pak0v5D3vt0aaSYB8mdsjgn-CSuw
```

### Role-Based Permissions
| Role | Permissions |
|------|-------------|
| Alpha/Beta | Create cases, attach files, submit for approval |
| Analysis | Review and analyze cases |
| Finance | Approve/reject financial aspects |
| PMO | Final approval, retrigger rejected cases |

## 📊 Database Schema

### Connection String
Update `appsettings.json` or `appsettings.Development.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=CityAidDb;Trusted_Connection=true;MultipleActiveResultSets=true"
  }
}
```

### Core Tables
- **Cases**: Main case entity with approval workflow
- **Files**: SharePoint file metadata linked to cases
- **Users**: User information and role assignments
- **ApprovalHistory**: Audit trail for case state changes

## 🔄 Case Workflow

### States
1. **Initiated**: Case created by Alpha/Beta team
2. **Pending_Analysis**: Awaiting analysis team review
3. **Pending_Finance**: Awaiting finance approval
4. **Pending_PMO**: Awaiting PMO final approval
5. **Approved**: Case approved and can proceed
6. **Rejected**: Case rejected (can be retriggered by PMO)

### State Transitions
```
Initiated → Pending_Analysis → Pending_Finance → Pending_PMO → Approved
     ↓            ↓                 ↓             ↓
   Rejected ← Rejected ← Rejected ← Rejected
     ↓
   PMO Retrigger → Pending_Finance
```

## 🌐 API Endpoints

### Cases
- `GET /cases` - List cases (with pagination and filtering)
- `POST /cases` - Create new case
- `GET /cases/{id}` - Get case by ID
- `PATCH /cases/{id}` - Update case metadata
- `POST /cases/{id}/submit` - Submit case for approval

### Files
- `GET /cases/{id}/files` - List files for case
- `POST /cases/{id}/files` - Attach file to case

### Approvals
- `POST /cases/{id}/approve` - Approve case (Finance/PMO)
- `POST /cases/{id}/reject` - Reject case
- `POST /cases/{id}/retrigger` - Retrigger rejected case (PMO only)

### Example Case ID Format
- Healthcare in Pune: `CS-2025-PUN-AL-001`
- Education in Delhi: `CS-2025-DEL-BE-002`

## 🧪 Testing

### Test Coverage
- **Domain Tests**: 14 tests covering business logic and RBAC
- **Application Tests**: 1 test for use case handlers
- **Integration Tests**: 7 tests for API endpoints

### Key Test Scenarios
- Case creation and validation
- RBAC enforcement (city/team isolation)
- Approval workflow state transitions
- File attachment operations
- Error handling (400, 404, 403 responses)

### Running Specific Tests
```bash
# Test case creation
dotnet test --filter "CreateCase"

# Test RBAC enforcement
dotnet test --filter "RBAC"

# Test API endpoints
dotnet test tests/CityAid.Api.Tests
```

## 🔧 Development Commands

### Entity Framework Operations
```bash
# Add new migration
dotnet ef migrations add MigrationName --project src/CityAid.Infrastructure --startup-project src/CityAid.Api

# Remove last migration
dotnet ef migrations remove --project src/CityAid.Infrastructure --startup-project src/CityAid.Api

# Update database
dotnet ef database update --project src/CityAid.Infrastructure --startup-project src/CityAid.Api

# Generate SQL script
dotnet ef migrations script --project src/CityAid.Infrastructure --startup-project src/CityAid.Api
```

### Build & Package
```bash
# Clean solution
dotnet clean

# Restore packages
dotnet restore

# Build solution
dotnet build

# Build for production
dotnet build --configuration Release

# Publish API
dotnet publish src/CityAid.Api --configuration Release --output ./publish
```

## 🐳 Docker Support (Optional)

### Dockerfile
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["src/CityAid.Api/CityAid.Api.csproj", "src/CityAid.Api/"]
COPY ["src/CityAid.Application/CityAid.Application.csproj", "src/CityAid.Application/"]
COPY ["src/CityAid.Domain/CityAid.Domain.csproj", "src/CityAid.Domain/"]
COPY ["src/CityAid.Infrastructure/CityAid.Infrastructure.csproj", "src/CityAid.Infrastructure/"]
RUN dotnet restore "src/CityAid.Api/CityAid.Api.csproj"
COPY . .
WORKDIR "/src/src/CityAid.Api"
RUN dotnet build "CityAid.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CityAid.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CityAid.Api.dll"]
```

## 🛡️ Security Considerations

### Production Deployment
1. **JWT Secrets**: Use strong, unique JWT signing keys
2. **Connection Strings**: Store in Azure Key Vault or similar
3. **HTTPS**: Enforce HTTPS in production
4. **CORS**: Configure appropriate CORS policies
5. **Rate Limiting**: Implement API rate limiting
6. **Logging**: Enable comprehensive logging and monitoring

### Environment Variables
```bash
# Required for production
ASPNETCORE_ENVIRONMENT=Production
JWT_SECRET_KEY=your-super-secret-key-here
CONNECTION_STRING=your-production-connection-string
```

## 📚 Additional Resources

- [Domain-Driven Design](https://docs.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)
- [ASP.NET Core Security](https://docs.microsoft.com/en-us/aspnet/core/security/)
- [OpenAPI/Swagger](https://docs.microsoft.com/en-us/aspnet/core/tutorials/web-api-help-pages-using-swagger)

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Run tests (`dotnet test`)
4. Commit changes (`git commit -m 'Add amazing feature'`)
5. Push to branch (`git push origin feature/amazing-feature`)
6. Open a Pull Request

---

**Built with ❤️ using .NET 9 and Domain-Driven Design principles**