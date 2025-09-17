# CityAid API Test Runner

A command-line tool for testing the CityAid API with Azure Entra ID authentication and role-based access control validation.

## Features

- **Azure Entra ID Authentication**: Automatically authenticates users using Azure AD credentials
- **Role-Based Testing**: Tests different user roles (Admin, CaseManager, Citizen)
- **Comprehensive API Coverage**: Tests all major endpoints including cases, files, and workflow operations
- **Authorization Validation**: Verifies proper access control and permission enforcement
- **Dependency Management**: Supports test dependencies (e.g., using created case IDs in subsequent tests)
- **Detailed Reporting**: Provides comprehensive test results with timing and error information

## Prerequisites

1. **Azure AD App Registration** with:
   - Public client configuration enabled
   - Resource Owner Password Credentials (ROPC) flow enabled
   - Appropriate API permissions for CityAid API
   - Users assigned to CityAid roles

2. **.NET 9 SDK** installed

3. **CityAid API** running and accessible

## Setup

### 1. Configure Credentials

Create an `appsers.json` file in the ApiTestRunner directory:

```bash
cp appsers.json.example appsers.json
```

Update `appsers.json` with your actual Azure AD configuration:

```json
{
  "tenantId": "your-tenant-id",
  "clientId": "your-client-id",
  "users": [
    {
      "username": "admin@yourcompany.onmicrosoft.com",
      "password": "your-admin-password",
      "role": "Admin",
      "description": "Full administrative access"
    },
    {
      "username": "manager@yourcompany.onmicrosoft.com",
      "password": "your-manager-password",
      "role": "CaseManager",
      "description": "Case management access"
    },
    {
      "username": "user@yourcompany.onmicrosoft.com",
      "password": "your-user-password",
      "role": "Citizen",
      "description": "Basic citizen access"
    }
  ],
  "scopes": [
    "api://your-client-id/access_as_user"
  ]
}
```

**⚠️ Security Note**: The `appsers.json` file is automatically excluded from git. Never commit credentials to source control.

### 2. Configure Test Cases

Create a test configuration file or use the provided example:

```bash
cp test-config.example.json my-tests.json
```

Update the `apiBaseUrl` to point to your CityAid API instance:

```json
{
  "apiBaseUrl": "https://localhost:7154",
  "requests": [
    // ... test definitions
  ]
}
```

## Usage

### Basic Usage

```bash
dotnet run --file my-tests.json --credentials appsers.json
```

### Advanced Options

```bash
# Verbose output with detailed request/response information
dotnet run --file my-tests.json --credentials appsers.json --verbose

# Custom delay between requests (default: 1000ms)
dotnet run --file my-tests.json --credentials appsers.json --delay 2000

# Save results to JSON file
dotnet run --file my-tests.json --credentials appsers.json --output results.json

# All options combined
dotnet run --file my-tests.json --credentials appsers.json --verbose --delay 500 --output detailed-results.json
```

### Build and Run Standalone

```bash
# Build the tool
dotnet build

# Run the built executable
./bin/Debug/net9.0/ApiTestRunner.exe --file my-tests.json --credentials appsers.json
```

## Test Configuration Format

### Test Request Structure

```json
{
  "name": "Test Name",
  "description": "Detailed description of what this test does",
  "method": "GET|POST|PUT|PATCH|DELETE",
  "endpoint": "/api/endpoint",
  "userRole": "Admin|CaseManager|Citizen",
  "payload": {
    // Request body for POST/PUT/PATCH requests
  },
  "expectedStatus": 200,
  "dependsOn": "Previous Test Name" // Optional: Use case ID from another test
}
```

### User Roles

- **Admin**: Full system access, can approve/reject cases
- **CaseManager**: Can manage cases, submit for approval, but cannot approve
- **Citizen**: Can create cases, view own cases, attach files

### Endpoint Placeholders

Use `{caseId}` in endpoints to reference case IDs created by previous tests:

```json
{
  "name": "Get Created Case",
  "endpoint": "/cases/{caseId}",
  "dependsOn": "Create Case - Citizen"
}
```

## Sample Test Scenarios

The included `test-config.example.json` demonstrates:

1. **Read Operations**: Testing case listing with different user roles
2. **Case Creation**: Creating cases as different user types
3. **Case Management**: Updating, submitting, and approving cases
4. **File Operations**: Attaching and listing case files
5. **Authorization Testing**: Verifying access control enforcement

## Output Examples

### Successful Run

```
🚀 CityAid API Test Runner
📄 Config: my-tests.json
🔐 Credentials: appsers.json
🎯 Target: https://localhost:7154
🧪 Tests: 12
👥 Users: 3
⏱️  Delay: 1000ms between requests

🔐 Authenticating users...
  👤 Admin: admin@company.com
    ✅ Authentication successful
  👤 CaseManager: manager@company.com
    ✅ Authentication successful
  👤 Citizen: user@company.com
    ✅ Authentication successful

[1/12] Get Cases - Citizen
  ✅ 200 - 245ms

[2/12] Create Case - Citizen
  ✅ 201 - 892ms
  📋 Created Case ID: case-12345

📊 Test Summary:
  ✅ Passed: 12/12
  ❌ Failed: 0/12
  📈 Success Rate: 100.0%
```

### With Errors

```
[5/12] Access Denied - Citizen Approve
  ❌ 200 - Expected status 403, got 200
  📄 Response: {"message":"Case approved successfully"}
```

## Troubleshooting

### Authentication Issues

**Error**: `AADSTS50126: Invalid username or password`
- Verify credentials in `appsers.json`
- Ensure users exist in Azure AD
- Check if MFA is disabled for test accounts

**Error**: `AADSTS65001: The user or administrator has not consented`
- Grant admin consent for the API permissions
- Ensure ROPC flow is enabled in app registration

**Error**: `AADSTS50194: Application is not configured as multi-tenant`
- Configure app registration for appropriate tenant type
- Use correct authority URL in configuration

### API Issues

**Error**: `401 Unauthorized`
- Verify API is running and accessible
- Check that JWT tokens are being accepted
- Ensure correct audience/scope configuration

**Error**: `403 Forbidden`
- Verify user has correct role assignments
- Check API authorization policies
- Confirm app roles are properly configured

### Configuration Issues

**Error**: `Failed to parse configuration file`
- Validate JSON syntax in configuration files
- Ensure all required fields are present
- Check file paths and permissions

## Security Considerations

1. **Credential Storage**: Never commit `appsers.json` to source control
2. **Test Accounts**: Use dedicated test accounts with minimal permissions
3. **Network Security**: Run tests in secure development environments
4. **Token Handling**: Tokens are stored in memory only and cleared on exit
5. **ROPC Flow**: Consider more secure authentication flows for production

## Development

### Adding New Test Cases

1. Add test definition to your configuration JSON
2. Specify appropriate user role and expected status
3. Use dependency chaining for tests that need case IDs
4. Test both positive and negative scenarios

### Extending Functionality

The tool can be extended to support:
- Additional authentication flows
- Custom assertion logic
- Performance benchmarking
- Integration with CI/CD pipelines
- Custom reporting formats

## Contributing

When contributing to the ApiTestRunner:

1. Maintain backward compatibility with existing test configurations
2. Add appropriate error handling and logging
3. Update documentation for new features
4. Test with different Azure AD configurations
5. Follow secure coding practices

## Version History

- **v1.0**: Initial release with basic JWT token support
- **v2.0**: Added Azure Entra ID authentication and RBAC testing
- **v2.1**: Enhanced error handling and verbose output options