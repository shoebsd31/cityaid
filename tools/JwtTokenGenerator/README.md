# JWT Token Generator for CityAid API

A simple command-line tool to generate valid JWT bearer tokens for testing the CityAid API with different user roles and permissions.

## 🎯 Purpose

This tool generates JWT tokens that match the authentication requirements of the CityAid API, allowing you to test different user scenarios and role-based access control (RBAC) in Swagger UI or with API clients.

## 🚀 Quick Start

### Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)

### Run the Generator
```bash
cd tools/JwtTokenGenerator
dotnet run
```

## 📋 Generated User Roles

The tool generates tokens for these predefined user scenarios:

| Role | City | Team | Permissions | Use Case |
|------|------|------|-------------|----------|
| **Alpha User** | Pune (PUN) | Alpha (AL) | Create/read/submit cases, manage files | Healthcare team member |
| **Beta User** | Delhi (DEL) | Beta (BE) | Create/read/submit cases, manage files | Education team member |
| **Finance User** | Pune (PUN) | Finance (FIN) | Read cases, approve finances | Financial approver |
| **PMO User** | Country (IN) | PMO (PMO) | Read cases, all approvals, retrigger | Program management |
| **Analysis User** | Pune (PUN) | Analysis (AN) | Read cases, submit for approval | Case analyst |

## 🔐 Token Structure

Each generated JWT token contains these claims:

```json
{
  "sub": "unique-user-id",
  "jti": "token-id",
  "iat": "issued-at-timestamp",
  "name": "Test [Role] User",
  "email": "test.[role]@cityaid.org",
  "city": "city-code",
  "team": "team-code",
  "role": "role-name",
  "scope": ["permission1", "permission2", ...],
  "exp": "expiration-timestamp",
  "iss": "CityAidApi",
  "aud": "CityAidClients"
}
```

### Token Configuration
- **Expiration**: 24 hours from generation
- **Algorithm**: HMAC SHA-256
- **Secret Key**: Matches `appsettings.json` in the API
- **Issuer**: CityAidApi
- **Audience**: CityAidClients

## 🌐 Using Tokens with Swagger UI

1. **Generate tokens**: Run this tool to get fresh JWT tokens
2. **Start CityAid API**:
   ```bash
   cd ../../src/CityAid.Api
   dotnet run
   ```
3. **Open Swagger**: Navigate to https://localhost:7144/swagger
4. **Authorize**:
   - Click the 🔒 "Authorize" button (top-right)
   - Enter: `Bearer [paste token here]`
   - Click "Authorize" and close dialog
5. **Test endpoints**: All API calls now include authentication

## 📱 Using Tokens with HTTP Clients

### cURL Example
```bash
curl -X GET "https://localhost:7144/cases" \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
```

### Postman Example
1. In Headers tab, add:
   - **Key**: `Authorization`
   - **Value**: `Bearer [your-token]`

### JavaScript/Fetch Example
```javascript
fetch('https://localhost:7144/cases', {
  headers: {
    'Authorization': 'Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...',
    'Content-Type': 'application/json'
  }
})
```

## 🧪 Testing RBAC Scenarios

### City-Level Isolation
- **Pune Alpha User** can only see cases with `city: "PUN"` and `team: "AL"`
- **Delhi Beta User** can only see cases with `city: "DEL"` and `team: "BE"`
- Users cannot access cases from other cities

### Team-Level Isolation
- **Alpha teams** cannot see Beta team cases (even in same city)
- **Beta teams** cannot see Alpha team cases
- **Finance/PMO** have broader access based on their scope

### Permission Testing
| User Type | Can Create Cases | Can Approve Finance | Can Final Approve | Can Retrigger |
|-----------|------------------|--------------------|--------------------|---------------|
| Alpha/Beta | ✅ | ❌ | ❌ | ❌ |
| Finance | ❌ | ✅ | ❌ | ❌ |
| PMO | ❌ | ✅ | ✅ | ✅ |
| Analysis | ❌ | ❌ | ❌ | ❌ |

## 🔧 Customization

### Modify User Roles
Edit the `tokens` dictionary in `Program.cs` to add/modify user scenarios:

```csharp
var tokens = new Dictionary<string, (string role, string city, string team, string[] scopes)>
{
    ["Custom User"] = ("CustomRole", "NYC", "CU", new[] { "case:read" }),
    // Add more users...
};
```

### Change Token Settings
Update these constants in `Program.cs`:
```csharp
private const string SecretKey = "YourSuperSecretKeyThatIsAtLeast32CharactersLong";
private const string Issuer = "CityAidApi";
private const string Audience = "CityAidClients";
```

### Adjust Expiration
Modify the expiration time in the `GenerateJwtToken` method:
```csharp
expires: DateTime.UtcNow.AddHours(24), // Change to desired duration
```

## 🔍 Token Validation

The API validates tokens using these parameters:
- **Issuer**: Must match `CityAidApi`
- **Audience**: Must match `CityAidClients`
- **Signature**: Must be signed with the secret key
- **Expiration**: Must not be expired
- **Claims**: Must contain required claims (city, team, role, scope)

## 🛠️ Troubleshooting

### Common Issues

**❌ "Unauthorized" errors**
- Check token is not expired (24 hour limit)
- Verify Bearer prefix: `Bearer [token]`
- Ensure secret key matches API configuration

**❌ "Forbidden" errors**
- User doesn't have required permissions
- Check scope claims match endpoint requirements
- Verify city/team isolation rules

**❌ Token generation fails**
- Ensure .NET 9 SDK is installed
- Check System.IdentityModel.Tokens.Jwt package is restored

### Debugging Tips

1. **Decode token**: Use https://jwt.io to decode and inspect token claims
2. **Check logs**: API logs show authentication/authorization failures
3. **Verify claims**: Ensure generated token has expected city/team/role values

## 📚 Related Documentation

- [Main CityAid API README](../../README.md)
- [JWT.io Debugger](https://jwt.io)
- [Microsoft JWT Documentation](https://docs.microsoft.com/en-us/dotnet/api/system.identitymodel.tokens.jwt)

## 🤝 Contributing

To add new user roles or modify token generation:

1. Update the `tokens` dictionary with new user scenarios
2. Add corresponding scopes if needed
3. Test with the API to ensure RBAC works correctly
4. Update this documentation

---

**Built for testing CityAid API authentication and RBAC scenarios** 🔐