using System.Reactive.Linq;
using TenistasReactive.Console.Models;

namespace TenistasReactive.Console.ErrorHandling;

/// <summary>
/// Demonstrates error handling strategies in reactive streams
/// Similar to RxJava's error handling operators
/// </summary>
public class ReactiveErrorHandler
{
    /// <summary>
    /// Retry with exponential backoff
    /// Similar to: retryWhen() con backoff en RxJava
    /// </summary>
    public static IObservable<Tenista> WithRetryBackoff(
        IObservable<Tenista> source, 
        int maxRetries, 
        TimeSpan initialDelay)
    {
        System.Console.WriteLine($"🔄 Error Handler: Retry con backoff (max {maxRetries} intentos)");
        
        var attempts = 0;
        return source.Catch((Exception ex) =>
        {
            if (attempts++ < maxRetries)
            {
                var delay = TimeSpan.FromMilliseconds(initialDelay.TotalMilliseconds * Math.Pow(2, attempts - 1));
                System.Console.WriteLine($"⚠️ Error: {ex.Message}. Reintentando en {delay.TotalMilliseconds}ms (intento {attempts}/{maxRetries})");
                return Observable.Timer(delay).SelectMany(_ => WithRetryBackoff(source, maxRetries - attempts, initialDelay));
            }
            System.Console.WriteLine($"❌ Error final después de {attempts} intentos: {ex.Message}");
            return Observable.Throw<Tenista>(ex);
        });
    }

    /// <summary>
    /// Fallback to default value on error
    /// Similar to: onErrorReturn() en RxJava
    /// </summary>
    public static IObservable<Tenista> WithFallback(
        IObservable<Tenista> source, 
        Tenista fallbackValue)
    {
        System.Console.WriteLine("🔄 Error Handler: Fallback a valor por defecto");
        return source.Catch(Observable.Return(fallbackValue));
    }

    /// <summary>
    /// Switch to alternative stream on error
    /// Similar to: onErrorResumeNext() en RxJava
    /// </summary>
    public static IObservable<Tenista> WithAlternativeStream(
        IObservable<Tenista> source, 
        IObservable<Tenista> alternative)
    {
        System.Console.WriteLine("🔄 Error Handler: Stream alternativo en caso de error");
        return source.Catch(alternative);
    }

    /// <summary>
    /// Timeout with fallback
    /// Similar to: timeout() en RxJava
    /// </summary>
    public static IObservable<Tenista> WithTimeout(
        IObservable<Tenista> source, 
        TimeSpan timeout, 
        Tenista fallbackValue)
    {
        System.Console.WriteLine($"⏰ Error Handler: Timeout de {timeout.TotalMilliseconds}ms con fallback");
        return source
            .Timeout(timeout)
            .Catch(Observable.Return(fallbackValue));
    }
}
