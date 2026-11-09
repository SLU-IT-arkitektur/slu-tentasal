using Dapper;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using SluTentasal.Web.Data;

namespace SluTentasal.Web.Health;

/// <summary>Kontrollerar att databasen går att nå och att tabellen Passwords finns.</summary>
public sealed class DatabaseHealthCheck(IDbConnectionFactory connectionFactory, ILogger<DatabaseHealthCheck> logger) : IHealthCheck
{
    // Portabel fråga som inte läser några rader men kräver att tabellen och kolumnerna finns.
    private const string Sql = "SELECT COUNT(*) FROM Passwords WHERE 1 = 0 AND campus IS NULL AND password IS NULL AND starttime IS NULL";

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var connection = connectionFactory.Create();
            await connection.OpenAsync(cancellationToken);
            await connection.ExecuteScalarAsync<long>(new CommandDefinition(Sql, cancellationToken: cancellationToken));
            return HealthCheckResult.Healthy("Databasen svarar och tabellen Passwords finns.");
        }
        catch (Exception ex) when (ex is not OperationCanceledException || !cancellationToken.IsCancellationRequested)
        {
            // Felmeddelandet loggas men exponeras inte i svaret, eftersom det kan innehålla servernamn m.m.
            logger.LogError(ex, "Databaskontrollen misslyckades.");
            return HealthCheckResult.Unhealthy("Databasen går inte att nå, eller så saknas tabellen Passwords.");
        }
    }
}
