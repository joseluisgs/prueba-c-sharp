using System.Reactive.Linq;
using ProductosReactive.Console.Models;
using ProductosReactive.Console.Services;
using ProductosReactive.Console.Streams;
using ProductosReactive.Console.Schedulers;

namespace ProductosReactive.Console;

class Program
{
    static async Task Main(string[] args)
    {
        System.Console.WriteLine("=== 🚀 Ejemplo 07: System.Reactive (RxJava → Rx.NET) ===\n");

        // Demo 1: Observable básico
        await DemoObservableBasico();

        // Demo 2: Hot vs Cold Observables
        await DemoHotVsColdObservables();

        // Demo 3: Subjects (PublishSubject, ReplaySubject, BehaviorSubject, AsyncSubject)
        await DemoSubjects();

        // Demo 4: Operators (filter, map, flatMap, etc.)
        await DemoOperators();

        // Demo 5: Schedulers (subscribeOn, observeOn)
        await DemoSchedulers();

        // Demo 6: Error Handling
        await DemoErrorHandling();

        System.Console.WriteLine("\n✅ Todos los ejemplos completados!");
    }

    static async Task DemoObservableBasico()
    {
        System.Console.WriteLine("\n📦 === Demo 1: Observable Básico ===");
        
        var service = new ProductoObservableService();

        System.Console.WriteLine("\n1.1 - GetProductosObservable (similar a Observable.fromIterable):");
        service.GetProductosObservable()
            .Subscribe(
                onNext: p => System.Console.WriteLine($"  ✓ {p}"),
                onError: ex => System.Console.WriteLine($"  ❌ Error: {ex.Message}"),
                onCompleted: () => System.Console.WriteLine("  ✅ Completado")
            );

        System.Console.WriteLine("\n1.2 - Filtrar productos caros (filter):");
        service.GetProductosPorPrecioMinimo(100)
            .Subscribe(p => System.Console.WriteLine($"  ✓ {p}"));

        System.Console.WriteLine("\n1.3 - Aplicar descuento (map):");
        service.GetProductosConDescuento(0.1m)
            .Subscribe(p => System.Console.WriteLine($"  ✓ {p}"));

        System.Console.WriteLine("\n1.4 - Agrupar por categoría (groupBy):");
        service.GetProductosAgrupadosPorCategoria()
            .Subscribe(group =>
            {
                System.Console.WriteLine($"  📁 Categoría: {group.Key}");
                group.Subscribe(p => System.Console.WriteLine($"    - {p.Nombre}"));
            });

        await Task.Delay(500);
    }

    static async Task DemoHotVsColdObservables()
    {
        System.Console.WriteLine("\n🔥 === Demo 2: Hot vs Cold Observables ===");

        // Cold Observable - cada subscriber obtiene su propia secuencia
        System.Console.WriteLine("\n2.1 - Cold Observable (cada subscriber recibe todos los eventos):");
        var coldStream = ProductoStream.CreateColdObservable();

        System.Console.WriteLine("  Subscriber 1:");
        coldStream.Subscribe(p => System.Console.WriteLine($"    Sub1: {p.Nombre}"));

        await Task.Delay(1000);

        System.Console.WriteLine("\n  Subscriber 2 (tarda en llegar):");
        coldStream.Subscribe(p => System.Console.WriteLine($"    Sub2: {p.Nombre}"));

        await Task.Delay(2000);

        // Hot Observable - todos los subscribers comparten el stream
        System.Console.WriteLine("\n2.2 - Hot Observable (subscribers comparten eventos):");
        var hotStream = new ProductoHotStream();

        System.Console.WriteLine("  Subscriber 1:");
        hotStream.HotObservable.Subscribe(p => System.Console.WriteLine($"    Sub1: {p.Nombre}"));

        hotStream.PublishProducto(new Producto { Id = 1, Nombre = "Hot Producto 1", Precio = 100, Categoria = "Hot", Stock = 10 });
        await Task.Delay(500);

        System.Console.WriteLine("\n  Subscriber 2 (tarda en llegar, se pierde eventos anteriores):");
        hotStream.HotObservable.Subscribe(p => System.Console.WriteLine($"    Sub2: {p.Nombre}"));

        hotStream.PublishProducto(new Producto { Id = 2, Nombre = "Hot Producto 2", Precio = 200, Categoria = "Hot", Stock = 20 });
        hotStream.Complete();

        await Task.Delay(500);
    }

    static async Task DemoSubjects()
    {
        System.Console.WriteLine("\n💾 === Demo 3: Subjects ===");

        // PublishSubject - similar a eventos, no guarda estado
        System.Console.WriteLine("\n3.1 - PublishSubject (sin replay):");
        var publishStream = new ProductoHotStream();
        publishStream.PublishProducto(new Producto { Id = 1, Nombre = "Publish 1", Precio = 100, Categoria = "Test", Stock = 10 });
        publishStream.HotObservable.Subscribe(p => System.Console.WriteLine($"  PublishSub: {p.Nombre}"));
        publishStream.PublishProducto(new Producto { Id = 2, Nombre = "Publish 2", Precio = 200, Categoria = "Test", Stock = 20 });

        // ReplaySubject - guarda y replay últimos N eventos
        System.Console.WriteLine("\n3.2 - ReplaySubject (replay últimos 2 eventos):");
        var replayStream = new ProductoReplayStream(bufferSize: 2);
        replayStream.PublishProducto(new Producto { Id = 1, Nombre = "Replay 1", Precio = 100, Categoria = "Test", Stock = 10 });
        replayStream.PublishProducto(new Producto { Id = 2, Nombre = "Replay 2", Precio = 200, Categoria = "Test", Stock = 20 });
        replayStream.PublishProducto(new Producto { Id = 3, Nombre = "Replay 3", Precio = 300, Categoria = "Test", Stock = 30 });
        
        System.Console.WriteLine("  Nuevo subscriber (recibe últimos 2):");
        replayStream.ReplayObservable.Subscribe(p => System.Console.WriteLine($"  ReplaySub: {p.Nombre}"));

        // BehaviorSubject - guarda último valor
        System.Console.WriteLine("\n3.3 - BehaviorSubject (último valor):");
        var behaviorStream = new ProductoBehaviorStream(
            new Producto { Id = 0, Nombre = "Initial", Precio = 0, Categoria = "Test", Stock = 0 });
        
        System.Console.WriteLine($"  Valor inicial: {behaviorStream.CurrentValue.Nombre}");
        behaviorStream.PublishProducto(new Producto { Id = 1, Nombre = "Behavior 1", Precio = 100, Categoria = "Test", Stock = 10 });
        
        System.Console.WriteLine("  Nuevo subscriber (recibe último valor):");
        behaviorStream.BehaviorObservable.Subscribe(p => System.Console.WriteLine($"  BehaviorSub: {p.Nombre}"));

        // AsyncSubject - solo emite último valor al completar
        System.Console.WriteLine("\n3.4 - AsyncSubject (último valor al completar):");
        var asyncStream = new ProductoAsyncStream();
        asyncStream.AsyncObservable.Subscribe(p => System.Console.WriteLine($"  AsyncSub: {p.Nombre}"));
        
        asyncStream.PublishProducto(new Producto { Id = 1, Nombre = "Async 1", Precio = 100, Categoria = "Test", Stock = 10 });
        asyncStream.PublishProducto(new Producto { Id = 2, Nombre = "Async 2", Precio = 200, Categoria = "Test", Stock = 20 });
        asyncStream.Complete(); // Solo ahora se emite el último valor

        await Task.Delay(500);
    }

    static async Task DemoOperators()
    {
        System.Console.WriteLine("\n⚙️ === Demo 4: Operators ===");

        var service = new ProductoObservableService();
        var reactiveService = new ProductoReactiveService(service);

        System.Console.WriteLine("\n4.1 - Merge (combinar múltiples streams):");
        var stream1 = Observable.Return(new Producto { Id = 1, Nombre = "Stream1", Precio = 100, Categoria = "A", Stock = 10 });
        var stream2 = Observable.Return(new Producto { Id = 2, Nombre = "Stream2", Precio = 200, Categoria = "B", Stock = 20 });
        
        reactiveService.MergeProductosStreams(stream1, stream2)
            .Subscribe(p => System.Console.WriteLine($"  Merged: {p.Nombre}"));

        System.Console.WriteLine("\n4.2 - Buffer (agrupar por tiempo/cantidad):");
        reactiveService.BufferProductos(TimeSpan.FromMilliseconds(100), 2)
            .Subscribe(list => System.Console.WriteLine($"  Buffer: {list.Count} productos"));

        System.Console.WriteLine("\n4.3 - Distinct (valores únicos):");
        reactiveService.GetProductosDistinctByPrecio()
            .Subscribe(p => System.Console.WriteLine($"  Distinct: {p.Nombre} - {p.Precio:C}"));

        System.Console.WriteLine("\n4.4 - Scan (acumulador):");
        reactiveService.GetTotalAcumulado()
            .Subscribe(total => System.Console.WriteLine($"  Total acumulado: {total:C}"));

        await Task.Delay(500);
    }

    static async Task DemoSchedulers()
    {
        System.Console.WriteLine("\n🔧 === Demo 5: Schedulers ===");

        var testObservable = ProductoSchedulers.CreateTestObservable();

        System.Console.WriteLine("\n5.1 - TaskPoolScheduler (background thread):");
        await ProductoSchedulers.DemoTaskPoolScheduler(testObservable)
            .LastOrDefaultAsync();

        System.Console.WriteLine("\n5.2 - SubscribeOn + ObserveOn:");
        await ProductoSchedulers.DemoSubscribeOnAndObserveOn(testObservable)
            .LastOrDefaultAsync();

        System.Console.WriteLine("\n5.3 - Throttle (control de flujo):");
        var rapidStream = Observable.Interval(TimeSpan.FromMilliseconds(50))
            .Take(10)
            .Select(i => new Producto 
            { 
                Id = i, 
                Nombre = $"Rapid {i}", 
                Precio = 100 * i, 
                Categoria = "Rapid", 
                Stock = 10 
            });
        
        await ProductoSchedulers.DemoThrottling(rapidStream, TimeSpan.FromMilliseconds(200))
            .LastOrDefaultAsync();

        await Task.Delay(500);
    }

    static async Task DemoErrorHandling()
    {
        System.Console.WriteLine("\n💥 === Demo 6: Error Handling ===");

        System.Console.WriteLine("\n6.1 - Cold Observable con error:");
        ProductoStream.CreateColdObservableWithError(shouldError: true)
            .Subscribe(
                onNext: p => System.Console.WriteLine($"  ✓ {p.Nombre}"),
                onError: ex => System.Console.WriteLine($"  ❌ Error manejado: {ex.Message}"),
                onCompleted: () => System.Console.WriteLine("  ✅ Completado")
            );

        System.Console.WriteLine("\n6.2 - Retry con error:");
        var service = new ProductoObservableService();
        var reactiveService = new ProductoReactiveService(service);
        
        var errorCount = 0;
        Observable.Create<Producto>(observer =>
            {
                if (errorCount++ < 2)
                {
                    System.Console.WriteLine($"  ⚠️ Intento {errorCount} - Error simulado");
                    observer.OnError(new Exception("Error temporal"));
                }
                else
                {
                    System.Console.WriteLine($"  ✓ Intento {errorCount} - Éxito");
                    observer.OnNext(new Producto { Id = 1, Nombre = "Success", Precio = 100, Categoria = "Test", Stock = 10 });
                    observer.OnCompleted();
                }
                return () => { };
            })
            .Retry(3)
            .Subscribe(
                onNext: p => System.Console.WriteLine($"  ✅ Recibido: {p.Nombre}"),
                onError: ex => System.Console.WriteLine($"  ❌ Error final: {ex.Message}"),
                onCompleted: () => System.Console.WriteLine("  ✅ Completado")
            );

        await Task.Delay(500);
    }
}
