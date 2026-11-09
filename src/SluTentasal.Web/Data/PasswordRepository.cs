using Dapper;
using Microsoft.Extensions.Options;
using SluTentasal.Web.Options;

namespace SluTentasal.Web.Data;

public sealed record ActivePassword(string Password, DateTime StartTime);

public sealed class PasswordRepository(IDbConnectionFactory connectionFactory, IOptions<TentasalOptions> options)
{
    // SQL:en är portabel mellan SQLite, SQL Server och PostgreSQL:
    // tidsfönstret räknas ut i C# och inga TOP/LIMIT/DATEADD används.
    private const string ActiveSql = """
        SELECT password AS Password, starttime AS StartTime
        FROM Passwords
        WHERE campus = @campus AND starttime <= @upper AND starttime > @lower
        ORDER BY starttime
        """;

    private const string NextSql = """
        SELECT starttime
        FROM Passwords
        WHERE campus = @campus AND starttime > @upper AND starttime < @dayEnd
        ORDER BY starttime
        """;

    /// <summary>
    /// Lösenordet visas när now ∈ [start − before, start + after),
    /// vilket är detsamma som start ∈ (now − after, now + before].
    /// </summary>
    public async Task<ActivePassword?> GetActiveAsync(string campus, DateTime now)
    {
        var (lower, upper) = Window(now);
        await using var connection = connectionFactory.Create();
        return await connection.QueryFirstOrDefaultAsync<ActivePassword>(ActiveSql, new { campus, lower, upper });
    }

    /// <summary>Nästa tenta som startar senare i dag. Tentor andra dagar visas inte på TV:n.</summary>
    public async Task<DateTime?> GetNextStartTodayAsync(string campus, DateTime now)
    {
        var (_, upper) = Window(now);
        var dayEnd = now.Date.AddDays(1);
        await using var connection = connectionFactory.Create();
        return await connection.QueryFirstOrDefaultAsync<DateTime?>(NextSql, new { campus, upper, dayEnd });
    }

    private (DateTime Lower, DateTime Upper) Window(DateTime now) =>
        (now.AddMinutes(-options.Value.ShowAfterStartMinutes),
         now.AddMinutes(options.Value.ShowBeforeStartMinutes));
}
