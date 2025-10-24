using System.Reactive.Subjects;
using System.Reactive.Linq;
using ProductosReactive.Console.Models;

namespace ProductosReactive.Console.Streams;

/// <summary>
/// Demonstrates Hot Observable pattern
/// All subscribers share the same data stream
/// Similar to RxJava's Hot Observable with Subject
/// </summary>
public class ProductoHotStream
{
    private readonly Subject<Producto> _subject = new();
    
    /// <summary>
    /// Hot Observable - todos los subscribers comparten el mismo stream
    /// Los nuevos subscribers solo reciben eventos futuros
    /// Similar a: PublishSubject en RxJava
    /// </summary>
    public IObservable<Producto> HotObservable => _subject.AsObservable();

    /// <summary>
    /// Publica un producto en el stream compartido
    /// </summary>
    public void PublishProducto(Producto producto)
    {
        System.Console.WriteLine($"🔥 Hot Observable: Publicando {producto.Nombre}");
        _subject.OnNext(producto);
    }

    /// <summary>
    /// Completa el stream para todos los subscribers
    /// </summary>
    public void Complete()
    {
        System.Console.WriteLine("🔥 Hot Observable: Stream completado");
        _subject.OnCompleted();
    }

    /// <summary>
    /// Simula publicación continua de productos
    /// </summary>
    public async Task StartPublishing(CancellationToken cancellationToken = default)
    {
        System.Console.WriteLine("🔥 Hot Observable: Iniciando publicación continua");
        
        var productoId = 1;
        while (!cancellationToken.IsCancellationRequested)
        {
            var producto = new Producto
            {
                Id = productoId++,
                Nombre = $"Hot Product {productoId}",
                Precio = Random.Shared.Next(50, 500),
                Categoria = "Hot Stream",
                Stock = Random.Shared.Next(1, 100)
            };

            PublishProducto(producto);
            await Task.Delay(1000, cancellationToken);
        }
    }
}

/// <summary>
/// ReplaySubject example - buffers and replays events to new subscribers
/// Similar to RxJava's ReplaySubject
/// </summary>
public class ProductoReplayStream
{
    private readonly ReplaySubject<Producto> _replaySubject;

    /// <summary>
    /// Constructor que acepta el tamaño del buffer de replay
    /// </summary>
    public ProductoReplayStream(int bufferSize = 3)
    {
        _replaySubject = new ReplaySubject<Producto>(bufferSize);
    }

    /// <summary>
    /// Observable que replay los últimos N eventos a nuevos subscribers
    /// </summary>
    public IObservable<Producto> ReplayObservable => _replaySubject.AsObservable();

    /// <summary>
    /// Publica un producto que será replayado
    /// </summary>
    public void PublishProducto(Producto producto)
    {
        System.Console.WriteLine($"📼 Replay Subject: Publicando {producto.Nombre}");
        _replaySubject.OnNext(producto);
    }

    public void Complete()
    {
        System.Console.WriteLine("📼 Replay Subject: Stream completado");
        _replaySubject.OnCompleted();
    }
}

/// <summary>
/// BehaviorSubject example - remembers the last emitted value
/// Similar to RxJava's BehaviorSubject
/// </summary>
public class ProductoBehaviorStream
{
    private readonly BehaviorSubject<Producto> _behaviorSubject;

    /// <summary>
    /// Constructor que requiere un valor inicial
    /// </summary>
    public ProductoBehaviorStream(Producto initialValue)
    {
        _behaviorSubject = new BehaviorSubject<Producto>(initialValue);
    }

    /// <summary>
    /// Observable que emite el último valor inmediatamente al subscribirse
    /// </summary>
    public IObservable<Producto> BehaviorObservable => _behaviorSubject.AsObservable();

    /// <summary>
    /// Obtiene el valor actual
    /// </summary>
    public Producto CurrentValue => _behaviorSubject.Value;

    /// <summary>
    /// Publica un nuevo producto
    /// </summary>
    public void PublishProducto(Producto producto)
    {
        System.Console.WriteLine($"💾 Behavior Subject: Publicando {producto.Nombre}");
        _behaviorSubject.OnNext(producto);
    }

    public void Complete()
    {
        System.Console.WriteLine("💾 Behavior Subject: Stream completado");
        _behaviorSubject.OnCompleted();
    }
}

/// <summary>
/// AsyncSubject example - only emits the last value when completed
/// Similar to RxJava's AsyncSubject
/// </summary>
public class ProductoAsyncStream
{
    private readonly AsyncSubject<Producto> _asyncSubject = new();

    /// <summary>
    /// Observable que solo emite el último valor cuando se completa
    /// </summary>
    public IObservable<Producto> AsyncObservable => _asyncSubject.AsObservable();

    /// <summary>
    /// Publica un producto (no será emitido hasta Complete)
    /// </summary>
    public void PublishProducto(Producto producto)
    {
        System.Console.WriteLine($"⏳ Async Subject: Guardando {producto.Nombre}");
        _asyncSubject.OnNext(producto);
    }

    /// <summary>
    /// Completa el stream y emite el último valor
    /// </summary>
    public void Complete()
    {
        System.Console.WriteLine("⏳ Async Subject: Completando y emitiendo último valor");
        _asyncSubject.OnCompleted();
    }
}
