﻿using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Anemone.Repository.Tests;

public class DatabaseFixture
{
    public DatabaseFixture()
    {
        var serviceCollection = new ServiceCollection();
        const string connectionString = "DataSource=file::memory:?cache=shared";
        Options = new RepositoryOptions(connectionString);
        serviceCollection.ConfigureDatabase(Options);
        var provider = serviceCollection.BuildServiceProvider();
        provider.UseDatabase();

        var dbConnectionFactoryMock = new Mock<IDbConnectionFactory>();
        dbConnectionFactoryMock.Setup(x => x.CreateSqliteConnection(It.IsAny<string>()))
            .Returns(new SqliteConnection(Options.ConnectionString));

        DbConnectionFactory = dbConnectionFactoryMock.Object;
    }

    public IDbConnectionFactory DbConnectionFactory { get; }
    public RepositoryOptions Options { get; }
}