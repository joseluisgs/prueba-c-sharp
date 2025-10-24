using NUnit.Framework;
using FluentAssertions;
using System.Reactive.Linq;
using System.Reactive.Concurrency;
using ProductosReactive.Console.Models;
using ProductosReactive.Console.Services;

namespace ProductosReactive.Tests;

[TestFixture]
public class ProductoReactiveServiceTests
{
    private ProductoObservableService _observableService = null!;
    private ProductoReactiveService _reactiveService = null!;

    [SetUp]
    public void Setup()
    {
        _observableService = new ProductoObservableService();
        _reactiveService = new ProductoReactiveService(_observableService);
    }

    [Test]
    public void GetProductosCarosConDescuento_DeberiaFiltrarYAplicarDescuento()
    {
        // Arrange
        var productos = new List<ProductoDto>();
        var precioMinimo = 100m;
        var descuento = 0.2m; // 20%

        // Act
        _reactiveService.GetProductosCarosConDescuento(precioMinimo, descuento)
            .Subscribe(p => productos.Add(p));

        // Assert
        productos.Should().NotBeEmpty();
        productos.Should().OnlyContain(p => p.Precio <= 1200 * 0.8m); // Precio máximo con descuento
    }

    [Test]
    public void MergeProductosStreams_DeberiaCombinarStreams()
    {
        // Arrange
        var productos = new List<Producto>();
        var stream1 = Observable.Return(new Producto { Id = 1, Nombre = "P1", Precio = 100, Categoria = "A", Stock = 10 });
        var stream2 = Observable.Return(new Producto { Id = 2, Nombre = "P2", Precio = 200, Categoria = "B", Stock = 20 });
        var stream3 = Observable.Return(new Producto { Id = 3, Nombre = "P3", Precio = 300, Categoria = "C", Stock = 30 });

        // Act
        _reactiveService.MergeProductosStreams(stream1, stream2, stream3)
            .Subscribe(p => productos.Add(p));

        // Assert
        productos.Should().HaveCount(3);
        productos.Select(p => p.Nombre).Should().Contain(new[] { "P1", "P2", "P3" });
    }

    [Test]
    public void ZipProductosConPrecios_DeberiaCombinarCorrectamente()
    {
        // Arrange
        var resultados = new List<string>();
        var productos = Observable.Range(1, 3)
            .Select(i => new Producto { Id = i, Nombre = $"Producto {i}", Precio = 100 * i, Categoria = "Test", Stock = 10 });
        var precios = Observable.Range(1, 3)
            .Select(i => (decimal)(50 * i));

        // Act
        _reactiveService.ZipProductosConPrecios(productos, precios)
            .Subscribe(r => resultados.Add(r));

        // Assert
        resultados.Should().HaveCount(3);
        resultados[0].Should().Contain("Producto 1").And.Contain("50");
    }

    [Test]
    public void BufferProductos_DeberiaAgruparProductos()
    {
        // Arrange
        var buffers = new List<IList<Producto>>();

        // Act
        _reactiveService.BufferProductos(TimeSpan.FromMilliseconds(100), 2)
            .Subscribe(buffer => buffers.Add(buffer));

        // Assert
        buffers.Should().NotBeEmpty();
        buffers.Sum(b => b.Count).Should().Be(5); // Total de productos
    }

    [Test]
    public void GetProductosWithRetry_DeberiaReintentar()
    {
        // Arrange
        var productos = new List<Producto>();

        // Act
        _reactiveService.GetProductosWithRetry(3)
            .Subscribe(p => productos.Add(p));

        // Assert
        productos.Should().HaveCount(5);
    }

    [Test]
    public void GetProductosDistinctByPrecio_DeberiaEliminarDuplicados()
    {
        // Arrange
        var productos = new List<Producto>();

        // Act
        _reactiveService.GetProductosDistinctByPrecio()
            .Subscribe(p => productos.Add(p));

        // Assert
        var precios = productos.Select(p => p.Precio).ToList();
        precios.Should().OnlyHaveUniqueItems();
    }

    [Test]
    public void GetProductosHastaPrecio_DeberiaTomarHastaLimite()
    {
        // Arrange
        var productos = new List<Producto>();
        var precioMaximo = 1500m; // Usar un límite más alto para que tome productos

        // Act
        _reactiveService.GetProductosHastaPrecio(precioMaximo)
            .Subscribe(p => productos.Add(p));

        // Assert
        productos.Should().NotBeEmpty();
        productos.Should().OnlyContain(p => p.Precio <= precioMaximo);
    }

    [Test]
    public void GetTotalAcumulado_DeberiaAcumularCorrectamente()
    {
        // Arrange
        var totales = new List<decimal>();

        // Act
        _reactiveService.GetTotalAcumulado()
            .Subscribe(total => totales.Add(total));

        // Assert
        totales.Should().NotBeEmpty();
        totales.Should().BeInAscendingOrder(); // Los totales deben ir incrementando
        totales.Last().Should().Be(2050); // 1200 + 25 + 75 + 300 + 450
    }

    [Test]
    public async Task GetProductosWithTimeout_DeberiaUsarFallback()
    {
        // Arrange
        var productos = new List<Producto>();
        var tcs = new TaskCompletionSource<bool>();

        // Act
        _reactiveService.GetProductosWithTimeout(TimeSpan.FromMilliseconds(1))
            .Subscribe(
                onNext: p => productos.Add(p),
                onCompleted: () => tcs.SetResult(true)
            );

        await tcs.Task;

        // Assert
        // Debería recibir el producto por defecto o algunos productos
        productos.Should().NotBeEmpty();
    }

    [Test]
    public void GetProductosCarosConDescuento_ConSchedulers_DeberiaUsarSchedulersCorrectamente()
    {
        // Arrange
        var productos = new List<ProductoDto>();
        var tcs = new TaskCompletionSource<bool>();

        // Act
        _reactiveService.GetProductosCarosConDescuento(
                100m, 
                0.1m, 
                subscribeOn: ImmediateScheduler.Instance,
                observeOn: ImmediateScheduler.Instance)
            .Subscribe(
                onNext: p => productos.Add(p),
                onCompleted: () => tcs.SetResult(true)
            );

        // Assert
        productos.Should().NotBeEmpty();
        productos.Should().OnlyContain(p => p.Precio > 0);
    }
}
