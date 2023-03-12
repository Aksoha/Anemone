using System.Data;
using System.Diagnostics.CodeAnalysis;
using Bogus;
using Dapper;

namespace Anemone.Repository.Tests.HeatingSystemData;

[SuppressMessage("ReSharper", "ParameterOnlyUsedForPreconditionCheck.Local")]
public class HeatingSystemSqliteRepositoryTests : IClassFixture<DatabaseFixture>, IDisposable
{
    private readonly DatabaseFixture _fixture;

    private readonly Faker<HeatingSystemTestModel> _testHeatingSystem;
    private readonly Faker<HeatingSystemPointTestModel> _testHeatingSystemPoint;

    public HeatingSystemSqliteRepositoryTests(DatabaseFixture fixture)
    {
        _fixture = fixture;

        _testHeatingSystemPoint = new Faker<HeatingSystemPointTestModel>()
            .RuleFor(o => o.Type, f => f.Random.Enum<HeatingSystemPointType>())
            .RuleFor(o => o.TypeValue, f => f.Random.Double(0, 300e3))
            .RuleFor(o => o.Resistance, f => f.Random.Double(0, 100e-3))
            .RuleFor(o => o.Inductance, f => f.Random.Double(0, 50e-6));

        _testHeatingSystem = new Faker<HeatingSystemTestModel>()
            .RuleFor(o => o.Name, f => f.Commerce.ProductName())
            .RuleFor(o => o.HeatingSystemPoints, _ => Generate());

        List<HeatingSystemPoint> Generate(int count = 10)
        {
            var points = _testHeatingSystemPoint.Generate(count);
            return points.Cast<HeatingSystemPoint>().ToList();
        }
    }

    private HeatingSystemSqliteRepository Repository => new(_fixture.DbConnectionFactory, _fixture.Options);


    public void Dispose()
    {
        var connection = _fixture.DbConnectionFactory.CreateSqliteConnection(_fixture.Options.ConnectionString);
        connection.Open();
        connection.Execute("delete from HeatingSystem where true");
        connection.Execute("delete from HeatingSystemPoint where true");
    }


    [Fact]
    public async Task Create_WhenDataIsValid()
    {
        // arrange
        var model = _testHeatingSystem.Generate();


        // act
        await Repository.Create(model);


        // assert

        // check model assignment
        Assert.NotNull(model.Id);
        Assert.NotNull(model.CreationDate);
        Assert.All(model.HeatingSystemPoints, point =>
        {
            Assert.NotNull(point.Id);
            Assert.Same(point.HeatingSystem, model);
            Assert.Equal(model.Id, point.HeatingSystemId);
        });
        Assert.True(model.HeatingSystemPoints.DistinctBy(x => x.Id).Count() == model.HeatingSystemPoints.Count,
            "heating system points should have unique Id");


        // check if model was properly inserted into db
        var actualHeatingSystem = await GetConnection()
            .QuerySingleAsync<HeatingSystem>("select * from HeatingSystem where Id = @Id",
                new { model.Id });


        Assert.Equal(model.CreationDate, actualHeatingSystem.CreationDate);
        Assert.Equal(model.Name, actualHeatingSystem.Name);

        var actualPoints = (await GetConnection().QueryAsync<HeatingSystemPoint>(
            "select * from HeatingSystemPoint where HeatingSystemId = @HeatingSystemId",
            new { HeatingSystemId = model.Id })).ToList();


        Assert.Equal(model.HeatingSystemPoints.Count, actualPoints.Count);
        Assert.All(model.HeatingSystemPoints, expectedPoint =>
        {
            var actualPoint = actualPoints.Single(x => x.Id == expectedPoint.Id);
            Assert.Equal(expectedPoint.HeatingSystemId, actualPoint.HeatingSystemId);
            Assert.Equal(expectedPoint.Type, actualPoint.Type);
            Assert.Equal(expectedPoint.TypeValue, actualPoint.TypeValue);
            Assert.Equal(expectedPoint.Resistance, actualPoint.Resistance);
            Assert.Equal(expectedPoint.Inductance, actualPoint.Inductance);
        });
    }

    [Fact]
    public async Task Get_WhenDataIsValid()
    {
        // arrange
        var model = _testHeatingSystem.Generate();

        await InsertHeatingSystemIntoDb(DateTime.UtcNow, model);
        var expectedPoints = model.HeatingSystemPoints;


        // act
        var actual = await Repository.Get((int)model.Id!);

        // assert
        Assert.NotNull(actual);
        Assert.Equal(model.Id, actual.Id);
        Assert.Equal(model.Name, actual.Name);
        Assert.Equal(model.CreationDate, actual.CreationDate);
        Assert.Equal(expectedPoints.Count, actual.HeatingSystemPoints.Count);

        Assert.All(expectedPoints, expectedPoint =>
        {
            var actualPoint = actual.HeatingSystemPoints.Single(x => x.Id == expectedPoint.Id);
            Assert.Equal(expectedPoint.HeatingSystemId, actualPoint.HeatingSystemId);
            Assert.NotNull(actualPoint.HeatingSystem);
            Assert.Equal(model.Id, actualPoint.HeatingSystem.Id);
            Assert.Equal(expectedPoint.HeatingSystemId, actualPoint.HeatingSystemId);
            Assert.Equal(expectedPoint.Type, actualPoint.Type);
            Assert.Equal(expectedPoint.TypeValue, actualPoint.TypeValue);
            Assert.Equal(expectedPoint.Resistance, actualPoint.Resistance);
            Assert.Equal(expectedPoint.Inductance, actualPoint.Inductance);
        });
    }

    [Fact]
    public async Task Get_WhenDoesNotExist()
    {
        // arrange
        const int id = 12;

        // act
        var actual = await Repository.Get(id);

        // assert
        Assert.Null(actual);
    }

    [Fact]
    public async Task GetAll()
    {
        // arrange
        var testedModels = _testHeatingSystem.Generate(5);
        foreach (var model in testedModels) await InsertHeatingSystemIntoDb(DateTime.UtcNow, model);

        // act
        var actualModels = (await Repository.GetAll()).ToList();

        // assert
        Assert.Equal(testedModels.Count, actualModels.Count);
        Assert.All(testedModels, expectedHeatingModel =>
        {
            var actualHeatingModel = actualModels.Single(x => x.Id == expectedHeatingModel.Id);
            Assert.Equal(expectedHeatingModel.Id, actualHeatingModel.Id);
            Assert.Equal(expectedHeatingModel.Name, actualHeatingModel.Name);
            Assert.Equal(expectedHeatingModel.CreationDate, actualHeatingModel.CreationDate);
            Assert.Equal(expectedHeatingModel.HeatingSystemPoints.Count, actualHeatingModel.HeatingSystemPoints.Count);

            Assert.All(expectedHeatingModel.HeatingSystemPoints, expectedPoint =>
            {
                var actualPoint = actualHeatingModel.HeatingSystemPoints.Single(x => x.Id == expectedPoint.Id);
                Assert.NotNull(expectedPoint.Id);
                Assert.NotNull(actualPoint.Id);
                Assert.Equal(expectedPoint.Id, actualPoint.Id);
                Assert.Equal(expectedPoint.Type, actualPoint.Type);
                Assert.Equal(expectedPoint.TypeValue, actualPoint.TypeValue);
                Assert.Equal(expectedPoint.Resistance, actualPoint.Resistance);
                Assert.Equal(expectedPoint.Inductance, actualPoint.Inductance);
                Assert.Equal(expectedPoint.HeatingSystemId, actualPoint.HeatingSystemId);
                Assert.NotNull(expectedPoint.HeatingSystem);
                Assert.NotNull(actualPoint.HeatingSystem);
                Assert.Equal(actualHeatingModel.Id, actualPoint.HeatingSystemId);
                Assert.Equal(actualHeatingModel.Id, actualPoint.HeatingSystem.Id);
            });
        });
    }


    [Fact]
    public async Task Update_Name()
    {
        // arrange
        var expected = _testHeatingSystem.Generate();
        await InsertHeatingSystemIntoDb(DateTime.UtcNow, expected);

        expected.Name = "new name";

        // act
        await Repository.Update(expected);


        // assert
        var actual = await GetConnection()
            .QuerySingleAsync<HeatingSystem>("select * from HeatingSystem where Id = @Id", new { expected.Id });
        Assert.Equal(expected.Name, actual.Name);
    }

    [Fact]
    public async Task Update_AddPoints()
    {
        // arrange
        var heatingSystem = _testHeatingSystem.Generate();
        await InsertHeatingSystemIntoDb(DateTime.UtcNow, heatingSystem);

        var currentPoints = heatingSystem.HeatingSystemPoints.ToList();
        var pointsToAdd = _testHeatingSystemPoint.Generate(5);
        heatingSystem.HeatingSystemPoints.AddRange(pointsToAdd);

        // act
        await Repository.Update(heatingSystem);


        // assert

        // check if points were added to the database
        var actualPoints = (await GetConnection().QueryAsync<HeatingSystemPoint>(
            "select * from HeatingSystemPoint where HeatingSystemId = @HeatingSystemId",
            new { HeatingSystemId = heatingSystem.Id })).ToList();

        var addedPointsFromDatabase =
            actualPoints.ExceptBy(currentPoints.Select(x => x.Id), point => point.Id).ToList();

        Assert.Equal(pointsToAdd.Count, addedPointsFromDatabase.Count);
        Assert.All(pointsToAdd, expectedPoint =>
        {
            var actualPoint = addedPointsFromDatabase.Single(x => x.Id == expectedPoint.Id);
            Assert.Equal(expectedPoint.Type, actualPoint.Type);
            Assert.Equal(expectedPoint.TypeValue, actualPoint.TypeValue);
            Assert.Equal(expectedPoint.Resistance, actualPoint.Resistance);
            Assert.Equal(expectedPoint.Inductance, actualPoint.Inductance);
            Assert.Equal(expectedPoint.HeatingSystemId, actualPoint.HeatingSystemId);
        });


        // check if points were assigned correctly in the heatingSystem
        Assert.All(pointsToAdd, point =>
        {
            Assert.NotNull(point.Id);
            Assert.NotNull(point.HeatingSystem);
            Assert.NotNull(point.HeatingSystemId);
            Assert.Equal(heatingSystem.Id, point.HeatingSystemId);
            Assert.Equal(heatingSystem.Id, point.HeatingSystem.Id);
        });
    }

    [Fact]
    public async Task Update_RemovePoints()
    {
        // arrange
        const int itemsToRemove = 3;
        const int pointsInHeatingSystem = 10;
        const int remainingPoints = pointsInHeatingSystem - itemsToRemove;

        var heatingSystem = _testHeatingSystem.Generate();
        heatingSystem.HeatingSystemPoints = _testHeatingSystemPoint.Generate(pointsInHeatingSystem)
            .Cast<HeatingSystemPoint>()
            .ToList();


        await InsertHeatingSystemIntoDb(DateTime.UtcNow, heatingSystem);
        var currentPoints = heatingSystem.HeatingSystemPoints.ToList();
        var pointsToRemove = currentPoints.Take(itemsToRemove).ToList();
        var idsToRemove = pointsToRemove.Select(x => x.Id).ToList();
        foreach (var point in pointsToRemove) heatingSystem.HeatingSystemPoints.Remove(point);

        // act
        await Repository.Update(heatingSystem);


        // assert


        var pointsInDb = (await GetConnection()
            .QueryAsync<int>("select Id from HeatingSystemPoint where HeatingSystemId = @HeatingSystemId",
                new { HeatingSystemId = heatingSystem.Id })).ToList();

        Assert.Equal(remainingPoints, pointsInDb.Count);
        Assert.Equal(remainingPoints, heatingSystem.HeatingSystemPoints.Count);
        Assert.DoesNotContain(pointsInDb, point => idsToRemove.Any(x => point == x));
    }

    [Fact]
    public async Task Update_ModifyPoints()
    {
        // arrange
        var heatingSystem = _testHeatingSystem.Generate();
        heatingSystem.HeatingSystemPoints = _testHeatingSystemPoint.Generate(5)
            .Cast<HeatingSystemPoint>()
            .ToList();
        await InsertHeatingSystemIntoDb(DateTime.UtcNow, heatingSystem);

        var pointToUpdate = heatingSystem.HeatingSystemPoints.First();
        var pointIdBeforeUpdate = pointToUpdate.Id;
        pointToUpdate.TypeValue = 0.18;
        pointToUpdate.Resistance = 3.12;
        pointToUpdate.Inductance = 8.15;

        // act
        await Repository.Update(heatingSystem);

        // assert
        var pointInDb = await GetConnection().QuerySingleAsync<HeatingSystemPoint>(
            "select * from HeatingSystemPoint where Id = @Id",
            new { Id = pointIdBeforeUpdate });

        Assert.Equal(pointIdBeforeUpdate, pointToUpdate.Id);
        Assert.Equal(pointToUpdate.Type, pointInDb.Type);
        Assert.Equal(pointToUpdate.TypeValue, pointInDb.TypeValue);
        Assert.Equal(pointToUpdate.Resistance, pointInDb.Resistance);
        Assert.Equal(pointToUpdate.Inductance, pointInDb.Inductance);
    }


    [Fact]
    public async Task Delete()
    {
        // arrange
        var expected = _testHeatingSystem.Generate();
        await InsertHeatingSystemIntoDb(DateTime.UtcNow, expected);
        var heatingSystemId = (int)expected.Id!;

        // act
        await Repository.Delete(expected);

        // assert


        // check if entities were deleted
        var actualHeatingSystem = await GetConnection()
            .QuerySingleOrDefaultAsync<HeatingSystem>("select * from HeatingSystem where Id = @Id",
                new { expected.Id });

        
        var actualPoints = (await GetConnection().QueryAsync<HeatingSystemPoint>(
            "select * from HeatingSystemPoint where HeatingSystemId = @HeatingSystemId",
            new { HeatingSystemId = heatingSystemId })).ToList();

        Assert.Null(actualHeatingSystem);
        Assert.Empty(actualPoints);

        // check clean up the model
        Assert.Null(expected.Id);
        Assert.Null(expected.CreationDate);
        Assert.All(expected.HeatingSystemPoints, point =>
        {
            Assert.Null(point.Id);
            Assert.Null(point.HeatingSystem);
        });
    }

    [Fact]
    public async Task GetAllNames()
    {
        // arrange
        var expectedHeatingSystems = _testHeatingSystem.Generate(5);
        foreach (var expectedItem in expectedHeatingSystems)
            expectedItem.Id = await GetConnection()
                .QuerySingleAsync<int>(
                    "insert into HeatingSystem (CreationDate, Name) VALUES (@CreationDate, @Name); select last_insert_rowid()",
                    new { CreationDate = DateTime.UtcNow, expectedItem.Name });

        // act
        var actualHeatingSystems = (await Repository.GetAllNames()).ToList();

        // assert
        Assert.Equal(expectedHeatingSystems.Count, actualHeatingSystems.Count);
        Assert.All(expectedHeatingSystems, expectedHeatingSystem =>
        {
            var actualHeatingSystem = actualHeatingSystems.Single(x => x.Id == expectedHeatingSystem.Id);
            Assert.Equal(expectedHeatingSystem.Id, actualHeatingSystem.Id);
            Assert.Equal(expectedHeatingSystem.Name, actualHeatingSystem.Name);
        });
    }


    private async Task InsertHeatingSystemIntoDb(DateTime creationDate, HeatingSystemTestModel heatingSystem)
    {
        var heatingSystemId = await GetConnection()
            .QuerySingleAsync<int>(
                "insert into HeatingSystem (CreationDate, Name) values(@CreationDate, @Name); select last_insert_rowid()",
                new { CreationDate = creationDate, heatingSystem.Name });

        heatingSystem.Id = heatingSystemId;
        heatingSystem.CreationDate = creationDate;

        await InsertPointsIntoDb(creationDate, heatingSystem, heatingSystemId);
    }

    private async Task InsertPointsIntoDb(DateTime creationDate, HeatingSystem heatingSystem, int heatingSystemId)
    {
        foreach (var point in heatingSystem.HeatingSystemPoints)
        {
            var pointId = await GetConnection().QuerySingleAsync<int>(@"
insert into HeatingSystemPoint (Type, TypeValue, Resistance, Inductance, HeatingSystemId) 
VALUES (@Type, @TypeValue, @Resistance, @Inductance, @HeatingSystemId);
SELECT last_insert_rowid()",
                new
                {
                    point.Type, point.TypeValue, point.Resistance, point.Inductance,
                    HeatingSystemId = heatingSystemId
                });

            if (point is HeatingSystemPointTestModel editablePoint)
            {
                editablePoint.Id = pointId;
                editablePoint.HeatingSystem = heatingSystem;
                editablePoint.HeatingSystemId = heatingSystemId;
            }
            else
            {
                throw new InvalidCastException("expected pointToInsert to be of type HeatingSystemPointTestModel");
            }
        }
    }

    private IDbConnection GetConnection()
    {
        return _fixture.DbConnectionFactory.CreateSqliteConnection(_fixture.Options.ConnectionString);
    }
}