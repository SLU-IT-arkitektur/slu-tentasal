using System.Globalization;
using Dapper;

namespace SluTentasal.Web.Data;

/// <summary>Skapar tabell och exempeldata i den lokala SQLite-databasen.</summary>
public static class SqliteDevSeeder
{
    public static void Seed(IDbConnectionFactory connectionFactory, DateTime now)
    {
        using var connection = connectionFactory.Create();
        connection.Open();

        connection.Execute("""
            CREATE TABLE IF NOT EXISTS Passwords (
                campus    TEXT NOT NULL,
                sal       TEXT NULL,
                password  TEXT NOT NULL,
                starttime TEXT NOT NULL
            )
            """);

        if (connection.ExecuteScalar<long>("SELECT COUNT(*) FROM Passwords") > 0)
        {
            return;
        }

        string At(int minutes) =>
            now.AddMinutes(minutes).ToString(SqliteDateTimeHandler.Format, CultureInfo.InvariantCulture);

        connection.Execute(
            "INSERT INTO Passwords (campus, sal, password, starttime) VALUES (@campus, NULL, @password, @starttime)",
            new[]
            {
                new { campus = "Ultuna", password = "Tenta-Ultuna-42", starttime = At(10) },
                new { campus = "Alnarp", password = "Tenta-Alnarp-17", starttime = At(180) },
                new { campus = "Umea", password = "Tenta-Umea-99", starttime = At(-120) }
            });
    }
}
