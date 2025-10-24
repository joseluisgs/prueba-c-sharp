using System.Reactive.Linq;
using System.Reactive.Subjects;
using ProductosReactive.Console.Models;

namespace ProductosReactive.Console.Services;

/// <summary>
/// Service demonstrating Observable patterns similar to RxJava PublishSubject
/// </summary>
public class ProductoObservableService
{
    private readonly Subject<Producto> _productosSubject = new();
    
    // Observable para suscripciones (similar a RxJava's PublishSubject)
    public IObservable<Producto> ProductosStream => _productosSubject.AsObservable();

    // Lista interna de productos
    private readonly List<Producto> _productos = new()
    {
        new Producto { Id = 1, Nombre = "Laptop", Precio = 1200, Categoria = "Electrónica", Stock = 10 },
        new Producto { Id = 2, Nombre = "Mouse", Precio = 25, Categoria = "Electrónica", Stock = 50 },
        new Producto { Id = 3, Nombre = "Teclado", Precio = 75, Categoria = "Electrónica", Stock = 30 },
        new Producto { Id = 4, Nombre = "Monitor", Precio = 300, Categoria = "Electrónica", Stock = 15 },
        new Producto { Id = 5, Nombre = "Silla Gamer", Precio = 450, Categoria = "Muebles", Stock = 8 },
    };

    /// <summary>
    /// Obtiene todos los productos como Observable
    /// Similar a: Observable.fromIterable(productos)
    /// </summary>
    public IObservable<Producto> GetProductosObservable()
    {
        return _productos.ToObservable();
    }

    /// <summary>
    /// Filtra productos por precio mínimo
    /// Similar a: Observable.filter(p -> p.getPrecio() > minPrecio)
    /// </summary>
    public IObservable<Producto> GetProductosPorPrecioMinimo(decimal minPrecio)
    {
        return GetProductosObservable()
            .Where(p => p.Precio > minPrecio);
    }

    /// <summary>
    /// Obtiene productos con descuento aplicado
    /// Similar a: Observable.map(p -> p.withDescuento(descuento))
    /// </summary>
    public IObservable<Producto> GetProductosConDescuento(decimal descuento)
    {
        return GetProductosObservable()
            .Select(p => p.WithDescuento(descuento));
    }

    /// <summary>
    /// Agrupa productos por categoría
    /// Similar a: Observable.groupBy(Producto::getCategoria)
    /// </summary>
    public IObservable<IGroupedObservable<string, Producto>> GetProductosAgrupadosPorCategoria()
    {
        return GetProductosObservable()
            .GroupBy(p => p.Categoria);
    }

    /// <summary>
    /// Publica un nuevo producto en el stream (Hot Observable)
    /// Similar a: subject.onNext(producto)
    /// </summary>
    public void PublishProducto(Producto producto)
    {
        _productosSubject.OnNext(producto);
    }

    /// <summary>
    /// Completa el stream de productos
    /// Similar a: subject.onComplete()
    /// </summary>
    public void CompleteStream()
    {
        _productosSubject.OnCompleted();
    }

    /// <summary>
    /// Emite un error en el stream
    /// Similar a: subject.onError(error)
    /// </summary>
    public void EmitError(Exception error)
    {
        _productosSubject.OnError(error);
    }
}
