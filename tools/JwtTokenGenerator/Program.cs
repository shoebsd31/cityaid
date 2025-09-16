using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace JwtTokenGenerator;

class Program
{
    // Use the same JWT settings as in appsettings.json
    private const string SecretKey = "YourSuperSecretKeyThatIsAtLeast32CharactersLong";
    private const string Issuer = "CityAidApi";
    private const string Audience = "CityAidClients";

    static void Main(string[] args)
    {
        Console.WriteLine("CityAid JWT Token Generator");
        Console.WriteLine("===========================");
        Console.WriteLine();

        // Generate tokens for different user types
        var tokens = new Dictionary<string, (string role, string city, string team, string[] scopes)>
        {
            ["Alpha User (Pune)"] = ("Alpha", "PUN", "AL", new[] { "case:create", "case:read", "case:submit", "file:manage" }),
            ["Beta User (Delhi)"] = ("Beta", "DEL", "BE", new[] { "case:create", "case:read", "case:submit", "file:manage" }),
            ["Finance User (Pune)"] = ("Finance", "PUN", "FIN", new[] { "case:read", "approval:finance" }),
            ["PMO User (Country)"] = ("PMO", "IN", "PMO", new[] { "case:read", "approval:finance", "approval:pmo" }),
            ["Analysis User (Pune)"] = ("Analysis", "PUN", "AN", new[] { "case:read", "case:submit" })
        };

        foreach (var (description, (role, city, team, scopes)) in tokens)
        {
            var token = GenerateJwtToken(
                userId: Guid.NewGuid().ToString(),
                userName: $"Test {role} User",
                email: $"test.{role.ToLower()}@cityaid.org",
                role: role,
                city: city,
                team: team,
                scopes: scopes
            );

            Console.WriteLine($"{description}:");
            Console.WriteLine($"  Token: {token}");
            Console.WriteLine();
        }

        Console.WriteLine("Usage in Swagger:");
        Console.WriteLine("1. Click the 'Authorize' button in Swagger UI");
        Console.WriteLine("2. Enter: Bearer [paste token here]");
        Console.WriteLine("3. Click 'Authorize' and close the dialog");
        Console.WriteLine();
        Console.WriteLine("Example Authorization header:");
        Console.WriteLine("Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...");
    }

    private static string GenerateJwtToken(
        string userId,
        string userName,
        string email,
        string role,
        string city,
        string team,
        string[] scopes)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
            new("name", userName),
            new("email", email),
            new("city", city),
            new("team", team),
            new("role", role)
        };

        // Add scope claims
        foreach (var scope in scopes)
        {
            claims.Add(new Claim("scope", scope));
        }

        var token = new JwtSecurityToken(
            issuer: Issuer,
            audience: Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(24), // 24 hour expiration
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}