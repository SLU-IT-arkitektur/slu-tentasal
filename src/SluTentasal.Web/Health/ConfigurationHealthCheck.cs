using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using SluTentasal.Web.Options;

namespace SluTentasal.Web.Health;

/// <summary>Kontrollerar att inställningarna i sektionen Tentasal är rimliga.</summary>
public sealed class ConfigurationHealthCheck(IOptionsMonitor<TentasalOptions> options) : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        // IOptionsMonitor ger aktuella värden, så felaktiga ändringar i appsettings.json syns här direkt.
        var value = options.CurrentValue;
        var problems = new List<string>();

        if (value.ShowBeforeStartMinutes < 0)
        {
            problems.Add("ShowBeforeStartMinutes får inte vara negativ.");
        }

        if (value.ShowAfterStartMinutes < 0)
        {
            problems.Add("ShowAfterStartMinutes får inte vara negativ.");
        }

        if (value.PasswordPollSeconds < 1)
        {
            problems.Add("PasswordPollSeconds måste vara minst 1.");
        }

        if (!TimeZoneInfo.TryFindSystemTimeZoneById(value.TimeZone, out _))
        {
            problems.Add($"Tidszonen '{value.TimeZone}' finns inte på servern.");
        }

        return Task.FromResult(problems.Count == 0
            ? HealthCheckResult.Healthy("Konfigurationen är giltig.")
            : HealthCheckResult.Unhealthy(string.Join(" ", problems)));
    }
}
