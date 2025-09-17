# Azure Authentication and Authorization Setup Guide

## Overview
The CityAid API has been updated to use Azure Entra ID for authentication and Azure role-based authorization. This document explains how to complete the setup.

## What Has Been Updated

### 1. Dependencies Added
- `Microsoft.Identity.Web` (3.14.1) - Entra ID integration
- `Microsoft.Graph` (5.93.0) - For future Azure role queries

### 2. Configuration Changes
- **appsettings.json**: Added AzureAd and MicrosoftGraph sections
- **Program.cs**: Replaced JWT Bearer with Entra ID authentication
- **Endpoints**: Applied role-based authorization policies

### 3. Authorization Policies Defined
- **AdminPolicy**: Requires `CityAid.Admin` role
- **CaseManagerPolicy**: Requires `CityAid.CaseManager` or `CityAid.Admin` role
- **CitizenPolicy**: Requires `CityAid.Citizen`, `CityAid.CaseManager`, or `CityAid.Admin` role

### 4. Endpoint Authorization Matrix
| Endpoint | Required Policy | Allowed Roles |
|----------|----------------|---------------|
| GET /cases | CitizenPolicy | Citizen, CaseManager, Admin |
| POST /cases | CitizenPolicy | Citizen, CaseManager, Admin |
| GET /cases/{id} | CitizenPolicy | Citizen, CaseManager, Admin |
| PATCH /cases/{id} | CaseManagerPolicy | CaseManager, Admin |
| POST /cases/{id}/submit | CaseManagerPolicy | CaseManager, Admin |
| POST /cases/{id}/approve | AdminPolicy | Admin |
| POST /cases/{id}/reject | AdminPolicy | Admin |
| POST /cases/{id}/files | CitizenPolicy | Citizen, CaseManager, Admin |
| GET /cases/{id}/files | CitizenPolicy | Citizen, CaseManager, Admin |

## Azure Portal Setup Required

### 1. App Registration
1. Create a new App Registration in Azure Portal
2. Note the **Application (client) ID** and **Directory (tenant) ID**
3. Configure authentication:
   - Add Web platform
   - Set redirect URIs for your environment
4. Configure API permissions:
   - Microsoft Graph: `User.Read`, `Directory.Read.All`
5. Create app roles in the manifest:

```json
{
  "appRoles": [
    {
      "allowedMemberTypes": ["User"],
      "description": "Full administrative access to CityAid",
      "displayName": "CityAid Admin",
      "id": "admin-guid-here",
      "isEnabled": true,
      "lang": null,
      "origin": "Application",
      "value": "CityAid.Admin"
    },
    {
      "allowedMemberTypes": ["User"],
      "description": "Case management access",
      "displayName": "CityAid Case Manager",
      "id": "casemanager-guid-here",
      "isEnabled": true,
      "lang": null,
      "origin": "Application",
      "value": "CityAid.CaseManager"
    },
    {
      "allowedMemberTypes": ["User"],
      "description": "Citizen access to create and view cases",
      "displayName": "CityAid Citizen",
      "id": "citizen-guid-here",
      "isEnabled": true,
      "lang": null,
      "origin": "Application",
      "value": "CityAid.Citizen"
    }
  ]
}
```

### 2. Configure appsettings.json
Update the placeholder values in `appsettings.json`:

```json
{
  "AzureAd": {
    "Instance": "https://login.microsoftonline.com/",
    "TenantId": "your-actual-tenant-id",
    "ClientId": "your-actual-client-id",
    "CallbackPath": "/signin-oidc",
    "SignedOutCallbackPath": "/signout-oidc"
  }
}
```

### 3. Assign Users to Roles
1. Go to Enterprise Applications → Your App → Users and groups
2. Assign users to the appropriate roles:
   - **CityAid.Admin**: Full system access
   - **CityAid.CaseManager**: Can manage cases but not approve/reject
   - **CityAid.Citizen**: Can create and view cases

## Testing the Setup

### 1. Swagger UI Testing
- Navigate to `/swagger` when running in development
- Use the OAuth2 authentication flow
- Test different endpoints with different user roles

### 2. JWT Token Claims
The system expects these claims in the JWT token:
- `roles`: Array containing assigned role values (e.g., ["CityAid.Admin"])
- `oid`: User object ID
- `tid`: Tenant ID

### 3. Role Verification
You can verify roles are working by:
1. Logging in as different users
2. Attempting to access restricted endpoints
3. Checking for 403 Forbidden responses for unauthorized roles

## Security Considerations

1. **Environment Variables**: Store sensitive values in environment variables or Azure Key Vault
2. **HTTPS**: Always use HTTPS in production
3. **Token Validation**: All tokens are validated against Azure AD
4. **Role-Based Access**: Fine-grained access control through Azure roles
5. **Audit Logging**: Consider adding audit logging for role-based actions

## Future Enhancements

1. **Microsoft Graph Integration**: Query user roles dynamically from Azure AD
2. **Conditional Access**: Implement location/device-based access policies
3. **Fine-Grained Permissions**: Add resource-level permissions (e.g., city-specific access)
4. **Token Caching**: Implement token caching for improved performance

## Troubleshooting

### Common Issues:
1. **401 Unauthorized**: Check app registration and configuration
2. **403 Forbidden**: Verify user has been assigned the correct role
3. **Invalid Token**: Ensure token audience matches your API's client ID
4. **Missing Roles**: Check app roles are defined and users are assigned

### Debug Tips:
- Enable detailed logging for Microsoft.AspNetCore.Authentication
- Check JWT token claims using jwt.io
- Verify redirect URIs match exactly (case-sensitive)