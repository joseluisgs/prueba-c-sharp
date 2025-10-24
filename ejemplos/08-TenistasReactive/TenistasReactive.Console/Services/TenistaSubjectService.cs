using System.Reactive.Subjects;
using System.Reactive.Linq;
using TenistasReactive.Console.Models;

namespace TenistasReactive.Console.Services;

/// <summary>
/// Service demonstrating Subject patterns (Hot Observables)
/// Similar to RxJava's PublishSubject, BehaviorSubject, ReplaySubject, AsyncSubject
/// </summary>
public class TenistaSubjectService
{
    private readonly Subject<Tenista> _publishSubject = new();
    private readonly BehaviorSubject<Tenista> _behaviorSubject;
    private readonly ReplaySubject<Tenista> _replaySubject;
    private readonly AsyncSubject<Tenista> _asyncSubject = new();

    public TenistaSubjectService()
    {
        var initialTenista = new Tenista 
        { 
            Id = 0, 
            Nombre = "Initial", 
            Ranking = 0, 
            Pais = "N/A", 
            Titulos = 0 
        };
        
        _behaviorSubject = new BehaviorSubject<Tenista>(initialTenista);
        _replaySubject = new ReplaySubject<Tenista>(bufferSize: 3);
    }

    // PublishSubject - Hot Observable sin estado
    public IObservable<Tenista> PublishStream => _publishSubject.AsObservable();
    
    public void PublishTenista(Tenista tenista)
    {
        System.Console.WriteLine($"📢 Publish: {tenista.Nombre}");
        _publishSubject.OnNext(tenista);
    }

    // BehaviorSubject - Mantiene el último valor
    public IObservable<Tenista> BehaviorStream => _behaviorSubject.AsObservable();
    public Tenista CurrentTenista => _behaviorSubject.Value;
    
    public void UpdateBehaviorTenista(Tenista tenista)
    {
        System.Console.WriteLine($"💾 Behavior: {tenista.Nombre}");
        _behaviorSubject.OnNext(tenista);
    }

    // ReplaySubject - Replay últimos N eventos
    public IObservable<Tenista> ReplayStream => _replaySubject.AsObservable();
    
    public void AddToReplay(Tenista tenista)
    {
        System.Console.WriteLine($"📼 Replay: {tenista.Nombre}");
        _replaySubject.OnNext(tenista);
    }

    // AsyncSubject - Solo emite el último valor al completar
    public IObservable<Tenista> AsyncStream => _asyncSubject.AsObservable();
    
    public void AddToAsync(Tenista tenista)
    {
        System.Console.WriteLine($"⏳ Async (guardado): {tenista.Nombre}");
        _asyncSubject.OnNext(tenista);
    }
    
    public void CompleteAsync()
    {
        System.Console.WriteLine("⏳ Async: Completando y emitiendo último valor");
        _asyncSubject.OnCompleted();
    }

    public void CompleteAll()
    {
        _publishSubject.OnCompleted();
        _behaviorSubject.OnCompleted();
        _replaySubject.OnCompleted();
    }
}
