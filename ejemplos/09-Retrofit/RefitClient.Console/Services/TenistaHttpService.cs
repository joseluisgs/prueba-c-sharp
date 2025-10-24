using RefitClient.Console.Clients;
using RefitClient.Console.Models;
using Refit;

namespace RefitClient.Console.Services;

/// <summary>
/// Service layer wrapping Refit HTTP client
/// Similar to service layer using Retrofit in Java
/// </summary>
public class TenistaHttpService
{
    private readonly ITenistaApiClient _client;

    public TenistaHttpService(ITenistaApiClient client)
    {
        _client = client;
    }

    /// <summary>
    /// Obtener todos los tenistas con manejo de errores
    /// </summary>
    public async Task<TenistaApiResponse<List<Tenista>>> GetAllTenistasAsync()
    {
        try
        {
            var tenistas = await _client.GetTenistasAsync();
            return TenistaApiResponse<List<Tenista>>.SuccessResponse(tenistas);
        }
        catch (ApiException ex)
        {
            return TenistaApiResponse<List<Tenista>>.ErrorResponse(
                $"API Error: {ex.Message}", 
                (int)ex.StatusCode);
        }
        catch (Exception ex)
        {
            return TenistaApiResponse<List<Tenista>>.ErrorResponse(
                $"Error: {ex.Message}");
        }
    }

    /// <summary>
    /// Obtener un tenista por ID con manejo de errores
    /// </summary>
    public async Task<TenistaApiResponse<Tenista>> GetTenistaByIdAsync(long id)
    {
        try
        {
            var tenista = await _client.GetTenistaAsync(id);
            return TenistaApiResponse<Tenista>.SuccessResponse(tenista);
        }
        catch (ApiException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return TenistaApiResponse<Tenista>.ErrorResponse("Tenista not found", 404);
        }
        catch (ApiException ex)
        {
            return TenistaApiResponse<Tenista>.ErrorResponse(
                $"API Error: {ex.Message}", 
                (int)ex.StatusCode);
        }
        catch (Exception ex)
        {
            return TenistaApiResponse<Tenista>.ErrorResponse($"Error: {ex.Message}");
        }
    }

    /// <summary>
    /// Crear un tenista con manejo de errores
    /// </summary>
    public async Task<TenistaApiResponse<Tenista>> CreateTenistaAsync(Tenista tenista)
    {
        try
        {
            var created = await _client.CreateTenistaAsync(tenista);
            return TenistaApiResponse<Tenista>.SuccessResponse(created);
        }
        catch (ApiException ex)
        {
            return TenistaApiResponse<Tenista>.ErrorResponse(
                $"API Error: {ex.Message}", 
                (int)ex.StatusCode);
        }
        catch (Exception ex)
        {
            return TenistaApiResponse<Tenista>.ErrorResponse($"Error: {ex.Message}");
        }
    }

    /// <summary>
    /// Buscar tenistas por nombre
    /// </summary>
    public async Task<TenistaApiResponse<List<Tenista>>> SearchTenistasAsync(string nombre, string? pais = null)
    {
        try
        {
            var tenistas = await _client.SearchTenistasAsync(nombre, pais);
            return TenistaApiResponse<List<Tenista>>.SuccessResponse(tenistas);
        }
        catch (ApiException ex)
        {
            return TenistaApiResponse<List<Tenista>>.ErrorResponse(
                $"API Error: {ex.Message}", 
                (int)ex.StatusCode);
        }
        catch (Exception ex)
        {
            return TenistaApiResponse<List<Tenista>>.ErrorResponse($"Error: {ex.Message}");
        }
    }
}
