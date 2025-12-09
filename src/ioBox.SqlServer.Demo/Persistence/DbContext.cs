using System.Data;

using Microsoft.Data.SqlClient;

namespace IOBox.SqlServer.Demo.Persistence;

internal class DbContext(IConfiguration configuration) : IDbContext
{
    public IDbConnection CreateConnection() =>
        new SqlConnection(configuration.GetValue<string>("Database:ConnectionString"));
}
