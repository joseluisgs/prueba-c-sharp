using NUnit.Framework;
using FluentAssertions;
using Testcontainers.PostgreSql;
using Npgsql;

namespace TestContainersDemo.Tests;

/// <summary>
/// Demonstrates TestContainers patterns for integration testing
/// Similar to TestContainers in Java
/// </summary>
[TestFixture]
public class PostgreSqlContainerTests
{
    private PostgreSqlContainer? _postgresContainer;

    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        // Create and start PostgreSQL container
        _postgresContainer = new PostgreSqlBuilder()
            .WithImage("postgres:15-alpine")
            .WithDatabase("testdb")
            .WithUsername("test")
            .WithPassword("test")
            .Build();

        await _postgresContainer.StartAsync();
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        if (_postgresContainer != null)
        {
            await _postgresContainer.StopAsync();
            await _postgresContainer.DisposeAsync();
        }
    }

    [Test]
    public async Task DeberiaConectarAPostgreSQL()
    {
        // Arrange
        var connectionString = _postgresContainer!.GetConnectionString();

        // Act
        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        // Assert
        connection.State.Should().Be(System.Data.ConnectionState.Open);
        await connection.CloseAsync();
    }

    [Test]
    public async Task DeberiaCrearYConsultarTabla()
    {
        // Arrange
        var connectionString = _postgresContainer!.GetConnectionString();
        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        // Act - Create table
        await using var createCmd = new NpgsqlCommand(
            "CREATE TABLE IF NOT EXISTS tenistas (id SERIAL PRIMARY KEY, nombre VARCHAR(100))", 
            connection);
        await createCmd.ExecuteNonQueryAsync();

        // Act - Insert data
        await using var insertCmd = new NpgsqlCommand(
            "INSERT INTO tenistas (nombre) VALUES (@nombre)", 
            connection);
        insertCmd.Parameters.AddWithValue("nombre", "Rafael Nadal");
        await insertCmd.ExecuteNonQueryAsync();

        // Act - Query data
        await using var selectCmd = new NpgsqlCommand(
            "SELECT COUNT(*) FROM tenistas", 
            connection);
        var count = (long?)await selectCmd.ExecuteScalarAsync();

        // Assert
        count.Should().Be(1);

        await connection.CloseAsync();
    }

    [Test]
    public void DeberiaExponerConnectionString()
    {
        // Arrange & Act
        var connectionString = _postgresContainer!.GetConnectionString();

        // Assert
        connectionString.Should().Contain("Host=");
        connectionString.Should().Contain("Database=testdb");
        connectionString.Should().Contain("Username=test");
    }
}
