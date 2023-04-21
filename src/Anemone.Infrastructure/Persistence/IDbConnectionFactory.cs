using System.Data;

namespace Anemone.Infrastructure.Persistence;

public interface IDbConnectionFactory
{
    IDbConnection CreateSqliteConnection(string connectionString);
}