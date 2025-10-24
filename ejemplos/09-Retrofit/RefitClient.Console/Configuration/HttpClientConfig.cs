using Polly;
using Polly.Extensions.Http;
using Microsoft.Extensions.DependencyInjection;
using Refit;

namespace RefitClient.Console.Configuration;

/// <summary>
/// HttpClient configuration with Polly policies
/// Similar to Retrofit configuration with OkHttp interceptors
/// </summary>
public static class HttpClientConfig
{
    /// <summary>
    /// Retry policy with exponential backoff
    /// Similar to Retrofit retry interceptor
    /// </summary>
    public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: retryAttempt => 
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (outcome, timespan, retryCount, context) =>
                {
                    System.Console.WriteLine($"⚠️ Retry {retryCount} after {timespan.TotalSeconds}s delay");
                });
    }

    /// <summary>
    /// Circuit breaker policy
    /// Similar to Retrofit failure thresholds
    /// </summary>
    public static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: 5,
                durationOfBreak: TimeSpan.FromSeconds(30),
                onBreak: (outcome, duration) =>
                {
                    System.Console.WriteLine($"🔴 Circuit breaker opened for {duration.TotalSeconds}s");
                },
                onReset: () =>
                {
                    System.Console.WriteLine("🟢 Circuit breaker reset");
                });
    }

    /// <summary>
    /// Timeout policy
    /// Similar to Retrofit timeout configuration
    /// </summary>
    public static IAsyncPolicy<HttpResponseMessage> GetTimeoutPolicy(TimeSpan timeout)
    {
        return Policy.TimeoutAsync<HttpResponseMessage>(timeout);
    }

    /// <summary>
    /// Configure HttpClient with all policies
    /// </summary>
    public static IHttpClientBuilder AddRefitClientWithPolicies<TClient>(
        this IServiceCollection services,
        string baseUrl,
        TimeSpan? timeout = null) where TClient : class
    {
        timeout ??= TimeSpan.FromSeconds(30);

        return services.AddRefitClient<TClient>()
            .ConfigureHttpClient(c =>
            {
                c.BaseAddress = new Uri(baseUrl);
                c.Timeout = timeout.Value;
            })
            .AddPolicyHandler(GetRetryPolicy())
            .AddPolicyHandler(GetCircuitBreakerPolicy());
    }
}
