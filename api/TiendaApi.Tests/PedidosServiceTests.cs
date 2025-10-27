using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TiendaApi.Models.DTOs;
using TiendaApi.Models.Entities;
using TiendaApi.Repositories;
using TiendaApi.Services;
using TiendaApi.Services.Cache;
using TiendaApi.Services.Email;
using TiendaApi.WebSockets;

namespace TiendaApi.Tests;

/// <summary>
/// Unit tests for PedidosService
/// All dependencies are mocked to ensure isolated testing
/// </summary>
public class PedidosServiceTests
{
    private Mock<IPedidosRepository> _mockPedidosRepository = null!;
    private Mock<IProductoRepository> _mockProductoRepository = null!;
    private Mock<ICacheService> _mockCacheService = null!;
    private Mock<IEmailService> _mockEmailService = null!;
    private Mock<IProductoWebSocketHandler> _mockWebSocketHandler = null!;
    private Mock<IMapper> _mockMapper = null!;
    private Mock<ILogger<PedidosService>> _mockLogger = null!;
    private PedidosService _service = null!;

    [SetUp]
    public void Setup()
    {
        _mockPedidosRepository = new Mock<IPedidosRepository>();
        _mockProductoRepository = new Mock<IProductoRepository>();
        _mockCacheService = new Mock<ICacheService>();
        _mockEmailService = new Mock<IEmailService>();
        _mockWebSocketHandler = new Mock<IProductoWebSocketHandler>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILogger<PedidosService>>();

        _service = new PedidosService(
            _mockPedidosRepository.Object,
            _mockProductoRepository.Object,
            _mockCacheService.Object,
            _mockEmailService.Object,
            _mockWebSocketHandler.Object,
            _mockMapper.Object,
            _mockLogger.Object
        );
    }

    [Test]
    public async Task CreatePedidoAsync_WithValidItems_ShouldReduceStock()
    {
        // Arrange
        var userId = 1L;
        var productoId = 100L;
        var initialStock = 10;
        var cantidad = 3;

        var producto = new Producto
        {
            Id = productoId,
            Nombre = "Test Product",
            Precio = 100.00m,
            Stock = initialStock,
            CategoriaId = 1
        };

        var request = new PedidoRequestDto
        {
            Items = new List<PedidoItemRequestDto>
            {
                new PedidoItemRequestDto { ProductoId = productoId, Cantidad = cantidad }
            }
        };

        var savedPedido = new Pedido
        {
            Id = "507f1f77bcf86cd799439011",
            UserId = userId,
            Items = new List<PedidoItem>
            {
                new PedidoItem
                {
                    ProductoId = productoId,
                    NombreProducto = "Test Product",
                    Cantidad = cantidad,
                    Precio = 100.00m,
                    Subtotal = 300.00m
                }
            },
            Total = 300.00m,
            Estado = PedidoEstado.PENDIENTE
        };

        var pedidoDto = new PedidoDto
        {
            Id = savedPedido.Id,
            UserId = userId,
            Total = 300.00m,
            Estado = PedidoEstado.PENDIENTE,
            Items = new List<PedidoItemDto>()
        };

        _mockProductoRepository
            .Setup(r => r.FindByIdAsync(productoId))
            .ReturnsAsync(producto);

        _mockProductoRepository
            .Setup(r => r.SaveAsync(It.IsAny<Producto>()))
            .ReturnsAsync((Producto p) => p);

        _mockPedidosRepository
            .Setup(r => r.SaveAsync(It.IsAny<Pedido>()))
            .ReturnsAsync(savedPedido);

        _mockEmailService
            .Setup(e => e.EnqueueEmailAsync(It.IsAny<EmailMessage>()))
            .Returns(Task.CompletedTask);

        _mockMapper
            .Setup(m => m.Map<PedidoDto>(It.IsAny<Pedido>()))
            .Returns(pedidoDto);

        // Act
        var result = await _service.CreatePedidoAsync(userId, request);

        // Assert
        result.Should().NotBeNull();
        result.Total.Should().Be(300.00m);

        // Verify stock was reduced
        _mockProductoRepository.Verify(
            r => r.SaveAsync(It.Is<Producto>(p => p.Stock == initialStock - cantidad)),
            Times.Once);
    }

    [Test]
    public async Task CreatePedidoAsync_ShouldSavePedidoToRepository()
    {
        // Arrange
        var userId = 1L;
        var productoId = 100L;

        var producto = new Producto
        {
            Id = productoId,
            Nombre = "Test Product",
            Precio = 50.00m,
            Stock = 10,
            CategoriaId = 1
        };

        var request = new PedidoRequestDto
        {
            Items = new List<PedidoItemRequestDto>
            {
                new PedidoItemRequestDto { ProductoId = productoId, Cantidad = 2 }
            }
        };

        var savedPedido = new Pedido
        {
            Id = "507f1f77bcf86cd799439011",
            UserId = userId,
            Total = 100.00m
        };

        _mockProductoRepository.Setup(r => r.FindByIdAsync(productoId)).ReturnsAsync(producto);
        _mockProductoRepository.Setup(r => r.SaveAsync(It.IsAny<Producto>())).ReturnsAsync(producto);
        _mockPedidosRepository.Setup(r => r.SaveAsync(It.IsAny<Pedido>())).ReturnsAsync(savedPedido);
        _mockEmailService.Setup(e => e.EnqueueEmailAsync(It.IsAny<EmailMessage>())).Returns(Task.CompletedTask);
        _mockMapper.Setup(m => m.Map<PedidoDto>(It.IsAny<Pedido>())).Returns(new PedidoDto());

        // Act
        await _service.CreatePedidoAsync(userId, request);

        // Assert
        _mockPedidosRepository.Verify(
            r => r.SaveAsync(It.Is<Pedido>(p => p.UserId == userId && p.Total == 100.00m)),
            Times.Once);
    }

    [Test]
    public async Task CreatePedidoAsync_ShouldEnqueueEmailNotification()
    {
        // Arrange
        var userId = 1L;
        var productoId = 100L;

        var producto = new Producto
        {
            Id = productoId,
            Nombre = "Test Product",
            Precio = 75.00m,
            Stock = 10,
            CategoriaId = 1
        };

        var request = new PedidoRequestDto
        {
            Items = new List<PedidoItemRequestDto>
            {
                new PedidoItemRequestDto { ProductoId = productoId, Cantidad = 1 }
            }
        };

        var savedPedido = new Pedido
        {
            Id = "507f1f77bcf86cd799439011",
            UserId = userId,
            Total = 75.00m
        };

        _mockProductoRepository.Setup(r => r.FindByIdAsync(productoId)).ReturnsAsync(producto);
        _mockProductoRepository.Setup(r => r.SaveAsync(It.IsAny<Producto>())).ReturnsAsync(producto);
        _mockPedidosRepository.Setup(r => r.SaveAsync(It.IsAny<Pedido>())).ReturnsAsync(savedPedido);
        _mockEmailService.Setup(e => e.EnqueueEmailAsync(It.IsAny<EmailMessage>())).Returns(Task.CompletedTask);
        _mockMapper.Setup(m => m.Map<PedidoDto>(It.IsAny<Pedido>())).Returns(new PedidoDto());

        // Act
        await _service.CreatePedidoAsync(userId, request);

        // Assert
        _mockEmailService.Verify(
            e => e.EnqueueEmailAsync(It.Is<EmailMessage>(msg => 
                msg.To == $"user{userId}@example.com" &&
                msg.Subject.Contains("confirmado"))),
            Times.Once);
    }

    [Test]
    public async Task CreatePedidoAsync_WithInsufficientStock_ShouldThrowException()
    {
        // Arrange
        var userId = 1L;
        var productoId = 100L;

        var producto = new Producto
        {
            Id = productoId,
            Nombre = "Test Product",
            Precio = 100.00m,
            Stock = 2, // Only 2 in stock
            CategoriaId = 1
        };

        var request = new PedidoRequestDto
        {
            Items = new List<PedidoItemRequestDto>
            {
                new PedidoItemRequestDto { ProductoId = productoId, Cantidad = 5 } // Requesting 5
            }
        };

        _mockProductoRepository.Setup(r => r.FindByIdAsync(productoId)).ReturnsAsync(producto);

        // Act & Assert
        var act = async () => await _service.CreatePedidoAsync(userId, request);
        
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Insufficient stock*");

        // Verify stock was not saved
        _mockProductoRepository.Verify(r => r.SaveAsync(It.IsAny<Producto>()), Times.Never);
        _mockPedidosRepository.Verify(r => r.SaveAsync(It.IsAny<Pedido>()), Times.Never);
    }

    [Test]
    public async Task CreatePedidoAsync_WithNonExistentProduct_ShouldThrowException()
    {
        // Arrange
        var userId = 1L;
        var productoId = 999L;

        var request = new PedidoRequestDto
        {
            Items = new List<PedidoItemRequestDto>
            {
                new PedidoItemRequestDto { ProductoId = productoId, Cantidad = 1 }
            }
        };

        _mockProductoRepository.Setup(r => r.FindByIdAsync(productoId)).ReturnsAsync((Producto?)null);

        // Act & Assert
        var act = async () => await _service.CreatePedidoAsync(userId, request);
        
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*not found*");

        // Verify nothing was saved
        _mockPedidosRepository.Verify(r => r.SaveAsync(It.IsAny<Pedido>()), Times.Never);
    }

    [Test]
    public async Task CreatePedidoAsync_WithMultipleItems_ShouldCalculateTotalCorrectly()
    {
        // Arrange
        var userId = 1L;
        
        var producto1 = new Producto
        {
            Id = 100L,
            Nombre = "Product 1",
            Precio = 50.00m,
            Stock = 10,
            CategoriaId = 1
        };

        var producto2 = new Producto
        {
            Id = 200L,
            Nombre = "Product 2",
            Precio = 30.00m,
            Stock = 10,
            CategoriaId = 1
        };

        var request = new PedidoRequestDto
        {
            Items = new List<PedidoItemRequestDto>
            {
                new PedidoItemRequestDto { ProductoId = 100L, Cantidad = 2 }, // 100
                new PedidoItemRequestDto { ProductoId = 200L, Cantidad = 3 }  // 90
            }
        };

        var savedPedido = new Pedido
        {
            Id = "507f1f77bcf86cd799439011",
            UserId = userId,
            Total = 190.00m
        };

        _mockProductoRepository.Setup(r => r.FindByIdAsync(100L)).ReturnsAsync(producto1);
        _mockProductoRepository.Setup(r => r.FindByIdAsync(200L)).ReturnsAsync(producto2);
        _mockProductoRepository.Setup(r => r.SaveAsync(It.IsAny<Producto>())).ReturnsAsync((Producto p) => p);
        _mockPedidosRepository.Setup(r => r.SaveAsync(It.IsAny<Pedido>())).ReturnsAsync(savedPedido);
        _mockEmailService.Setup(e => e.EnqueueEmailAsync(It.IsAny<EmailMessage>())).Returns(Task.CompletedTask);
        _mockMapper.Setup(m => m.Map<PedidoDto>(It.IsAny<Pedido>())).Returns(new PedidoDto());

        // Act
        await _service.CreatePedidoAsync(userId, request);

        // Assert
        _mockPedidosRepository.Verify(
            r => r.SaveAsync(It.Is<Pedido>(p => p.Total == 190.00m && p.Items.Count == 2)),
            Times.Once);
    }

    [Test]
    public async Task GetPedidoByIdAsync_WithExistingId_ShouldReturnPedidoDto()
    {
        // Arrange
        var pedidoId = "507f1f77bcf86cd799439011";
        var pedido = new Pedido
        {
            Id = pedidoId,
            UserId = 1L,
            Total = 100.00m
        };

        var pedidoDto = new PedidoDto
        {
            Id = pedidoId,
            UserId = 1L,
            Total = 100.00m
        };

        _mockPedidosRepository.Setup(r => r.FindByIdAsync(pedidoId)).ReturnsAsync(pedido);
        _mockMapper.Setup(m => m.Map<PedidoDto>(pedido)).Returns(pedidoDto);

        // Act
        var result = await _service.GetPedidoByIdAsync(pedidoId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(pedidoId);
        result.Total.Should().Be(100.00m);
    }

    [Test]
    public async Task GetPedidoByIdAsync_WithNonExistentId_ShouldReturnNull()
    {
        // Arrange
        var pedidoId = "nonexistent";
        _mockPedidosRepository.Setup(r => r.FindByIdAsync(pedidoId)).ReturnsAsync((Pedido?)null);

        // Act
        var result = await _service.GetPedidoByIdAsync(pedidoId);

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public async Task GetPedidosByUserIdAsync_ShouldReturnUserPedidos()
    {
        // Arrange
        var userId = 1L;
        var pedidos = new List<Pedido>
        {
            new Pedido { Id = "1", UserId = userId, Total = 100.00m },
            new Pedido { Id = "2", UserId = userId, Total = 200.00m }
        };

        var pedidoDtos = new List<PedidoDto>
        {
            new PedidoDto { Id = "1", UserId = userId, Total = 100.00m },
            new PedidoDto { Id = "2", UserId = userId, Total = 200.00m }
        };

        _mockPedidosRepository.Setup(r => r.FindByUserIdAsync(userId)).ReturnsAsync(pedidos);
        _mockMapper.Setup(m => m.Map<IEnumerable<PedidoDto>>(pedidos)).Returns(pedidoDtos);

        // Act
        var result = await _service.GetPedidosByUserIdAsync(userId);

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(p => p.UserId == userId);
    }
}
