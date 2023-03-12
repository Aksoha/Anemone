using System.Data;
using Microsoft.Data.Sqlite;

namespace Anemone.Repository;

internal class DbConnectionFactory : IDbConnectionFactory
{
    public IDbConnection CreateSqliteConnection(string connectionString)
    {
        var connection = new SqliteConnection(connectionString);
        return connection;
    }
}