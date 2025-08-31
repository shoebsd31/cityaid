using System.Data.Common;
using System.Security.Claims;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

namespace CityAid.Api.Infrastructure;

public sealed class SessionContextInterceptor : DbConnectionInterceptor
{
    private readonly IHttpContextAccessor _http;
    private readonly ILogger<SessionContextInterceptor> _logger;

    public SessionContextInterceptor(IHttpContextAccessor http, ILogger<SessionContextInterceptor> logger)
    {
        _http = http;
        _logger = logger;
    }

    private (string? City, string? Team, string? Role) ExtractContext()
    {
        var user = _http.HttpContext?.User;
        if (user is null || !user.Identity?.IsAuthenticated == true) return (null, null, null);

        // Expect these in JWT custom claims mapped from Entra app roles or extension claims
        var city = user.FindFirstValue("city");
        var team = user.FindFirstValue("team");
        var role = user.FindFirstValue(ClaimTypes.Role) ?? user.FindFirstValue("role");
        return (city, team, role);
    }

    public override async Task ConnectionOpenedAsync(
        DbConnection connection,
        ConnectionEndEventData eventData,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var (city, team, role) = ExtractContext();
            if (string.IsNullOrWhiteSpace(city) || string.IsNullOrWhiteSpace(team) || string.IsNullOrWhiteSpace(role))
                return;

            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"EXEC sys.sp_set_session_context @key=N'city', @value=@p_city, @read_only=1;
                                EXEC sys.sp_set_session_context @key=N'team', @value=@p_team, @read_only=1;
                                EXEC sys.sp_set_session_context @key=N'role', @value=@p_role, @read_only=1;";
            var pCity = cmd.CreateParameter(); pCity.ParameterName = "@p_city"; pCity.Value = city;
            var pTeam = cmd.CreateParameter(); pTeam.ParameterName = "@p_team"; pTeam.Value = team;
            var pRole = cmd.CreateParameter(); pRole.ParameterName = "@p_role"; pRole.Value = role;
            cmd.Parameters.Add(pCity); cmd.Parameters.Add(pTeam); cmd.Parameters.Add(pRole);
            await cmd.ExecuteNonQueryAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to set SQL SESSION_CONTEXT");
        }
    }
}
