namespace RefitClient.Console.Models;

public class ErrorResponse
{
    public string? Error { get; set; }
    public string? Message { get; set; }
    public int StatusCode { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public override string ToString() =>
        $"ErrorResponse{{Error='{Error}', Message='{Message}', StatusCode={StatusCode}, Timestamp={Timestamp}}}";
}
