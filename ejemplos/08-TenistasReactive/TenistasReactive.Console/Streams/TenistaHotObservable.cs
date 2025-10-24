using System.Reactive.Linq;
using TenistasReactive.Console.Models;

namespace TenistasReactive.Console.Streams;

/// <summary>
/// Demonstrates Hot Observable patterns for real-time data
/// Similar to RxJava's Hot Observable with shared state
/// </summary>
public class TenistaHotObservable
{
    private readonly IObservable<Tenista> _hotStream;

    public TenistaHotObservable(IObservable<Tenista> source)
    {
        // Publish makes the observable hot - all subscribers share the same sequence
        _hotStream = source.Publish().RefCount();
    }

    public IObservable<Tenista> Stream => _hotStream;

    /// <summary>
    /// Crea un hot observable que emite periódicamente
    /// Similar a: Observable.interval() en RxJava
    /// </summary>
    public static IObservable<Tenista> CreatePeriodicHotObservable(TimeSpan interval)
    {
        var tenistas = new[]
        {
            new Tenista { Id = 1, Nombre = "Nadal", Ranking = 1, Pais = "España", Titulos = 22 },
            new Tenista { Id = 2, Nombre = "Federer", Ranking = 2, Pais = "Suiza", Titulos = 20 },
            new Tenista { Id = 3, Nombre = "Djokovic", Ranking = 3, Pais = "Serbia", Titulos = 24 }
        };

        return Observable.Interval(interval)
            .Select(i => tenistas[(int)(i % tenistas.Length)])
            .Publish()
            .RefCount();
    }
}
