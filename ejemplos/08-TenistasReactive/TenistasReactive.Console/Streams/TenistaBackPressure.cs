using System.Reactive.Linq;
using TenistasReactive.Console.Models;

namespace TenistasReactive.Console.Streams;

/// <summary>
/// Demonstrates backpressure handling strategies
/// Similar to RxJava's Flowable with backpressure strategies
/// </summary>
public class TenistaBackPressure
{
    /// <summary>
    /// Buffer strategy - accumulate items until processed
    /// Similar to: BackpressureStrategy.BUFFER en RxJava
    /// </summary>
    public static IObservable<IList<Tenista>> WithBufferStrategy(
        IObservable<Tenista> source, 
        TimeSpan timeWindow, 
        int maxCount)
    {
        System.Console.WriteLine($"📦 BackPressure: Buffer strategy ({timeWindow.TotalMilliseconds}ms, max {maxCount})");
        return source.Buffer(timeWindow, maxCount);
    }

    /// <summary>
    /// Throttle strategy - drop intermediate items
    /// Similar to: BackpressureStrategy.DROP/LATEST en RxJava
    /// </summary>
    public static IObservable<Tenista> WithThrottleStrategy(
        IObservable<Tenista> source, 
        TimeSpan throttleTime)
    {
        System.Console.WriteLine($"⏱️ BackPressure: Throttle strategy ({throttleTime.TotalMilliseconds}ms)");
        return source.Throttle(throttleTime);
    }

    /// <summary>
    /// Sample strategy - take items at regular intervals
    /// Similar to: sample() en RxJava
    /// </summary>
    public static IObservable<Tenista> WithSampleStrategy(
        IObservable<Tenista> source, 
        TimeSpan sampleInterval)
    {
        System.Console.WriteLine($"📸 BackPressure: Sample strategy ({sampleInterval.TotalMilliseconds}ms)");
        return source.Sample(sampleInterval);
    }
}
