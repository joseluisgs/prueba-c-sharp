using NUnit.Framework;
using FluentAssertions;
using System.Reactive.Linq;
using Microsoft.Reactive.Testing;
using ProductosReactive.Console.Models;
using ProductosReactive.Console.Services;

namespace ProductosReactive.Tests;

[TestFixture]
public class ProductoObservableServiceTests
{
    private ProductoObservableService _service = null!;

    [SetUp]
    public void Setup()
    {
        _service = new ProductoObservableService();
    }

    [Test]
    public void GetProductosObservable_DeberiaEmitirTodosLosProductos()
    {
        // Arrange
        var productos = new List<Producto>();

        // Act
        _service.GetProductosObservable()
            .Subscribe(p => productos.Add(p));

        // Assert
        productos.Should().HaveCount(5);
        productos.Should().Contain(p => p.Nombre == "Laptop");
    }

    [Test]
    public void GetProductosPorPrecioMinimo_DeberiaFiltrarCorrectamente()
    {
        // Arrange
        var productos = new List<Producto>();
        var precioMinimo = 100m;

        // Act
        _service.GetProductosPorPrecioMinimo(precioMinimo)
            .Subscribe(p => productos.Add(p));

        // Assert
        productos.Should().NotBeEmpty();
        productos.Should().OnlyContain(p => p.Precio > precioMinimo);
    }

    [Test]
    public void GetProductosConDescuento_DeberiaAplicarDescuentoCorrectamente()
    {
        // Arrange
        var productos = new List<Producto>();
        var descuento = 0.1m; // 10%

        // Act
        _service.GetProductosConDescuento(descuento)
            .Subscribe(p => productos.Add(p));

        // Assert
        productos.Should().NotBeEmpty();
        // Verificar que todos los precios son menores que los originales
        productos.Should().OnlyContain(p => p.Precio < 1200); // Laptop original: 1200
    }

    [Test]
    public void GetProductosAgrupadosPorCategoria_DeberiaAgruparCorrectamente()
    {
        // Arrange
        var categorias = new List<string>();

        // Act
        _service.GetProductosAgrupadosPorCategoria()
            .Subscribe(group =>
            {
                categorias.Add(group.Key);
                group.Subscribe(); // Consumir el grupo
            });

        // Assert
        categorias.Should().Contain("Electrónica");
        categorias.Should().Contain("Muebles");
    }

    [Test]
    public void PublishProducto_DeberiaEmitirEnStream()
    {
        // Arrange
        var productosRecibidos = new List<Producto>();
        _service.ProductosStream.Subscribe(p => productosRecibidos.Add(p));

        var nuevoProducto = new Producto
        {
            Id = 100,
            Nombre = "Test Product",
            Precio = 500,
            Categoria = "Test",
            Stock = 10
        };

        // Act
        _service.PublishProducto(nuevoProducto);

        // Assert
        productosRecibidos.Should().HaveCount(1);
        productosRecibidos[0].Nombre.Should().Be("Test Product");
    }

    [Test]
    public void CompleteStream_DeberiaCompletarStream()
    {
        // Arrange
        var completed = false;
        _service.ProductosStream.Subscribe(
            onNext: _ => { },
            onCompleted: () => completed = true
        );

        // Act
        _service.CompleteStream();

        // Assert
        completed.Should().BeTrue();
    }

    [Test]
    public void EmitError_DeberiaEmitirError()
    {
        // Arrange
        Exception? error = null;
        _service.ProductosStream.Subscribe(
            onNext: _ => { },
            onError: ex => error = ex
        );

        var expectedError = new InvalidOperationException("Test error");

        // Act
        _service.EmitError(expectedError);

        // Assert
        error.Should().NotBeNull();
        error.Should().BeOfType<InvalidOperationException>();
        error!.Message.Should().Be("Test error");
    }

    [Test]
    public async Task ProductosStream_HotObservable_DeberiaSoloEmitirEventosFuturos()
    {
        // Arrange
        var subscriber1 = new List<Producto>();
        var subscriber2 = new List<Producto>();

        var producto1 = new Producto { Id = 1, Nombre = "Producto 1", Precio = 100, Categoria = "Test", Stock = 10 };
        var producto2 = new Producto { Id = 2, Nombre = "Producto 2", Precio = 200, Categoria = "Test", Stock = 20 };

        // Act
        _service.ProductosStream.Subscribe(p => subscriber1.Add(p));
        _service.PublishProducto(producto1);
        
        await Task.Delay(100);
        
        _service.ProductosStream.Subscribe(p => subscriber2.Add(p));
        _service.PublishProducto(producto2);

        await Task.Delay(100);

        // Assert
        subscriber1.Should().HaveCount(2);
        subscriber2.Should().HaveCount(1); // Solo recibe el segundo producto
        subscriber2[0].Should().Be(producto2);
    }
}
