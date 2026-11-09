using System.Data;
using System.Globalization;
using Dapper;

namespace SluTentasal.Web.Data;

/// <summary>
/// SQLite saknar datumtyp. Datum lagras som TEXT i formatet yyyy-MM-dd HH:mm:ss
/// så att strängjämförelser i SQL ger rätt ordning.
/// </summary>
public sealed class SqliteDateTimeHandler : SqlMapper.TypeHandler<DateTime>
{
    public const string Format = "yyyy-MM-dd HH:mm:ss";

    public override void SetValue(IDbDataParameter parameter, DateTime value)
    {
        parameter.DbType = DbType.String;
        parameter.Value = value.ToString(Format, CultureInfo.InvariantCulture);
    }

    public override DateTime Parse(object value) =>
        DateTime.Parse((string)value, CultureInfo.InvariantCulture);
}
