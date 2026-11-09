namespace SluTentasal.Web.Options;

public enum DatabaseProvider
{
    Sqlite,
    SqlServer,
    Postgres
}

public sealed class DatabaseOptions
{
    public const string SectionName = "Database";

    public DatabaseProvider Provider { get; set; } = DatabaseProvider.SqlServer;
}
