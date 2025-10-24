using System.Reactive.Linq;
using TenistasReactive.Console.Models;
using TenistasReactive.Console.Services;
using TenistasReactive.Console.Streams;
using TenistasReactive.Console.ErrorHandling;

namespace TenistasReactive.Console;

class Program
{
    static async Task Main(string[] args)
    {
        System.Console.WriteLine("=== 🎾 Ejemplo 08: Tenistas Reactive (Advanced Rx.NET) ===\n");

        // Demo 1: Hot vs Cold Observables
        await DemoHotVsCold();

        // Demo 2: Subjects
        await DemoSubjects();

        // Demo 3: BackPressure
        await DemoBackPressure();

        // Demo 4: Error Handling
        await DemoErrorHandling();

        System.Console.WriteLine("\n✅ Todos los ejemplos completados!");
    }

    static async Task DemoHotVsCold()
    {
        System.Console.WriteLine("\n🔥 === Demo 1: Hot vs Cold Observables ===");

        // Cold Observable
        System.Console.WriteLine("\n1.1 - Cold Observable (cada subscriber ejecuta independientemente):");
        var cold = TenistaColdObservable.CreateColdObservable();
        
        System.Console.WriteLine("  Subscriber 1:");
        cold.Subscribe(t => System.Console.WriteLine($"    Sub1: {t.Nombre}"));
        
        await Task.Delay(500);
        
        System.Console.WriteLine("\n  Subscriber 2:");
        cold.Subscribe(t => System.Console.WriteLine($"    Sub2: {t.Nombre}"));

        await Task.Delay(500);

        // Hot Observable
        System.Console.WriteLine("\n1.2 - Hot Observable (subscribers comparten la misma secuencia):");
        var service = new TenistaReactiveService();
        var hot = new TenistaHotObservable(service.GetTenistasColObservable());
        
        System.Console.WriteLine("  Subscriber 1:");
        hot.Stream.Subscribe(t => System.Console.WriteLine($"    Sub1: {t.Nombre}"));
        
        await Task.Delay(300);
        
        System.Console.WriteLine("\n  Subscriber 2 (llega tarde, se pierde algunos eventos):");
        hot.Stream.Subscribe(t => System.Console.WriteLine($"    Sub2: {t.Nombre}"));

        await Task.Delay(1000);
    }

    static async Task DemoSubjects()
    {
        System.Console.WriteLine("\n💾 === Demo 2: Subjects (Hot Observables) ===");

        var service = new TenistaSubjectService();

        // PublishSubject
        System.Console.WriteLine("\n2.1 - PublishSubject (sin estado):");
        service.PublishStream.Subscribe(t => System.Console.WriteLine($"  Pub1: {t.Nombre}"));
        service.PublishTenista(new Tenista { Id = 1, Nombre = "Nadal", Ranking = 1, Pais = "España", Titulos = 22 });
        
        System.Console.WriteLine("  Nuevo subscriber (se pierde evento anterior):");
        service.PublishStream.Subscribe(t => System.Console.WriteLine($"  Pub2: {t.Nombre}"));
        service.PublishTenista(new Tenista { Id = 2, Nombre = "Federer", Ranking = 2, Pais = "Suiza", Titulos = 20 });

        // BehaviorSubject
        System.Console.WriteLine("\n2.2 - BehaviorSubject (último valor):");
        System.Console.WriteLine($"  Valor actual: {service.CurrentTenista.Nombre}");
        service.UpdateBehaviorTenista(new Tenista { Id = 1, Nombre = "Nadal", Ranking = 1, Pais = "España", Titulos = 22 });
        
        System.Console.WriteLine("  Nuevo subscriber (recibe último valor):");
        service.BehaviorStream.Subscribe(t => System.Console.WriteLine($"  Behavior: {t.Nombre}"));

        // ReplaySubject
        System.Console.WriteLine("\n2.3 - ReplaySubject (replay últimos 3):");
        service.AddToReplay(new Tenista { Id = 1, Nombre = "Nadal", Ranking = 1, Pais = "España", Titulos = 22 });
        service.AddToReplay(new Tenista { Id = 2, Nombre = "Federer", Ranking = 2, Pais = "Suiza", Titulos = 20 });
        service.AddToReplay(new Tenista { Id = 3, Nombre = "Djokovic", Ranking = 3, Pais = "Serbia", Titulos = 24 });
        
        System.Console.WriteLine("  Nuevo subscriber (recibe últimos 3):");
        service.ReplayStream.Subscribe(t => System.Console.WriteLine($"  Replay: {t.Nombre}"));

        // AsyncSubject
        System.Console.WriteLine("\n2.4 - AsyncSubject (solo último al completar):");
        service.AsyncStream.Subscribe(t => System.Console.WriteLine($"  Async: {t.Nombre}"));
        service.AddToAsync(new Tenista { Id = 1, Nombre = "Nadal", Ranking = 1, Pais = "España", Titulos = 22 });
        service.AddToAsync(new Tenista { Id = 2, Nombre = "Federer", Ranking = 2, Pais = "Suiza", Titulos = 20 });
        service.CompleteAsync(); // Solo ahora emite el último

        await Task.Delay(500);
    }

    static async Task DemoBackPressure()
    {
        System.Console.WriteLine("\n📦 === Demo 3: BackPressure Strategies ===");

        var service = new TenistaReactiveService();
        var source = service.GetTenistasColObservable();

        // Buffer Strategy
        System.Console.WriteLine("\n3.1 - Buffer Strategy:");
        await TenistaBackPressure.WithBufferStrategy(source, TimeSpan.FromMilliseconds(500), 3)
            .Do(batch => System.Console.WriteLine($"  📦 Buffer: {batch.Count} tenistas"))
            .LastOrDefaultAsync();

        // Throttle Strategy
        System.Console.WriteLine("\n3.2 - Throttle Strategy:");
        var rapidSource = Observable.Interval(TimeSpan.FromMilliseconds(50))
            .Take(10)
            .Select(i => new Tenista 
            { 
                Id = i, 
                Nombre = $"Tenista {i}", 
                Ranking = (int)i + 1, 
                Pais = "Test", 
                Titulos = 0 
            });
        
        await TenistaBackPressure.WithThrottleStrategy(rapidSource, TimeSpan.FromMilliseconds(200))
            .Do(t => System.Console.WriteLine($"  ⏱️ Throttled: {t.Nombre}"))
            .LastOrDefaultAsync();

        await Task.Delay(500);
    }

    static async Task DemoErrorHandling()
    {
        System.Console.WriteLine("\n💥 === Demo 4: Error Handling ===");

        // Error con fallback
        System.Console.WriteLine("\n4.1 - Error con valor por defecto:");
        var errorSource = Observable.Create<Tenista>(observer =>
        {
            observer.OnError(new InvalidOperationException("Error simulado"));
            return () => { };
        });

        var fallback = new Tenista { Id = -1, Nombre = "Fallback", Ranking = 999, Pais = "N/A", Titulos = 0 };
        
        await ReactiveErrorHandler.WithFallback(errorSource, fallback)
            .Do(t => System.Console.WriteLine($"  ✓ Recibido: {t.Nombre}"))
            .LastOrDefaultAsync();

        // Timeout
        System.Console.WriteLine("\n4.2 - Timeout con fallback:");
        var slowSource = Observable.Create<Tenista>(async (observer, ct) =>
        {
            await Task.Delay(2000, ct);
            observer.OnNext(new Tenista { Id = 1, Nombre = "Slow", Ranking = 1, Pais = "Test", Titulos = 0 });
            observer.OnCompleted();
        });

        await ReactiveErrorHandler.WithTimeout(slowSource, TimeSpan.FromMilliseconds(100), fallback)
            .Do(t => System.Console.WriteLine($"  ⏰ Timeout - Usando fallback: {t.Nombre}"))
            .LastOrDefaultAsync();

        await Task.Delay(500);
    }
}
