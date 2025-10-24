using Refit;
using RefitClient.Console.Models;

namespace RefitClient.Console.Clients;

/// <summary>
/// Refit HTTP client interface for Tenistas API
/// Similar to Retrofit interface in Java
/// </summary>
public interface ITenistaApiClient
{
    /// <summary>
    /// GET /tenistas - Obtener todos los tenistas
    /// Similar a: @GET("tenistas") en Retrofit
    /// </summary>
    [Get("/tenistas")]
    Task<List<Tenista>> GetTenistasAsync();

    /// <summary>
    /// GET /tenistas/{id} - Obtener un tenista por ID
    /// Similar a: @GET("tenistas/{id}") en Retrofit
    /// </summary>
    [Get("/tenistas/{id}")]
    Task<Tenista> GetTenistaAsync(long id);

    /// <summary>
    /// POST /tenistas - Crear un nuevo tenista
    /// Similar a: @POST("tenistas") @Body Tenista en Retrofit
    /// </summary>
    [Post("/tenistas")]
    Task<Tenista> CreateTenistaAsync([Body] Tenista tenista);

    /// <summary>
    /// PUT /tenistas/{id} - Actualizar un tenista
    /// Similar a: @PUT("tenistas/{id}") @Body Tenista en Retrofit
    /// </summary>
    [Put("/tenistas/{id}")]
    Task<Tenista> UpdateTenistaAsync(long id, [Body] Tenista tenista);

    /// <summary>
    /// DELETE /tenistas/{id} - Eliminar un tenista
    /// Similar a: @DELETE("tenistas/{id}") en Retrofit
    /// </summary>
    [Delete("/tenistas/{id}")]
    Task DeleteTenistaAsync(long id);

    /// <summary>
    /// GET /tenistas/ranking/{ranking} - Buscar por ranking
    /// Similar a: @GET("tenistas/ranking/{ranking}") en Retrofit
    /// </summary>
    [Get("/tenistas/ranking/{ranking}")]
    Task<List<Tenista>> GetTenistasByRankingAsync(int ranking);

    /// <summary>
    /// GET /tenistas/search - Buscar con query parameters
    /// Similar a: @GET("tenistas/search") @Query en Retrofit
    /// </summary>
    [Get("/tenistas/search")]
    Task<List<Tenista>> SearchTenistasAsync([Query] string nombre, [Query] string? pais = null);
}
