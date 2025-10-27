using MongoDB.Driver;
using TiendaApi.Data;
using TiendaApi.Models.Entities;

namespace TiendaApi.Repositories;

/// <summary>
/// Repository implementation for Pedidos using MongoDB
/// </summary>
public class PedidosRepository : IPedidosRepository
{
    private readonly IMongoCollection<Pedido> _pedidos;
    private readonly ILogger<PedidosRepository> _logger;

    public PedidosRepository(MongoDbContext context, ILogger<PedidosRepository> logger)
    {
        _pedidos = context.GetCollection<Pedido>("pedidos");
        _logger = logger;
    }

    public async Task<Pedido> SaveAsync(Pedido pedido)
    {
        _logger.LogInformation("Saving pedido for user {UserId}", pedido.UserId);
        await _pedidos.InsertOneAsync(pedido);
        return pedido;
    }

    public async Task<Pedido?> FindByIdAsync(string id)
    {
        _logger.LogDebug("Finding pedido by ID: {Id}", id);
        return await _pedidos.Find(p => p.Id == id).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Pedido>> FindAllAsync()
    {
        _logger.LogDebug("Finding all pedidos");
        return await _pedidos.Find(_ => true).ToListAsync();
    }

    public async Task<IEnumerable<Pedido>> FindByUserIdAsync(long userId)
    {
        _logger.LogDebug("Finding pedidos for user {UserId}", userId);
        return await _pedidos.Find(p => p.UserId == userId).ToListAsync();
    }
}
