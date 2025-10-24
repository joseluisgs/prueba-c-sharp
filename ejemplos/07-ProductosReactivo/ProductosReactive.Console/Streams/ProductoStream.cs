using System.Reactive.Linq;
using ProductosReactive.Console.Models;

namespace ProductosReactive.Console.Streams;

/// <summary>
/// Demonstrates Cold Observable pattern
/// Each subscription creates a new data stream
/// Similar to RxJava's Cold Observable
/// </summary>
public class ProductoStream
{
    /// <summary>
    /// Cold Observable - cada suscripción crea un nuevo stream independiente
    /// Cada subscriber recibe su propia secuencia de eventos
    /// </summary>
    public static IObservable<Producto> CreateColdObservable()
    {
        return Observable.Create<Producto>(observer =>
        {
            System.Console.WriteLine("🧊 Cold Observable: Nueva suscripción creada");
            
            // Simular carga de datos (cada subscriber obtiene su propia carga)
            var productos = new List<Producto>
            {
                new() { Id = 1, Nombre = "Producto Cold 1", Precio = 100, Categoria = "Test", Stock = 10 },
                new() { Id = 2, Nombre = "Producto Cold 2", Precio = 200, Categoria = "Test", Stock = 20 },
                new() { Id = 3, Nombre = "Producto Cold 3", Precio = 300, Categoria = "Test", Stock = 30 }
            };

            foreach (var producto in productos)
            {
                observer.OnNext(producto);
                Thread.Sleep(500); // Simular procesamiento
            }

            observer.OnCompleted();
            System.Console.WriteLine("🧊 Cold Observable: Stream completado");

            return () => System.Console.WriteLine("🧊 Cold Observable: Suscripción cancelada");
        });
    }

    /// <summary>
    /// Cold Observable con delay entre emisiones
    /// Útil para simular streams de datos en tiempo real
    /// </summary>
    public static IObservable<Producto> CreateDelayedColdObservable(TimeSpan delay)
    {
        return Observable.Create<Producto>(async (observer, cancellationToken) =>
        {
            System.Console.WriteLine("⏰ Delayed Cold Observable: Nueva suscripción creada");
            
            var productos = new List<Producto>
            {
                new() { Id = 10, Nombre = "Delayed 1", Precio = 150, Categoria = "Delayed", Stock = 5 },
                new() { Id = 11, Nombre = "Delayed 2", Precio = 250, Categoria = "Delayed", Stock = 15 },
            };

            foreach (var producto in productos)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                observer.OnNext(producto);
                await Task.Delay(delay, cancellationToken);
            }

            if (!cancellationToken.IsCancellationRequested)
            {
                observer.OnCompleted();
                System.Console.WriteLine("⏰ Delayed Cold Observable: Stream completado");
            }
        });
    }

    /// <summary>
    /// Cold Observable con manejo de errores
    /// Demuestra cómo cada suscripción maneja sus propios errores
    /// </summary>
    public static IObservable<Producto> CreateColdObservableWithError(bool shouldError = true)
    {
        return Observable.Create<Producto>(observer =>
        {
            System.Console.WriteLine("💥 Cold Observable with Error: Nueva suscripción");
            
            try
            {
                observer.OnNext(new Producto 
                { 
                    Id = 100, 
                    Nombre = "Producto antes de error", 
                    Precio = 99, 
                    Categoria = "Error Test", 
                    Stock = 1 
                });

                if (shouldError)
                {
                    throw new InvalidOperationException("Error simulado en Cold Observable");
                }

                observer.OnNext(new Producto 
                { 
                    Id = 101, 
                    Nombre = "Producto después de error", 
                    Precio = 199, 
                    Categoria = "Error Test", 
                    Stock = 2 
                });

                observer.OnCompleted();
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"💥 Error capturado: {ex.Message}");
                observer.OnError(ex);
            }

            return () => System.Console.WriteLine("💥 Cold Observable with Error: Suscripción cancelada");
        });
    }
}
