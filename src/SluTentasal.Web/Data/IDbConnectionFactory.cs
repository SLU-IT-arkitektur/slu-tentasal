using System.Data.Common;

namespace SluTentasal.Web.Data;

public interface IDbConnectionFactory
{
    DbConnection Create();
}
