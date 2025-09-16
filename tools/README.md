# CityAid API Testing Tools

This directory contains tools for testing the CityAid API with various user scenarios and RBAC combinations.

## 📁 Directory Structure

```
tools/
├── JwtTokenGenerator/          # JWT token generation for testing
├── ApiTestRunner/              # Automated API test execution tool
├── api-test-requests.json      # Test request definitions
├── generate-curl-commands.ps1  # PowerShell script to generate cURL commands
├── generate-curl-commands.sh   # Bash script to generate cURL commands
└── README.md                   # This file
```

## 🔐 JWT Token Generator

Generates valid JWT tokens for different user roles and cities.

```bash
cd JwtTokenGenerator
dotnet run
```

**Generated User Types:**
- **Alpha User (Pune)**: Healthcare team member in Pune
- **Beta User (Delhi)**: Education team member in Delhi
- **Finance User (Pune)**: Financial approver for Pune
- **PMO User (Country)**: Program management with country-wide access
- **Analysis User (Pune)**: Case analyst for Pune

## 🧪 API Test Runner (.NET Tool)

Advanced test runner that executes all API requests automatically with dependency management.

### Features
- ✅ Automatic JWT token management
- ✅ Request dependency resolution (case IDs)
- ✅ Detailed success/failure reporting
- ✅ JSON output for CI/CD integration
- ✅ Configurable delays between requests

### Usage

```bash
cd ApiTestRunner
dotnet run -- --file ../api-test-requests.json
```

**Options:**
- `--file`: JSON configuration file (required)
- `--delay`: Delay between requests in ms (default: 1000)
- `--verbose`: Show detailed request/response info
- `--output`: Save results to JSON file

**Examples:**
```bash
# Basic usage
dotnet run -- --file ../api-test-requests.json

# Verbose mode with custom delay
dotnet run -- --file ../api-test-requests.json --delay 500 --verbose

# Save results to file
dotnet run -- --file ../api-test-requests.json --output results.json
```

## 📋 Test Request Configuration

The `api-test-requests.json` file contains 20 comprehensive test scenarios:

### Case Creation Tests (10 scenarios)
1. **Pune Alpha**: Blood donation drive (healthcare)
2. **Delhi Beta**: Autism school admissions (education)
3. **Mumbai Alpha**: Laser treatment equipment (healthcare)
4. **Kolkata Beta**: Braille books distribution (education)
5. **Chennai Alpha**: Vaccination drive (healthcare)
6. **Pune Beta**: Higher education awareness (education)
7. **Bangalore Alpha**: Sports charity event (healthcare)
8. **Hyderabad Beta**: Study support for failed students (education)
9. **Jaipur Alpha**: Autism therapy program (healthcare)
10. **Ahmedabad Beta**: Digital learning initiative (education)

### Workflow Tests (10 scenarios)
11. **PMO View**: List all cases across country
12. **Finance View**: List city-specific cases
13. **Team View**: List team-specific cases
14. **Submit for Approval**: Alpha user submits case
15. **Finance Approval**: Finance approves submitted case
16. **PMO Final Approval**: PMO gives final approval
17. **File Attachment**: Attach SharePoint file to case
18. **File Listing**: List files attached to case
19. **Case Rejection**: Finance rejects a case
20. **Retrigger Flow**: PMO retriggers rejected case

### Cities Covered
- **PUN** (Pune)
- **DEL** (Delhi)
- **MUM** (Mumbai)
- **KOL** (Kolkata)
- **CHN** (Chennai)
- **BLR** (Bangalore)
- **HYD** (Hyderabad)
- **JAI** (Jaipur)
- **AHM** (Ahmedabad)

## 🌐 cURL Command Generation

Generate standalone cURL commands for manual testing or CI/CD integration.

### PowerShell Version
```powershell
.\generate-curl-commands.ps1
```

**Options:**
- `-ConfigFile`: Input JSON file (default: api-test-requests.json)
- `-OutputFile`: Output script file (default: curl-commands.sh)
- `-Windows`: Generate Windows-compatible commands

### Bash Version
```bash
./generate-curl-commands.sh
```

**Output:** `curl-commands.sh` - Executable script with all cURL commands

## 🚀 Quick Start Guide

### 1. Start the CityAid API
```bash
cd ../src/CityAid.Api
dotnet run
```
*API will be available at https://localhost:5151*

### 2. Generate Fresh Tokens (Optional)
```bash
cd tools/JwtTokenGenerator
dotnet run
```
*Copy tokens to update api-test-requests.json if needed*

### 3. Run Automated Tests
```bash
cd tools/ApiTestRunner
dotnet run -- --file ../api-test-requests.json --verbose
```

### 4. Or Generate cURL Commands
```bash
cd tools
./generate-curl-commands.sh
./curl-commands.sh
```

## 📊 Expected Test Results

When running against a working API:

**✅ Should Pass (18-20 tests):**
- All case creation requests (201 status)
- Case listing with proper RBAC filtering (200 status)
- File operations (200/201 status)
- Approval workflows (200 status)

**⚠️ May Fail:**
- Requests requiring specific case states
- Dependent requests if prerequisites fail
- Token expiration (24-hour limit)

## 🔧 Troubleshooting

### Common Issues

**❌ Connection Refused**
- Ensure CityAid API is running: `dotnet run --project src/CityAid.Api`
- Check API URL in configuration: `https://localhost:7144`

**❌ 401 Unauthorized**
- Generate fresh JWT tokens: `cd JwtTokenGenerator && dotnet run`
- Update tokens in `api-test-requests.json`

**❌ 403 Forbidden**
- Check RBAC rules in test scenarios
- Verify user has correct permissions for operation

**❌ 404 Not Found**
- Ensure database is migrated: `dotnet ef database update`
- Check API endpoints match OpenAPI specification

**❌ 500 Internal Server Error**
- Check API logs for detailed error information
- Verify database connection and schema

### Token Expiration
JWT tokens expire after 24 hours. If tests fail with 401 errors:

1. Generate new tokens: `cd JwtTokenGenerator && dotnet run`
2. Update `api-test-requests.json` with fresh tokens
3. Re-run tests

## 🎯 Test Scenarios Coverage

### RBAC Testing
- ✅ City-level isolation (Pune users can't see Delhi cases)
- ✅ Team-level isolation (Alpha can't see Beta cases)
- ✅ Role-based permissions (Finance can approve, Alpha cannot)
- ✅ Country-level access (PMO sees all cities)

### Workflow Testing
- ✅ Case creation and validation
- ✅ State transitions (Initiated → Approval → Approved)
- ✅ File attachment and listing
- ✅ Rejection and retrigger flows

### Data Scenarios
- ✅ Multiple cities across India
- ✅ Healthcare vs Education use cases
- ✅ Realistic case descriptions and requirements
- ✅ Proper case ID format (CS-YYYY-CITY-TEAM-###)

## 🔄 Continuous Integration

The tools support CI/CD integration:

### GitHub Actions Example
```yaml
- name: Run API Tests
  run: |
    cd tools/ApiTestRunner
    dotnet run -- --file ../api-test-requests.json --output test-results.json
```

### JSON Results Format
```json
{
  "Summary": {
    "Total": 20,
    "Passed": 18,
    "Failed": 2,
    "SuccessRate": 90.0
  },
  "Results": [...]
}
```

---

**Built for comprehensive testing of CityAid API authentication, RBAC, and business workflows** 🧪