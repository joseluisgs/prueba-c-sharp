using System.Reactive.Concurrency;
using System.Reactive.Linq;
using ProductosReactive.Console.Models;

namespace ProductosReactive.Console.Schedulers;

/// <summary>
/// Demonstrates Rx.NET Schedulers similar to RxJava Schedulers
/// Schedulers control where and when Observable operations execute
/// </summary>
public class ProductoSchedulers
{
    /// <summary>
    /// Demuestra el uso de TaskPoolScheduler
    /// Similar a: Schedulers.io() en RxJava
    /// </summary>
    public static IObservable<Producto> DemoTaskPoolScheduler(IObservable<Producto> source)
    {
        System.Console.WriteLine("🔧 Usando TaskPoolScheduler (similar a Schedulers.io())");
        
        return source
            .SubscribeOn(TaskPoolScheduler.Default) // Ejecutar suscripción en thread pool
            .Do(p => System.Console.WriteLine(
                $"  📊 Procesando en Thread {Environment.CurrentManagedThreadId}: {p.Nombre}"));
    }

    /// <summary>
    /// Demuestra el uso de NewThreadScheduler
    /// Similar a: Schedulers.newThread() en RxJava
    /// </summary>
    public static IObservable<Producto> DemoNewThreadScheduler(IObservable<Producto> source)
    {
        System.Console.WriteLine("🔧 Usando NewThreadScheduler (similar a Schedulers.newThread())");
        
        return source
            .SubscribeOn(NewThreadScheduler.Default) // Ejecutar en un nuevo thread dedicado
            .Do(p => System.Console.WriteLine(
                $"  🆕 Procesando en Thread {Environment.CurrentManagedThreadId}: {p.Nombre}"));
    }

    /// <summary>
    /// Demuestra el uso de ImmediateScheduler
    /// Similar a: Schedulers.immediate() en RxJava
    /// </summary>
    public static IObservable<Producto> DemoImmediateScheduler(IObservable<Producto> source)
    {
        System.Console.WriteLine("🔧 Usando ImmediateScheduler (similar a Schedulers.immediate())");
        
        return source
            .ObserveOn(ImmediateScheduler.Instance) // Ejecutar inmediatamente en el thread actual
            .Do(p => System.Console.WriteLine(
                $"  ⚡ Procesando en Thread {Environment.CurrentManagedThreadId}: {p.Nombre}"));
    }

    /// <summary>
    /// Demuestra el uso de CurrentThreadScheduler
    /// Similar a: Schedulers.trampoline() en RxJava
    /// Cola las operaciones en el thread actual
    /// </summary>
    public static IObservable<Producto> DemoCurrentThreadScheduler(IObservable<Producto> source)
    {
        System.Console.WriteLine("🔧 Usando CurrentThreadScheduler (similar a Schedulers.trampoline())");
        
        return source
            .ObserveOn(Scheduler.CurrentThread) // Encolar en el thread actual
            .Do(p => System.Console.WriteLine(
                $"  🔄 Procesando en Thread {Environment.CurrentManagedThreadId}: {p.Nombre}"));
    }

    /// <summary>
    /// Demuestra combinación de SubscribeOn y ObserveOn
    /// Similar al patrón común en RxJava: subscribeOn(io()).observeOn(mainThread())
    /// </summary>
    public static IObservable<Producto> DemoSubscribeOnAndObserveOn(IObservable<Producto> source)
    {
        System.Console.WriteLine("🔧 Combinando SubscribeOn y ObserveOn");
        
        return source
            .Do(p => System.Console.WriteLine(
                $"  📥 Origen en Thread {Environment.CurrentManagedThreadId}: {p.Nombre}"))
            .SubscribeOn(TaskPoolScheduler.Default) // Suscripción en background
            .Do(p => System.Console.WriteLine(
                $"  🔄 Después de SubscribeOn en Thread {Environment.CurrentManagedThreadId}: {p.Nombre}"))
            .ObserveOn(NewThreadScheduler.Default) // Observación en otro thread
            .Do(p => System.Console.WriteLine(
                $"  📤 Después de ObserveOn en Thread {Environment.CurrentManagedThreadId}: {p.Nombre}"));
    }

    /// <summary>
    /// Demuestra operaciones paralelas con múltiples schedulers
    /// </summary>
    public static IObservable<Producto> DemoParallelProcessing(IObservable<Producto> source)
    {
        System.Console.WriteLine("🔧 Procesamiento paralelo con múltiples schedulers");
        
        return source
            .SelectMany(p => 
                Observable.Return(p)
                    .SubscribeOn(TaskPoolScheduler.Default)
                    .Select(producto =>
                    {
                        System.Console.WriteLine(
                            $"  ⚙️ Procesando {producto.Nombre} en Thread {Environment.CurrentManagedThreadId}");
                        Thread.Sleep(100); // Simular trabajo
                        return producto;
                    }));
    }

    /// <summary>
    /// Demuestra throttling con schedulers
    /// Útil para rate limiting y debouncing
    /// </summary>
    public static IObservable<Producto> DemoThrottling(IObservable<Producto> source, TimeSpan throttleTime)
    {
        System.Console.WriteLine($"🔧 Aplicando throttle de {throttleTime.TotalMilliseconds}ms");
        
        return source
            .Throttle(throttleTime, TaskPoolScheduler.Default)
            .Do(p => System.Console.WriteLine($"  🕐 Throttled: {p.Nombre}"));
    }

    /// <summary>
    /// Demuestra delay con scheduler específico
    /// </summary>
    public static IObservable<Producto> DemoDelay(IObservable<Producto> source, TimeSpan delay)
    {
        System.Console.WriteLine($"🔧 Aplicando delay de {delay.TotalMilliseconds}ms");
        
        return source
            .Delay(delay, TaskPoolScheduler.Default)
            .Do(p => System.Console.WriteLine(
                $"  ⏰ Delayed en Thread {Environment.CurrentManagedThreadId}: {p.Nombre}"));
    }

    /// <summary>
    /// Demuestra Sample/Throttle para control de flujo
    /// Similar a: sample() en RxJava
    /// </summary>
    public static IObservable<Producto> DemoSample(IObservable<Producto> source, TimeSpan sampleInterval)
    {
        System.Console.WriteLine($"🔧 Muestreando cada {sampleInterval.TotalMilliseconds}ms");
        
        return source
            .Sample(sampleInterval, TaskPoolScheduler.Default)
            .Do(p => System.Console.WriteLine($"  📸 Sample: {p.Nombre}"));
    }

    /// <summary>
    /// Crea un Observable de ejemplo para testing
    /// </summary>
    public static IObservable<Producto> CreateTestObservable()
    {
        return Observable.Create<Producto>(observer =>
        {
            var productos = new[]
            {
                new Producto { Id = 1, Nombre = "Producto 1", Precio = 100, Categoria = "Test", Stock = 10 },
                new Producto { Id = 2, Nombre = "Producto 2", Precio = 200, Categoria = "Test", Stock = 20 },
                new Producto { Id = 3, Nombre = "Producto 3", Precio = 300, Categoria = "Test", Stock = 30 }
            };

            foreach (var producto in productos)
            {
                observer.OnNext(producto);
            }

            observer.OnCompleted();
            return () => { };
        });
    }
}
