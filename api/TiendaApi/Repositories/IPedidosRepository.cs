using TiendaApi.Models.Entities;

namespace TiendaApi.Repositories;

/// <summary>
/// Repository interface for Pedidos (MongoDB)
/// </summary>
public interface IPedidosRepository
{
    Task<Pedido> SaveAsync(Pedido pedido);
    Task<Pedido?> FindByIdAsync(string id);
    Task<IEnumerable<Pedido>> FindAllAsync();
    Task<IEnumerable<Pedido>> FindByUserIdAsync(long userId);
}
