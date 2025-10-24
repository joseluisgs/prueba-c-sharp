using System.Reactive.Linq;
using System.Reactive.Concurrency;
using ProductosReactive.Console.Models;

namespace ProductosReactive.Console.Services;

/// <summary>
/// Service demonstrating advanced reactive operations similar to RxJava
/// </summary>
public class ProductoReactiveService
{
    private readonly ProductoObservableService _observableService;

    public ProductoReactiveService(ProductoObservableService observableService)
    {
        _observableService = observableService;
    }

    /// <summary>
    /// Combina operaciones reactivas complejas
    /// Similar a: Observable.filter().map().subscribeOn().observeOn()
    /// </summary>
    public IObservable<ProductoDto> GetProductosCarosConDescuento(
        decimal precioMinimo, 
        decimal descuento,
        IScheduler? subscribeOn = null,
        IScheduler? observeOn = null)
    {
        var observable = _observableService.GetProductosObservable()
            .Where(p => p.Precio > precioMinimo)
            .Select(p => p.WithDescuento(descuento))
            .Select(ProductoDto.FromProducto);

        // Apply schedulers if provided (similar to RxJava's subscribeOn/observeOn)
        if (subscribeOn != null)
            observable = observable.SubscribeOn(subscribeOn);
        
        if (observeOn != null)
            observable = observable.ObserveOn(observeOn);

        return observable;
    }

    /// <summary>
    /// Combina múltiples observables
    /// Similar a: Observable.merge()
    /// </summary>
    public IObservable<Producto> MergeProductosStreams(params IObservable<Producto>[] streams)
    {
        return streams.Merge();
    }

    /// <summary>
    /// Combina productos con Zip
    /// Similar a: Observable.zip()
    /// </summary>
    public IObservable<string> ZipProductosConPrecios(
        IObservable<Producto> productos,
        IObservable<decimal> precios)
    {
        return productos.Zip(precios, (p, precio) => 
            $"{p.Nombre} con nuevo precio: {precio:C}");
    }

    /// <summary>
    /// Buffer de productos por tiempo
    /// Similar a: Observable.buffer(time, count)
    /// </summary>
    public IObservable<IList<Producto>> BufferProductos(TimeSpan timeSpan, int count)
    {
        return _observableService.GetProductosObservable()
            .Buffer(timeSpan, count);
    }

    /// <summary>
    /// Retry con estrategia exponencial
    /// Similar a: Observable.retry() with backoff
    /// </summary>
    public IObservable<Producto> GetProductosWithRetry(int maxRetries)
    {
        return Observable.Create<Producto>(observer =>
        {
            var subscription = _observableService.GetProductosObservable()
                .Retry(maxRetries)
                .Subscribe(observer);
            
            return subscription;
        });
    }

    /// <summary>
    /// Timeout con fallback
    /// Similar a: Observable.timeout()
    /// </summary>
    public IObservable<Producto> GetProductosWithTimeout(TimeSpan timeout)
    {
        var fallback = Observable.Return(new Producto 
        { 
            Id = -1, 
            Nombre = "Producto por defecto", 
            Precio = 0, 
            Categoria = "N/A", 
            Stock = 0 
        });

        return _observableService.GetProductosObservable()
            .Timeout(timeout)
            .Catch(fallback);
    }

    /// <summary>
    /// Distinct products por precio
    /// Similar a: Observable.distinct()
    /// </summary>
    public IObservable<Producto> GetProductosDistinctByPrecio()
    {
        return _observableService.GetProductosObservable()
            .Distinct(p => p.Precio);
    }

    /// <summary>
    /// Take hasta que se cumpla condición
    /// Similar a: Observable.takeWhile()
    /// </summary>
    public IObservable<Producto> GetProductosHastaPrecio(decimal precioMaximo)
    {
        return _observableService.GetProductosObservable()
            .TakeWhile(p => p.Precio <= precioMaximo);
    }

    /// <summary>
    /// Scan para acumular valores
    /// Similar a: Observable.scan()
    /// </summary>
    public IObservable<decimal> GetTotalAcumulado()
    {
        return _observableService.GetProductosObservable()
            .Select(p => p.Precio)
            .Scan((acc, precio) => acc + precio);
    }
}
