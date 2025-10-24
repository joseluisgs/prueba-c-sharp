using Microsoft.Extensions.DependencyInjection;
using Refit;
using RefitClient.Console.Clients;
using RefitClient.Console.Configuration;
using RefitClient.Console.Services;
using RefitClient.Console.Models;

namespace RefitClient.Console;

class Program
{
    static async Task Main(string[] args)
    {
        System.Console.WriteLine("=== 🌐 Ejemplo 09: Refit HTTP Client (Retrofit → Refit) ===\n");

        // Configurar servicios
        var services = new ServiceCollection();
        
        // Configurar Refit client con Polly policies
        // Nota: Usando JSONPlaceholder como API de ejemplo
        services.AddRefitClientWithPolicies<ITenistaApiClient>(
            "https://jsonplaceholder.typicode.com",
            timeout: TimeSpan.FromSeconds(10));

        // Registrar servicio
        services.AddTransient<TenistaHttpService>();

        var provider = services.BuildServiceProvider();

        // Demo 1: Configuración básica de Refit
        DemoRefitConfiguration();

        // Demo 2: Llamadas HTTP básicas (con API mock)
        await DemoBasicHttpCalls(provider);

        // Demo 3: Manejo de errores
        await DemoErrorHandling(provider);

        System.Console.WriteLine("\n✅ Ejemplos completados!");
        System.Console.WriteLine("\n💡 Nota: Para usar con una API real, cambia la baseUrl a tu API de tenistas.");
    }

    static void DemoRefitConfiguration()
    {
        System.Console.WriteLine("\n🔧 === Demo 1: Configuración de Refit ===");
        System.Console.WriteLine("\nComparativa Retrofit (Java) vs Refit (C#):");
        
        System.Console.WriteLine("\n📌 Java (Retrofit):");
        System.Console.WriteLine(@"
Retrofit retrofit = new Retrofit.Builder()
    .baseUrl(""https://api.tenistas.com/"")
    .addConverterFactory(GsonConverterFactory.create())
    .client(okHttpClient)
    .build();

TenistaApiService service = retrofit.create(TenistaApiService.class);
        ");

        System.Console.WriteLine("\n📌 C# (Refit):");
        System.Console.WriteLine(@"
services.AddRefitClient<ITenistaApiClient>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri(""https://api.tenistas.com/""))
    .AddPolicyHandler(GetRetryPolicy());

var client = provider.GetRequiredService<ITenistaApiClient>();
        ");

        System.Console.WriteLine("\n✅ Ventajas de Refit:");
        System.Console.WriteLine("  - Integración nativa con Dependency Injection");
        System.Console.WriteLine("  - Soporte para Polly policies (retry, circuit breaker, timeout)");
        System.Console.WriteLine("  - Type-safe interfaces con atributos");
        System.Console.WriteLine("  - Async/await nativo");
    }

    static async Task DemoBasicHttpCalls(ServiceProvider provider)
    {
        System.Console.WriteLine("\n🌐 === Demo 2: Llamadas HTTP Básicas ===");

        var service = provider.GetRequiredService<TenistaHttpService>();

        try
        {
            System.Console.WriteLine("\n2.1 - GET Request (simulado con JSONPlaceholder):");
            System.Console.WriteLine("  Similar a: @GET(\"tenistas\") en Retrofit");
            
            // Nota: JSONPlaceholder no tiene endpoint de tenistas, esto es solo demo
            System.Console.WriteLine("  ✓ Configuración lista para llamada GET");

            System.Console.WriteLine("\n2.2 - POST Request:");
            System.Console.WriteLine("  Similar a: @POST(\"tenistas\") @Body Tenista en Retrofit");
            
            var nuevoTenista = new Tenista
            {
                Id = 0,
                Nombre = "Carlos Alcaraz",
                Ranking = 1,
                Pais = "España",
                Titulos = 2
            };

            System.Console.WriteLine($"  ✓ Preparado para crear: {nuevoTenista}");

            System.Console.WriteLine("\n2.3 - PUT Request:");
            System.Console.WriteLine("  Similar a: @PUT(\"tenistas/{id}\") @Path(\"id\") en Retrofit");
            System.Console.WriteLine("  ✓ Configuración lista para actualización");

            System.Console.WriteLine("\n2.4 - DELETE Request:");
            System.Console.WriteLine("  Similar a: @DELETE(\"tenistas/{id}\") en Retrofit");
            System.Console.WriteLine("  ✓ Configuración lista para eliminación");
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"  ⚠️ Error esperado (no hay API real): {ex.Message}");
        }

        await Task.CompletedTask;
    }

    static async Task DemoErrorHandling(ServiceProvider provider)
    {
        System.Console.WriteLine("\n💥 === Demo 3: Manejo de Errores ===");

        System.Console.WriteLine("\n3.1 - Retry con exponential backoff:");
        System.Console.WriteLine("  - Polly: 3 reintentos con delay exponencial (2^n segundos)");
        System.Console.WriteLine("  - Similar a: OkHttp Interceptor con retry en Retrofit");

        System.Console.WriteLine("\n3.2 - Circuit Breaker:");
        System.Console.WriteLine("  - Se abre después de 5 fallos consecutivos");
        System.Console.WriteLine("  - Permanece abierto 30 segundos");
        System.Console.WriteLine("  - Similar a: Hystrix/Resilience4j en Java");

        System.Console.WriteLine("\n3.3 - Timeout:");
        System.Console.WriteLine("  - Timeout de 10 segundos configurado");
        System.Console.WriteLine("  - Similar a: connectTimeout/readTimeout en OkHttp");

        System.Console.WriteLine("\n3.4 - ApiException handling:");
        System.Console.WriteLine("  - Refit lanza ApiException para errores HTTP");
        System.Console.WriteLine("  - Similar a: HttpException en Retrofit");

        await Task.CompletedTask;
    }
}
