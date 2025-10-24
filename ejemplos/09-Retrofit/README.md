# 🌐 Ejemplo 09: Refit HTTP Client - Retrofit vs Refit

## 🎯 Objetivo

Demostrar el uso de **Refit** como cliente HTTP declarativo en C#/.NET, equivalente a **Retrofit** en Java, mostrando cómo migrar conceptos de HTTP clients declarativos.

## 🔄 Comparativa: Retrofit (Java) vs Refit (C#)

### Definición de API Interface

**Java (Retrofit):**
```java
public interface TenistaApiService {
    @GET("tenistas")
    Call<List<Tenista>> getTenistas();
    
    @GET("tenistas/{id}")
    Call<Tenista> getTenista(@Path("id") Long id);
    
    @POST("tenistas")
    Call<Tenista> createTenista(@Body Tenista tenista);
    
    @PUT("tenistas/{id}")
    Call<Tenista> updateTenista(@Path("id") Long id, @Body Tenista tenista);
    
    @DELETE("tenistas/{id}")
    Call<Void> deleteTenista(@Path("id") Long id);
    
    @GET("tenistas/search")
    Call<List<Tenista>> searchTenistas(@Query("nombre") String nombre);
}
```

**C# (Refit):**
```csharp
public interface ITenistaApiClient
{
    [Get("/tenistas")]
    Task<List<Tenista>> GetTenistasAsync();
    
    [Get("/tenistas/{id}")]
    Task<Tenista> GetTenistaAsync(long id);
    
    [Post("/tenistas")]
    Task<Tenista> CreateTenistaAsync([Body] Tenista tenista);
    
    [Put("/tenistas/{id}")]
    Task<Tenista> UpdateTenistaAsync(long id, [Body] Tenista tenista);
    
    [Delete("/tenistas/{id}")]
    Task DeleteTenistaAsync(long id);
    
    [Get("/tenistas/search")]
    Task<List<Tenista>> SearchTenistasAsync([Query] string nombre);
}
```

### Configuración del Cliente

**Java (Retrofit):**
```java
// Configurar OkHttpClient
OkHttpClient client = new OkHttpClient.Builder()
    .connectTimeout(10, TimeUnit.SECONDS)
    .readTimeout(10, TimeUnit.SECONDS)
    .addInterceptor(loggingInterceptor)
    .build();

// Crear Retrofit instance
Retrofit retrofit = new Retrofit.Builder()
    .baseUrl("https://api.tenistas.com/")
    .client(client)
    .addConverterFactory(GsonConverterFactory.create())
    .build();

// Crear service
TenistaApiService service = retrofit.create(TenistaApiService.class);
```

**C# (Refit):**
```csharp
// Configurar con Dependency Injection
services.AddRefitClient<ITenistaApiClient>()
    .ConfigureHttpClient(c =>
    {
        c.BaseAddress = new Uri("https://api.tenistas.com/");
        c.Timeout = TimeSpan.FromSeconds(10);
    })
    .AddPolicyHandler(GetRetryPolicy())
    .AddPolicyHandler(GetCircuitBreakerPolicy());

// Obtener service
var client = provider.GetRequiredService<ITenistaApiClient>();
```

## 🏗️ Estructura del Proyecto

```
09-Retrofit/
├── RefitClient.Console/
│   ├── Models/
│   │   ├── Tenista.cs                # API response models
│   │   ├── ApiResponse.cs            # Generic API response
│   │   └── ErrorResponse.cs          # Error handling
│   ├── Clients/
│   │   └── ITenistaApiClient.cs      # Refit interface
│   ├── Services/
│   │   └── TenistaHttpService.cs     # Business logic layer
│   ├── Configuration/
│   │   └── HttpClientConfig.cs       # HttpClient setup with Polly
│   ├── Program.cs                    # Retrofit vs Refit demos
│   └── RefitClient.Console.csproj
├── RefitClient.Tests/                # HTTP client testing
│   ├── TenistaHttpServiceTests.cs
│   └── RefitClient.Tests.csproj
└── README.md
```

## 🔧 Tecnologías Utilizadas

### NuGet Packages
```xml
<!-- Refit HTTP Client -->
<PackageReference Include="Refit" Version="7.0.0" />
<PackageReference Include="Refit.HttpClientFactory" Version="7.0.0" />

<!-- Microsoft HTTP Extensions -->
<PackageReference Include="Microsoft.Extensions.Http" Version="8.0.0" />
<PackageReference Include="Microsoft.Extensions.Http.Polly" Version="8.0.0" />
<PackageReference Include="Microsoft.Extensions.DependencyInjection" Version="8.0.0" />

<!-- Polly Resilience -->
<PackageReference Include="Polly" Version="8.2.0" />
```

## 🚀 Ejecución

```bash
# Console App
cd RefitClient.Console
dotnet run

# Tests
cd RefitClient.Tests
dotnet test
```

## 📚 Conceptos Demostrados

### 1. Atributos HTTP en Refit

| Retrofit (Java) | Refit (C#) | Descripción |
|-----------------|------------|-------------|
| `@GET` | `[Get]` | HTTP GET request |
| `@POST` | `[Post]` | HTTP POST request |
| `@PUT` | `[Put]` | HTTP PUT request |
| `@DELETE` | `[Delete]` | HTTP DELETE request |
| `@PATCH` | `[Patch]` | HTTP PATCH request |
| `@HEAD` | `[Head]` | HTTP HEAD request |
| `@OPTIONS` | `[Options]` | HTTP OPTIONS request |

### 2. Parámetros de Request

| Retrofit (Java) | Refit (C#) | Uso |
|-----------------|------------|-----|
| `@Path("id")` | Path parameter en URL | Parámetro en ruta |
| `@Query("name")` | `[Query]` | Query string parameter |
| `@Body` | `[Body]` | Request body |
| `@Header("name")` | `[Header("name")]` | HTTP header |
| `@HeaderMap` | `[HeaderCollection]` | Múltiples headers |
| `@Field` | `[Property]` | Form field |

### 3. Polly Policies (Resilience)

Refit se integra perfectamente con **Polly** para resilience patterns:

```csharp
// Retry Policy
services.AddRefitClient<ITenistaApiClient>()
    .AddPolicyHandler(HttpPolicyExtensions
        .HandleTransientHttpError()
        .WaitAndRetryAsync(3, retryAttempt => 
            TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))));

// Circuit Breaker
services.AddRefitClient<ITenistaApiClient>()
    .AddPolicyHandler(HttpPolicyExtensions
        .HandleTransientHttpError()
        .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30)));

// Timeout
services.AddRefitClient<ITenistaApiClient>()
    .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(
        TimeSpan.FromSeconds(10)));
```

**Equivalente en Java (Retrofit con OkHttp):**
```java
OkHttpClient client = new OkHttpClient.Builder()
    .addInterceptor(new RetryInterceptor(3))
    .connectTimeout(10, TimeUnit.SECONDS)
    .readTimeout(10, TimeUnit.SECONDS)
    .build();
```

### 4. Manejo de Errores

**Refit:**
```csharp
try
{
    var tenista = await client.GetTenistaAsync(id);
    return ApiResponse<Tenista>.SuccessResponse(tenista);
}
catch (ApiException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
{
    return ApiResponse<Tenista>.ErrorResponse("Not found", 404);
}
catch (ApiException ex)
{
    return ApiResponse<Tenista>.ErrorResponse($"API Error: {ex.Message}", (int)ex.StatusCode);
}
```

**Retrofit:**
```java
try {
    Response<Tenista> response = service.getTenista(id).execute();
    if (response.isSuccessful()) {
        return ApiResponse.success(response.body());
    } else {
        return ApiResponse.error(response.code(), response.message());
    }
} catch (IOException e) {
    return ApiResponse.error(500, e.getMessage());
}
```

## 🎓 Diferencias Clave Java → C#

### Nomenclatura
- **Java:** `@GET` → **C#:** `[Get]` (atributos con corchetes)
- **Java:** `Call<T>` → **C#:** `Task<T>` (async nativo)
- **Java:** `execute()` / `enqueue()` → **C#:** `await` (async/await)

### Configuración
- **Java:** Retrofit.Builder() → **C#:** IServiceCollection extensions
- **Java:** OkHttp interceptors → **C#:** Polly policies
- **Java:** Gson/Jackson → **C#:** System.Text.Json (por defecto)

### Dependency Injection
- **Retrofit:** Manual creation → **Refit:** Integración nativa con DI de .NET

## 🔗 Ventajas de Refit sobre Retrofit

1. **Type Safety**: Interfaces fuertemente tipadas con async/await nativo
2. **DI Integration**: Integración perfecta con Dependency Injection de .NET
3. **Polly Integration**: Resilience patterns con Circuit Breaker, Retry, Timeout
4. **Less Boilerplate**: No necesitas manejar Response<T> manualmente
5. **Modern C#**: Aprovecha características modernas de C# (async/await, nullable reference types)

## 🔗 Recursos Adicionales

- [Refit Documentation](https://github.com/reactiveui/refit)
- [Polly Documentation](https://github.com/App-vNext/Polly)
- [Retrofit to Refit Migration Guide](https://github.com/reactiveui/refit#comparison-to-retrofit)

---

**Nota:** Este ejemplo es parte del proyecto educativo de migración Java/Spring Boot → C#/.NET
