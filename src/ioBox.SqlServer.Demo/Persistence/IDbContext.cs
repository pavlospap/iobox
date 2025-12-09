using System.Data;

namespace IOBox.SqlServer.Demo.Persistence;

internal interface IDbContext
{
    IDbConnection CreateConnection();
}
