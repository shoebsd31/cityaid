using System.CommandLine;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Identity.Client;
using System.Security;

namespace ApiTestRunner;

public class UserCredentials
{
    [JsonPropertyName("username")]
    public string Username { get; set; } = string.Empty;

    [JsonPropertyName("password")]
    public string Password { get; set; } = string.Empty;

    [JsonPropertyName("role")]
    public string Role { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;
}

public class AuthConfiguration
{
    [JsonPropertyName("tenantId")]
    public string TenantId { get; set; } = string.Empty;

    [JsonPropertyName("clientId")]
    public string ClientId { get; set; } = string.Empty;

    [JsonPropertyName("users")]
    public List<UserCredentials> Users { get; set; } = new();

    [JsonPropertyName("scopes")]
    public List<string> Scopes { get; set; } = new();
}

public class TestRequest
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("method")]
    public string Method { get; set; } = string.Empty;

    [JsonPropertyName("endpoint")]
    public string Endpoint { get; set; } = string.Empty;

    [JsonPropertyName("userRole")]
    public string UserRole { get; set; } = string.Empty;

    [JsonPropertyName("payload")]
    public JsonElement? Payload { get; set; }

    [JsonPropertyName("expectedStatus")]
    public int ExpectedStatus { get; set; }

    [JsonPropertyName("dependsOn")]
    public string? DependsOn { get; set; }
}

public class TestConfiguration
{
    [JsonPropertyName("apiBaseUrl")]
    public string ApiBaseUrl { get; set; } = string.Empty;

    [JsonPropertyName("requests")]
    public List<TestRequest> Requests { get; set; } = new();
}

public class TestResult
{
    public string Name { get; set; } = string.Empty;
    public bool Success { get; set; }
    public int StatusCode { get; set; }
    public string Response { get; set; } = string.Empty;
    public string? Error { get; set; }
    public TimeSpan Duration { get; set; }
    public string? CreatedCaseId { get; set; }
}

class Program
{
    private static readonly HttpClient httpClient = new();
    private static readonly Dictionary<string, string> createdCaseIds = new();
    private static readonly Dictionary<string, string> accessTokens = new();
    private static readonly JsonSerializerOptions jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    static async Task<int> Main(string[] args)
    {
        var fileOption = new Option<FileInfo>(
            name: "--file",
            description: "JSON file containing API test requests")
        {
            IsRequired = true
        };

        var delayOption = new Option<int>(
            name: "--delay",
            description: "Delay between requests in milliseconds",
            getDefaultValue: () => 1000);

        var verboseOption = new Option<bool>(
            name: "--verbose",
            description: "Show detailed request/response information",
            getDefaultValue: () => false);

        var outputOption = new Option<FileInfo?>(
            name: "--output",
            description: "Output file for test results (JSON format)");

        var credentialsOption = new Option<FileInfo>(
            name: "--credentials",
            description: "JSON file containing user credentials")
        {
            IsRequired = true
        };

        var rootCommand = new RootCommand("CityAid API Test Runner")
        {
            fileOption,
            credentialsOption,
            delayOption,
            verboseOption,
            outputOption
        };

        rootCommand.SetHandler(async (file, credentials, delay, verbose, output) =>
        {
            await RunTests(file, credentials, delay, verbose, output);
        }, fileOption, credentialsOption, delayOption, verboseOption, outputOption);

        return await rootCommand.InvokeAsync(args);
    }

    static async Task RunTests(FileInfo configFile, FileInfo credentialsFile, int delay, bool verbose, FileInfo? outputFile)
    {
        try
        {
            // Load configuration
            var configJson = await File.ReadAllTextAsync(configFile.FullName);
            var config = JsonSerializer.Deserialize<TestConfiguration>(configJson, jsonOptions);

            if (config == null)
            {
                Console.WriteLine("❌ Failed to parse configuration file");
                return;
            }

            // Load credentials
            var credentialsJson = await File.ReadAllTextAsync(credentialsFile.FullName);
            var authConfig = JsonSerializer.Deserialize<AuthConfiguration>(credentialsJson, jsonOptions);

            if (authConfig == null)
            {
                Console.WriteLine("❌ Failed to parse credentials file");
                return;
            }

            Console.WriteLine($"🚀 CityAid API Test Runner");
            Console.WriteLine($"📄 Config: {configFile.Name}");
            Console.WriteLine($"🔐 Credentials: {credentialsFile.Name}");
            Console.WriteLine($"🎯 Target: {config.ApiBaseUrl}");
            Console.WriteLine($"🧪 Tests: {config.Requests.Count}");
            Console.WriteLine($"👥 Users: {authConfig.Users.Count}");
            Console.WriteLine($"⏱️  Delay: {delay}ms between requests");
            Console.WriteLine();

            // Authenticate users and get access tokens
            Console.WriteLine("🔐 Authenticating users...");
            await AuthenticateUsers(authConfig, verbose);
            Console.WriteLine();

            // Configure HTTP client
            httpClient.Timeout = TimeSpan.FromSeconds(30);
            httpClient.DefaultRequestHeaders.Add("User-Agent", "CityAid-API-Test-Runner/1.0");

            var results = new List<TestResult>();
            var successCount = 0;

            // Execute tests
            for (int i = 0; i < config.Requests.Count; i++)
            {
                var request = config.Requests[i];
                Console.WriteLine($"[{i + 1}/{config.Requests.Count}] {request.Name}");

                if (verbose)
                {
                    Console.WriteLine($"  📝 {request.Description}");
                }

                var result = await ExecuteRequest(config, request, verbose);
                results.Add(result);

                if (result.Success)
                {
                    successCount++;
                    Console.WriteLine($"  ✅ {result.StatusCode} - {result.Duration.TotalMilliseconds:F0}ms");

                    // Store case ID if this was a case creation
                    if (result.CreatedCaseId != null)
                    {
                        createdCaseIds[request.Name] = result.CreatedCaseId;
                        if (verbose)
                        {
                            Console.WriteLine($"  📋 Created Case ID: {result.CreatedCaseId}");
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"  ❌ {result.StatusCode} - {result.Error}");
                    if (verbose && !string.IsNullOrEmpty(result.Response))
                    {
                        Console.WriteLine($"  📄 Response: {result.Response}");
                    }
                }

                if (verbose)
                {
                    Console.WriteLine();
                }

                // Add delay between requests
                if (i < config.Requests.Count - 1)
                {
                    await Task.Delay(delay);
                }
            }

            // Summary
            Console.WriteLine($"\n📊 Test Summary:");
            Console.WriteLine($"  ✅ Passed: {successCount}/{config.Requests.Count}");
            Console.WriteLine($"  ❌ Failed: {config.Requests.Count - successCount}/{config.Requests.Count}");
            Console.WriteLine($"  📈 Success Rate: {(double)successCount / config.Requests.Count * 100:F1}%");

            // Save results if output file specified
            if (outputFile != null)
            {
                var resultsJson = JsonSerializer.Serialize(new
                {
                    Summary = new
                    {
                        Total = config.Requests.Count,
                        Passed = successCount,
                        Failed = config.Requests.Count - successCount,
                        SuccessRate = (double)successCount / config.Requests.Count * 100
                    },
                    Results = results
                }, jsonOptions);

                await File.WriteAllTextAsync(outputFile.FullName, resultsJson);
                Console.WriteLine($"📁 Results saved to: {outputFile.FullName}");
            }

            // Display created case IDs
            if (createdCaseIds.Any())
            {
                Console.WriteLine($"\n📋 Created Cases:");
                foreach (var (requestName, caseId) in createdCaseIds)
                {
                    Console.WriteLine($"  {caseId} - {requestName}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error: {ex.Message}");
            if (verbose)
            {
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }
    }

    static async Task AuthenticateUsers(AuthConfiguration authConfig, bool verbose)
    {
        foreach (var user in authConfig.Users)
        {
            try
            {
                Console.WriteLine($"  👤 {user.Role}: {user.Username}");

                var app = PublicClientApplicationBuilder
                    .Create(authConfig.ClientId)
                    .WithAuthority($"https://login.microsoftonline.com/{authConfig.TenantId}")
                    .WithRedirectUri("http://localhost")
                    .Build();

                var securePassword = new SecureString();
                foreach (char c in user.Password)
                    securePassword.AppendChar(c);
                securePassword.MakeReadOnly();

                var result = await app.AcquireTokenByUsernamePassword(
                    authConfig.Scopes,
                    user.Username,
                    securePassword)
                    .ExecuteAsync();

                accessTokens[user.Role] = result.AccessToken;
                Console.WriteLine($"    ✅ Authentication successful");

                if (verbose)
                {
                    Console.WriteLine($"    🎫 Token expires: {result.ExpiresOn:yyyy-MM-dd HH:mm:ss}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"    ❌ Authentication failed: {ex.Message}");
                if (verbose)
                {
                    Console.WriteLine($"    📄 Details: {ex}");
                }
            }
        }
    }

    static async Task<TestResult> ExecuteRequest(TestConfiguration config, TestRequest request, bool verbose)
    {
        var result = new TestResult { Name = request.Name };
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        try
        {
            // Get access token for user role
            if (!accessTokens.TryGetValue(request.UserRole, out var token))
            {
                result.Error = $"No access token available for role '{request.UserRole}'";
                return result;
            }

            // Resolve endpoint with case ID if needed
            var endpoint = request.Endpoint;
            if (endpoint.Contains("{caseId}") && request.DependsOn != null)
            {
                if (createdCaseIds.TryGetValue(request.DependsOn, out var caseId))
                {
                    endpoint = endpoint.Replace("{caseId}", caseId);
                }
                else
                {
                    result.Error = $"Dependent request '{request.DependsOn}' did not create a case ID";
                    return result;
                }
            }

            var url = $"{config.ApiBaseUrl.TrimEnd('/')}{endpoint}";

            // Create HTTP request
            var httpRequest = new HttpRequestMessage(new HttpMethod(request.Method), url);
            httpRequest.Headers.Add("Authorization", $"Bearer {token}");

            // Add payload for POST/PUT/PATCH requests
            if (request.Payload.HasValue && (request.Method == "POST" || request.Method == "PUT" || request.Method == "PATCH"))
            {
                var payloadJson = JsonSerializer.Serialize(request.Payload.Value, jsonOptions);
                httpRequest.Content = new StringContent(payloadJson, Encoding.UTF8, "application/json");

                if (verbose)
                {
                    Console.WriteLine($"  📤 Payload: {payloadJson}");
                }
            }

            if (verbose)
            {
                Console.WriteLine($"  🌐 {request.Method} {url}");
            }

            // Execute request
            var response = await httpClient.SendAsync(httpRequest);
            var responseContent = await response.Content.ReadAsStringAsync();

            stopwatch.Stop();
            result.Duration = stopwatch.Elapsed;
            result.StatusCode = (int)response.StatusCode;
            result.Response = responseContent;
            result.Success = result.StatusCode == request.ExpectedStatus;

            // Extract case ID from response if this was a case creation
            if (request.Method == "POST" && request.Endpoint == "/cases" && result.Success)
            {
                try
                {
                    var responseJson = JsonSerializer.Deserialize<JsonElement>(responseContent);
                    if (responseJson.TryGetProperty("id", out var idElement))
                    {
                        result.CreatedCaseId = idElement.GetString();
                    }
                }
                catch
                {
                    // Ignore JSON parsing errors for case ID extraction
                }
            }

            if (!result.Success)
            {
                result.Error = $"Expected status {request.ExpectedStatus}, got {result.StatusCode}";
            }
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            result.Duration = stopwatch.Elapsed;
            result.Error = ex.Message;
            result.Success = false;
        }

        return result;
    }
}