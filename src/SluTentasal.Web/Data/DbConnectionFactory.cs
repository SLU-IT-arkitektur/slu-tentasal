using System.Data.Common;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using Npgsql;
using SluTentasal.Web.Options;

namespace SluTentasal.Web.Data;

public sealed class DbConnectionFactory(DatabaseProvider provider, string connectionString) : IDbConnectionFactory
{
    public DbConnection Create() => provider switch
    {
        DatabaseProvider.Sqlite => new SqliteConnection(connectionString),
        DatabaseProvider.SqlServer => new SqlConnection(connectionString),
        DatabaseProvider.Postgres => new NpgsqlConnection(connectionString),
        _ => throw new InvalidOperationException($"Okänd databasprovider: {provider}")
    };
}
