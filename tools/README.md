# CityAid API Testing Tools

This directory contains tools for testing the CityAid API with Azure Entra ID authentication and role-based access control (RBAC) validation.

## 📁 Directory Structure

```
tools/
├── ApiTestRunner/              # Automated API test execution tool with Azure AD authentication
├── generate-curl-commands.ps1  # PowerShell script to generate cURL commands (deprecated)
├── generate-curl-commands.sh   # Bash script to generate cURL commands (deprecated)
└── README.md                   # This file
```

## 🧪 API Test Runner (.NET Tool)

Advanced test runner that executes all API requests automatically with Azure Entra ID authentication and dependency management.

### Features
- ✅ **Azure Entra ID Authentication**: Automatic login with username/password
- ✅ **Role-Based Testing**: Tests Admin, CaseManager, and Citizen roles
- ✅ **Request Dependency Resolution**: Automatic case ID management
- ✅ **Authorization Validation**: Verifies proper access control
- ✅ **Detailed Reporting**: Success/failure with timing and error details
- ✅ **JSON Output**: CI/CD integration support
- ✅ **Configurable Delays**: Control request pacing

### Setup

1. **Configure Credentials**: Copy and update the credentials template:
   ```bash
   cd ApiTestRunner
   cp appsers.json.example appsers.json
   # Edit appsers.json with your Azure AD tenant and user credentials
   ```

2. **Configure Tests**: Copy and update the test configuration:
   ```bash
   cp test-config.example.json my-tests.json
   # Edit my-tests.json to adjust API URL and test scenarios
   ```

### Usage

```bash
cd ApiTestRunner
dotnet run -- --file my-tests.json --credentials appsers.json
```

**Options:**
- `--file`: JSON test configuration file (required)
- `--credentials`: JSON credentials file with Azure AD config (required)
- `--delay`: Delay between requests in ms (default: 1000)
- `--verbose`: Show detailed request/response info
- `--output`: Save results to JSON file

**Examples:**
```bash
# Basic usage
dotnet run -- --file my-tests.json --credentials appsers.json

# Verbose mode with custom delay
dotnet run -- --file my-tests.json --credentials appsers.json --delay 500 --verbose

# Save results to file
dotnet run -- --file my-tests.json --credentials appsers.json --output results.json
```

## 📋 Test Configuration

The `test-config.example.json` file contains comprehensive test scenarios covering:

### Role-Based Access Testing
- **Admin Role**: Full system access, case approval/rejection
- **CaseManager Role**: Case management, submission for approval
- **Citizen Role**: Case creation, viewing, file attachment

### API Endpoint Coverage
1. **GET /cases**: List cases with role-based filtering
2. **POST /cases**: Create new cases
3. **GET /cases/{id}**: Retrieve specific cases
4. **PATCH /cases/{id}**: Update case metadata (CaseManager+)
5. **POST /cases/{id}/submit**: Submit for approval (CaseManager+)
6. **POST /cases/{id}/approve**: Approve cases (Admin only)
7. **POST /cases/{id}/reject**: Reject cases (Admin only)
8. **POST /cases/{id}/files**: Attach files to cases
9. **GET /cases/{id}/files**: List case files

### Authorization Boundary Testing
- Verifies proper 403 Forbidden responses
- Tests role escalation prevention
- Validates resource access controls

## 🔐 Azure AD Configuration

The tool requires proper Azure AD app registration and user setup:

### App Registration Requirements
1. **Public Client**: Enable "Allow public client flows"
2. **API Permissions**: Grant access to your CityAid API scope
3. **App Roles**: Define Admin, CaseManager, and Citizen roles
4. **User Assignment**: Assign test users to appropriate roles

### Credentials Configuration
```json
{
  "tenantId": "your-azure-tenant-id",
  "clientId": "your-app-registration-client-id",
  "users": [
    {
      "username": "admin@yourcompany.com",
      "password": "password",
      "role": "Admin"
    }
  ],
  "scopes": ["api://your-client-id/access_as_user"]
}
```

## 🚀 Quick Start Guide

### 1. Start the CityAid API
```bash
cd ../src/CityAid.Api
dotnet run
```
*API will be available at https://localhost:7154*

### 2. Configure Azure AD Authentication
1. Update `appsers.json` with your Azure AD tenant and client IDs
2. Add test user credentials with appropriate role assignments
3. Ensure API is configured with matching Azure AD settings

### 3. Run Automated Tests
```bash
cd tools/ApiTestRunner
dotnet run -- --file test-config.example.json --credentials appsers.json --verbose
```

### 4. Review Results
- Check console output for test results
- Use `--output results.json` to save detailed results
- Investigate any failures with `--verbose` mode

## 📊 Expected Test Results

When running against a properly configured API:

**✅ Should Pass:**
- User authentication for all roles (Admin, CaseManager, Citizen)
- Case creation requests (201 status)
- Case listing with proper RBAC filtering (200 status)
- File operations (200/201 status)
- Role-appropriate approval workflows (200 status)
- Authorization boundary tests (403 status for denied operations)

**⚠️ May Fail:**
- Authentication issues if Azure AD is misconfigured
- Dependent requests if prerequisites fail
- Database connectivity or migration issues

## 🔧 Troubleshooting

### Common Issues

**❌ Authentication Failed**
- Verify Azure AD tenant and client IDs in `appsers.json`
- Check user credentials and ensure accounts exist
- Ensure MFA is disabled for test accounts
- Verify "Allow public client flows" is enabled in app registration

**❌ 401 Unauthorized**
- Check API is configured with same Azure AD tenant
- Verify audience/scope configuration matches
- Ensure app registration has required API permissions

**❌ 403 Forbidden**
- Verify users are assigned to correct Azure AD app roles
- Check RBAC policies in API are properly configured
- Confirm role claims are included in JWT tokens

**❌ Connection Refused**
- Ensure CityAid API is running: `dotnet run --project src/CityAid.Api`
- Check API URL in test configuration matches actual API address

**❌ 500 Internal Server Error**
- Check API logs for detailed error information
- Verify database connection and schema
- Ensure database migrations are applied

## 🎯 Test Scenarios Coverage

### RBAC Testing
- ✅ **Role-based Access Control**: Admin, CaseManager, and Citizen permissions
- ✅ **Authorization Boundaries**: Proper 403 responses for unauthorized actions
- ✅ **Permission Escalation Prevention**: Citizens cannot perform admin actions
- ✅ **Resource Access Control**: Users can only access appropriate resources

### Workflow Testing
- ✅ **Case Lifecycle**: Creation → Update → Submit → Approve
- ✅ **File Management**: Attach and list case files
- ✅ **State Transitions**: Proper workflow state management
- ✅ **Dependency Testing**: Tests that require case IDs from previous operations

### Authentication Testing
- ✅ **Azure AD Integration**: Username/password authentication flow
- ✅ **Token Management**: Automatic token acquisition and usage
- ✅ **Multi-Role Support**: Tests with different user roles simultaneously
- ✅ **Error Handling**: Proper response to authentication failures

## 🔄 Continuous Integration

The tools support CI/CD integration with secure credential management:

### GitHub Actions Example
```yaml
- name: Setup Test Credentials
  run: |
    echo '${{ secrets.AZURE_TEST_CREDENTIALS }}' > tools/ApiTestRunner/appsers.json

- name: Run API Tests
  run: |
    cd tools/ApiTestRunner
    dotnet run -- --file test-config.example.json --credentials appsers.json --output test-results.json

- name: Clean Credentials
  run: rm tools/ApiTestRunner/appsers.json
  if: always()
```

### JSON Results Format
```json
{
  "Summary": {
    "Total": 12,
    "Passed": 11,
    "Failed": 1,
    "SuccessRate": 91.7
  },
  "Results": [
    {
      "Name": "Get Cases - Citizen",
      "Success": true,
      "StatusCode": 200,
      "Duration": "00:00:00.245"
    }
  ]
}
```

## 📚 Additional Resources

- **API Documentation**: See `AZURE_AUTH_SETUP.md` for detailed Azure AD configuration
- **ApiTestRunner README**: Full documentation in `tools/ApiTestRunner/README.md`
- **Security Guide**: Review security considerations for test environment setup

---

**Built for comprehensive testing of CityAid API with Azure Entra ID authentication and RBAC validation** 🔐