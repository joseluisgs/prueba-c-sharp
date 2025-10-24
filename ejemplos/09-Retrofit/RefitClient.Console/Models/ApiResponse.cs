namespace RefitClient.Console.Models;

public class TenistaApiResponse<T>
{
    public T? Data { get; set; }
    public bool Success { get; set; }
    public string? Message { get; set; }
    public int StatusCode { get; set; }

    public static TenistaApiResponse<T> SuccessResponse(T data) => new()
    {
        Data = data,
        Success = true,
        Message = "OK",
        StatusCode = 200
    };

    public static TenistaApiResponse<T> ErrorResponse(string message, int statusCode = 500) => new()
    {
        Data = default,
        Success = false,
        Message = message,
        StatusCode = statusCode
    };
}
