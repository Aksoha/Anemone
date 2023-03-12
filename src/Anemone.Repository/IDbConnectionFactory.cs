using System.Data;

namespace Anemone.Repository;

public interface IDbConnectionFactory
{
    IDbConnection CreateSqliteConnection(string connectionString);
}