using System.Reactive.Linq;
using TenistasReactive.Console.Models;

namespace TenistasReactive.Console.Streams;

/// <summary>
/// Demonstrates Cold Observable patterns - each subscriber gets its own independent sequence
/// Similar to RxJava's Cold Observable
/// </summary>
public class TenistaColdObservable
{
    /// <summary>
    /// Creates a cold observable - each subscription triggers a new execution
    /// Similar to: Observable.create() en RxJava
    /// </summary>
    public static IObservable<Tenista> CreateColdObservable()
    {
        return Observable.Create<Tenista>(observer =>
        {
            System.Console.WriteLine("🧊 Cold: Nueva ejecución para este subscriber");
            
            var tenistas = new[]
            {
                new Tenista { Id = 1, Nombre = "Nadal", Ranking = 1, Pais = "España", Titulos = 22 },
                new Tenista { Id = 2, Nombre = "Federer", Ranking = 2, Pais = "Suiza", Titulos = 20 }
            };

            foreach (var tenista in tenistas)
            {
                observer.OnNext(tenista);
            }

            observer.OnCompleted();
            return () => System.Console.WriteLine("🧊 Cold: Subscription disposed");
        });
    }
}
