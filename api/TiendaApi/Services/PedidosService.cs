using AutoMapper;
using TiendaApi.Models.DTOs;
using TiendaApi.Models.Entities;
using TiendaApi.Repositories;
using TiendaApi.Services.Cache;
using TiendaApi.Services.Email;
using TiendaApi.WebSockets;

namespace TiendaApi.Services;

/// <summary>
/// Service for managing Pedidos with stock reduction, caching, email notifications
/// </summary>
public class PedidosService
{
    private readonly IPedidosRepository _pedidosRepository;
    private readonly IProductoRepository _productoRepository;
    private readonly ICacheService _cacheService;
    private readonly IEmailService _emailService;
    private readonly IProductoWebSocketHandler _webSocketHandler;
    private readonly IMapper _mapper;
    private readonly ILogger<PedidosService> _logger;

    public PedidosService(
        IPedidosRepository pedidosRepository,
        IProductoRepository productoRepository,
        ICacheService cacheService,
        IEmailService emailService,
        IProductoWebSocketHandler webSocketHandler,
        IMapper mapper,
        ILogger<PedidosService> logger)
    {
        _pedidosRepository = pedidosRepository;
        _productoRepository = productoRepository;
        _cacheService = cacheService;
        _emailService = emailService;
        _webSocketHandler = webSocketHandler;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Create a new pedido, reduce stock, cache, and send email notification
    /// </summary>
    public async Task<PedidoDto> CreatePedidoAsync(long userId, PedidoRequestDto request)
    {
        _logger.LogInformation("Creating pedido for user {UserId}", userId);

        // Build pedido items and calculate total
        var items = new List<PedidoItem>();
        decimal total = 0;

        foreach (var itemRequest in request.Items)
        {
            // Find producto
            var producto = await _productoRepository.FindByIdAsync(itemRequest.ProductoId);
            if (producto == null)
            {
                throw new InvalidOperationException($"Producto {itemRequest.ProductoId} not found");
            }

            // Check stock
            if (producto.Stock < itemRequest.Cantidad)
            {
                throw new InvalidOperationException($"Insufficient stock for producto {producto.Nombre}");
            }

            // Reduce stock
            producto.Stock -= itemRequest.Cantidad;
            producto.UpdatedAt = DateTime.UtcNow;
            await _productoRepository.SaveAsync(producto);

            _logger.LogInformation(
                "Reduced stock for producto {ProductoId} by {Cantidad}", 
                producto.Id, 
                itemRequest.Cantidad);

            // Build item
            var subtotal = producto.Precio * itemRequest.Cantidad;
            items.Add(new PedidoItem
            {
                ProductoId = producto.Id,
                NombreProducto = producto.Nombre,
                Cantidad = itemRequest.Cantidad,
                Precio = producto.Precio,
                Subtotal = subtotal
            });

            total += subtotal;
        }

        // Create pedido
        var pedido = new Pedido
        {
            UserId = userId,
            Items = items,
            Total = total,
            Estado = PedidoEstado.PENDIENTE,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Save to repository
        var savedPedido = await _pedidosRepository.SaveAsync(pedido);

        _logger.LogInformation("Pedido {PedidoId} saved successfully", savedPedido.Id);

        // Enqueue email notification
        var emailMessage = new EmailMessage
        {
            To = $"user{userId}@example.com",
            Subject = "Pedido confirmado",
            Body = $"Su pedido {savedPedido.Id} ha sido confirmado. Total: {savedPedido.Total:C}",
            IsHtml = false
        };

        await _emailService.EnqueueEmailAsync(emailMessage);

        _logger.LogInformation("Email notification queued for pedido {PedidoId}", savedPedido.Id);

        // Return DTO
        return _mapper.Map<PedidoDto>(savedPedido);
    }

    /// <summary>
    /// Get pedido by ID
    /// </summary>
    public async Task<PedidoDto?> GetPedidoByIdAsync(string id)
    {
        _logger.LogDebug("Getting pedido {PedidoId}", id);
        var pedido = await _pedidosRepository.FindByIdAsync(id);
        return pedido != null ? _mapper.Map<PedidoDto>(pedido) : null;
    }

    /// <summary>
    /// Get all pedidos for a user
    /// </summary>
    public async Task<IEnumerable<PedidoDto>> GetPedidosByUserIdAsync(long userId)
    {
        _logger.LogDebug("Getting pedidos for user {UserId}", userId);
        var pedidos = await _pedidosRepository.FindByUserIdAsync(userId);
        return _mapper.Map<IEnumerable<PedidoDto>>(pedidos);
    }
}
