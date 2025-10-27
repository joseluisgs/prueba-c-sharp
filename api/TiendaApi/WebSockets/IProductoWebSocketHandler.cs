namespace TiendaApi.WebSockets;

/// <summary>
/// Interface for WebSocket handler for managing producto notification connections
/// </summary>
public interface IProductoWebSocketHandler
{
    /// <summary>
    /// Notify all connected clients about producto creation
    /// </summary>
    Task NotifyProductoCreatedAsync(long productoId, string productoNombre, object? data = null);

    /// <summary>
    /// Notify all connected clients about producto update
    /// </summary>
    Task NotifyProductoUpdatedAsync(long productoId, string productoNombre, object? data = null);

    /// <summary>
    /// Notify all connected clients about producto deletion
    /// </summary>
    Task NotifyProductoDeletedAsync(long productoId, string productoNombre);
}
