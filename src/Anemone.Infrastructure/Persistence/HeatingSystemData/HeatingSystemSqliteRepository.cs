using System.Data;
using System.Diagnostics;
using Anemone.Core.Common.Entities;
using Anemone.Core.Persistence;
using Anemone.Core.Persistence.HeatingSystem;
using Dapper;
using Microsoft.Data.Sqlite;

namespace Anemone.Infrastructure.Persistence.HeatingSystemData;

public class HeatingSystemSqliteRepository : IHeatingSystemRepository
{
    public HeatingSystemSqliteRepository(IDbConnectionFactory connectionFactory, RepositoryOptions options)
    {
        ConnectionFactory = connectionFactory;
        Options = options;
    }

    private IDbConnectionFactory ConnectionFactory { get; }
    private RepositoryOptions Options { get; }

    public async Task<bool> Exists(int id)
    {
        using var connection = GetConnection();
        connection.Open();
        try
        {
            const string sql = "SELECT 1 FROM HeatingSystem WHERE Id = @Id";
            var exists = await connection.QuerySingleOrDefaultAsync<bool>(sql, new { Id = id });
            return exists;
        }
        catch (SqliteException e)
        {
            throw new RepositoryReadException(connection.Database, e);
        }
    }

    public async Task Create(Core.Common.Entities.HeatingSystem data)
    {
        using var connection = GetConnection();
        connection.Open();
        using var tr = connection.BeginTransaction();

        try
        {
            var creationDate = DateTime.UtcNow;
            var parameters = new { data.Name, CreationDate = creationDate };

            const string sql = @"
insert into HeatingSystem (Name, CreationDate) VALUES (@Name, @CreationDate);
select last_insert_rowid()";

            var id = await connection.QuerySingleAsync<int>(sql, parameters);

            foreach (var point in data.HeatingSystemPoints)
            {
                await AddPoint(connection, point, id);
                point.HeatingSystem = data;
            }

            data.CreationDate = creationDate;
            data.Id = id;
            tr.Commit();
        }
        catch (SqliteException e)
        {
            tr.Rollback();
            data.Id = null;
            data.CreationDate = null;
            data.HeatingSystemPoints.ForEach(x =>
            {
                x.Id = null;
                x.HeatingSystemId = null;
                x.HeatingSystem = null;
            });
            throw new RepositoryWriteException(connection.Database, e);
        }
    }

    public async Task<Core.Common.Entities.HeatingSystem?> Get(int id)
    {
        using var connection = GetConnection();
        connection.Open();

        try
        {
            var parameters = new { Id = id };
            const string sql = "select Id, CreationDate, Name from HeatingSystem where Id = @Id";
            var heatingSystemData = await connection.QuerySingleOrDefaultAsync<Core.Common.Entities.HeatingSystem>(sql, parameters);

            if (heatingSystemData is null)
                return null;


            heatingSystemData.HeatingSystemPoints = (await GetPoints(connection, id)).ToList();

            heatingSystemData.HeatingSystemPoints.ForEach(x =>
            {
                x.HeatingSystemId = (int)heatingSystemData.Id!;
                x.HeatingSystem = heatingSystemData;
            });

            return heatingSystemData;
        }
        catch (SqliteException e)
        {
            throw new RepositoryReadException(connection.Database, e);
        }
    }

    public async Task<IEnumerable<Core.Common.Entities.HeatingSystem>> GetAll()
    {
        using var connection = GetConnection();
        connection.Open();

        try
        {
            const string sql = "select Id, CreationDate, Name from HeatingSystem";
            var heatingSystems = (await connection.QueryAsync<Core.Common.Entities.HeatingSystem>(sql)).ToList();

            if (heatingSystems.Count == 0)
                return heatingSystems;

            foreach (var heatingSystem in heatingSystems)
            {
                if (heatingSystem.Id is null)
                    throw new UnreachableException("Id of heating system is not mapped");

                heatingSystem.HeatingSystemPoints = (await GetPoints(connection, (int)heatingSystem.Id)).ToList();

                heatingSystem.HeatingSystemPoints.ForEach(x => { x.HeatingSystem = heatingSystem; });
            }

            return heatingSystems;
        }
        catch (SqliteException e)
        {
            throw new RepositoryReadException(connection.Database, e);
        }
    }

    public async Task Update(Core.Common.Entities.HeatingSystem data)
    {
        using var connection = GetConnection();
        connection.Open();
        using var tr = connection.BeginTransaction();

        try
        {
            const string sql =
                "update HeatingSystem Set Name = @Name, ModificationDate = @ModificationDate where Id = @Id";

            var parameters = new { data.Name, data.Id, ModificationDate = DateTime.UtcNow };
            await connection.ExecuteAsync(sql, parameters);

            await UpdatePoints(connection, data);

            tr.Commit();
        }
        catch (SqliteException e)
        {
            tr.Rollback();
            throw new RepositoryReadException(connection.Database, e);
        }
    }

    public async Task Delete(Core.Common.Entities.HeatingSystem data)
    {
        var connection = GetConnection();
        connection.Open();
        using var tr = connection.BeginTransaction();
        try
        {
            var parameters = new { data.Id };
            const string sql =
                "delete from HeatingSystem where Id = @Id; delete from HeatingSystemPoint where HeatingSystemId = @Id";
            await connection.ExecuteAsync(sql, parameters);

            tr.Commit();
        }
        catch (SqliteException e)
        {
            tr.Rollback();
            throw new RepositoryWriteException(connection.Database, e);
        }

        data.Id = null;
        data.CreationDate = null;
        foreach (var point in data.HeatingSystemPoints)
        {
            point.Id = null;
            point.HeatingSystem = null;
        }
    }

    public async Task<IEnumerable<HeatingSystemName>> GetAllNames()
    {
        using var connection = GetConnection();
        connection.Open();

        try
        {
            const string sql = "select Id, Name from HeatingSystem";
            return await connection.QueryAsync<HeatingSystemName>(sql);
        }
        catch (SqliteException e)
        {
            throw new RepositoryReadException(connection.Database, e);
        }
    }

    private IDbConnection GetConnection()
    {
        return ConnectionFactory.CreateSqliteConnection(Options.ConnectionString);
    }

    private static async Task<IEnumerable<HeatingSystemPoint>> GetPoints(IDbConnection connection, int heatingSystemId)
    {
        const string sql =
            "select Id, Type, TypeValue, Resistance, Inductance, HeatingSystemId from HeatingSystemPoint where HeatingSystemId = @HeatingSystemId";
        var parameters = new { HeatingSystemId = heatingSystemId };
        return await connection.QueryAsync<HeatingSystemPoint>(sql, parameters);
    }

    private static async Task UpdatePoints(IDbConnection connection, Core.Common.Entities.HeatingSystem heatingSystem)
    {
        if (heatingSystem.Id is null)
            throw new ArgumentNullException(nameof(heatingSystem.Id),
                "Id of the heating system must be set before calling this method");


        const string sql = "select Id From HeatingSystemPoint where HeatingSystemId = @Id";
        var parameters = new { heatingSystem.Id };

        var pointIds = await connection.QueryAsync<int>(sql, parameters);

        var pointsToDelete = pointIds.ExceptBy(
            heatingSystem.HeatingSystemPoints.Select(x => x.Id),
            x => x);


        // delete points
        foreach (var point in pointsToDelete) await DeletePoint(connection, point);

        // update existing points
        var pointsToAddOrUpdate = heatingSystem.HeatingSystemPoints;
        foreach (var point in pointsToAddOrUpdate)
            if (point.Id is null)
            {
                await AddPoint(connection, point, (int)heatingSystem.Id);
                point.HeatingSystem = heatingSystem;
            }
            else
            {
                await UpdatePoint(connection, point);
            }
    }

    private static async Task UpdatePoint(IDbConnection connection, HeatingSystemPoint point)
    {
        const string sql = @"
update HeatingSystemPoint
Set Type = @Type, TypeValue = @TypeValue, Resistance = @Resistance, Inductance = @Inductance
where Id = @Id";
        var parameters = new
        {
            point.Id,
            point.Type,
            point.TypeValue,
            point.Resistance,
            point.Inductance
        };
        await connection.ExecuteAsync(sql, parameters);
    }

    private static async Task DeletePoint(IDbConnection connection, int id)
    {
        const string sql = "delete from HeatingSystemPoint where Id = @Id";
        var parameters = new { Id = id };
        await connection.ExecuteAsync(sql, parameters);
    }

    private static async Task AddPoint(IDbConnection connection, HeatingSystemPoint point, int heatingSystemId)
    {
        const string sql = @"
insert into HeatingSystemPoint (Type, TypeValue, Resistance, Inductance, HeatingSystemId) 
VALUES (@Type, @TypeValue, @Resistance, @Inductance, @HeatingSystemId);
SELECT last_insert_rowid()";

        var parameters = new
        {
            point.Type,
            point.TypeValue,
            point.Resistance,
            point.Inductance,
            HeatingSystemId = heatingSystemId
        };

        var pointId = await connection.QuerySingleAsync<int>(sql, parameters);

        point.Id = pointId;
        point.HeatingSystemId = heatingSystemId;
    }
}