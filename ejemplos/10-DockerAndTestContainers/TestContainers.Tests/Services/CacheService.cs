using StackExchange.Redis;

namespace TestContainers.Tests.Services;

/// <summary>
/// Servicio de caché usando Redis
/// Similar a Spring Cache en Java
/// </summary>
public class CacheService
{
    private readonly IDatabase _database;

    public CacheService(string connectionString)
    {
        var connection = ConnectionMultiplexer.Connect(connectionString);
        _database = connection.GetDatabase();
    }

    public async Task<bool> GuardarAsync(string key, string value, TimeSpan? expiration = null)
    {
        return await _database.StringSetAsync(key, value, expiration);
    }

    public async Task<string?> ObtenerAsync(string key)
    {
        var value = await _database.StringGetAsync(key);
        return value.HasValue ? value.ToString() : null;
    }

    public async Task<bool> ExisteAsync(string key)
    {
        return await _database.KeyExistsAsync(key);
    }

    public async Task<bool> EliminarAsync(string key)
    {
        return await _database.KeyDeleteAsync(key);
    }

    public async Task<long> IncrementarAsync(string key)
    {
        return await _database.StringIncrementAsync(key);
    }

    public async Task<bool> GuardarHashAsync(string key, Dictionary<string, string> valores)
    {
        var entries = valores.Select(kvp => new HashEntry(kvp.Key, kvp.Value)).ToArray();
        await _database.HashSetAsync(key, entries);
        return true;
    }

    public async Task<Dictionary<string, string>> ObtenerHashAsync(string key)
    {
        var entries = await _database.HashGetAllAsync(key);
        return entries.ToDictionary(
            entry => entry.Name.ToString(),
            entry => entry.Value.ToString()
        );
    }

    public async Task LimpiarAsync()
    {
        var endpoints = _database.Multiplexer.GetEndPoints();
        foreach (var endpoint in endpoints)
        {
            var server = _database.Multiplexer.GetServer(endpoint);
            await server.FlushDatabaseAsync();
        }
    }
}
