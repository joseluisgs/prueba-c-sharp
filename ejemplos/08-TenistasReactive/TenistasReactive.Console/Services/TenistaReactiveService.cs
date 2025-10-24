using System.Reactive.Linq;
using System.Reactive.Subjects;
using TenistasReactive.Console.Models;

namespace TenistasReactive.Console.Services;

/// <summary>
/// Advanced reactive service demonstrating complex RxJava patterns in Rx.NET
/// </summary>
public class TenistaReactiveService
{
    private readonly List<Tenista> _tenistas = new()
    {
        new() { Id = 1, Nombre = "Rafael Nadal", Ranking = 1, Pais = "España", Titulos = 22 },
        new() { Id = 2, Nombre = "Roger Federer", Ranking = 2, Pais = "Suiza", Titulos = 20 },
        new() { Id = 3, Nombre = "Novak Djokovic", Ranking = 3, Pais = "Serbia", Titulos = 24 },
        new() { Id = 4, Nombre = "Carlos Alcaraz", Ranking = 4, Pais = "España", Titulos = 2 },
        new() { Id = 5, Nombre = "Daniil Medvedev", Ranking = 5, Pais = "Rusia", Titulos = 1 }
    };

    /// <summary>
    /// Cold Observable - cada subscriber obtiene su propia secuencia completa
    /// Similar a: Observable.create() en RxJava
    /// </summary>
    public IObservable<Tenista> GetTenistasColObservable()
    {
        return Observable.Create<Tenista>(async (observer, cancellationToken) =>
        {
            System.Console.WriteLine("🧊 Cold Observable: Nueva suscripción iniciada");
            
            foreach (var tenista in _tenistas)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;
                
                observer.OnNext(tenista);
                await Task.Delay(100, cancellationToken); // Simular procesamiento asíncrono
            }
            
            if (!cancellationToken.IsCancellationRequested)
            {
                observer.OnCompleted();
                System.Console.WriteLine("🧊 Cold Observable: Completado");
            }
        });
    }

    /// <summary>
    /// Obtiene tenistas filtrados por ranking
    /// Similar a: filter() en RxJava
    /// </summary>
    public IObservable<Tenista> GetTenistasByRankingTop(int maxRanking)
    {
        return GetTenistasColObservable()
            .Where(t => t.Ranking <= maxRanking);
    }

    /// <summary>
    /// Combina múltiples streams reactivos
    /// Similar a: Observable.merge() en RxJava
    /// </summary>
    public IObservable<Tenista> MergeTenistaStreams(params IObservable<Tenista>[] streams)
    {
        return streams.Merge();
    }

    /// <summary>
    /// Procesa con backpressure usando buffer
    /// Similar a: buffer() con backpressure en RxJava
    /// </summary>
    public IObservable<IList<Tenista>> GetTenistasWithBackPressure(TimeSpan timeWindow, int maxCount)
    {
        return GetTenistasColObservable()
            .Buffer(timeWindow, maxCount);
    }

    /// <summary>
    /// Manejo de errores con retry
    /// Similar a: retry() con estrategia en RxJava
    /// </summary>
    public IObservable<Tenista> GetTenistasWithRetry(int maxRetries)
    {
        return GetTenistasColObservable()
            .Retry(maxRetries);
    }

    /// <summary>
    /// Throttle para rate limiting
    /// Similar a: throttleFirst() en RxJava
    /// </summary>
    public IObservable<Tenista> GetTenistasThrottled(TimeSpan throttleTime)
    {
        return GetTenistasColObservable()
            .Throttle(throttleTime);
    }
}
