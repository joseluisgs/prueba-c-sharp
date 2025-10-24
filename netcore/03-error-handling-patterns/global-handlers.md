# 🛡️ Global Exception Handlers

## Introducción

Los **Global Exception Handlers** permiten centralizar el manejo de errores en un solo lugar, similar a `@ControllerAdvice` en Spring Boot.

## 🔄 Comparativa: Spring Boot vs ASP.NET Core

### Java Spring Boot - @ControllerAdvice

```java
@ControllerAdvice
@Slf4j
public class GlobalExceptionHandler {
    
    @ExceptionHandler(NotFoundException.class)
    public ResponseEntity<ErrorResponse> handleNotFoundException(
        NotFoundException ex,
        WebRequest request) {
        
        log.error("Not found error: {}", ex.getMessage());
        
        ErrorResponse error = ErrorResponse.builder()
            .timestamp(LocalDateTime.now())
            .status(HttpStatus.NOT_FOUND.value())
            .error("Not Found")
            .message(ex.getMessage())
            .path(request.getDescription(false))
            .build();
        
        return ResponseEntity.status(HttpStatus.NOT_FOUND).body(error);
    }
    
    @ExceptionHandler(ValidationException.class)
    public ResponseEntity<ValidationErrorResponse> handleValidationException(
        ValidationException ex,
        WebRequest request) {
        
        log.error("Validation error: {}", ex.getErrors());
        
        ValidationErrorResponse error = ValidationErrorResponse.builder()
            .timestamp(LocalDateTime.now())
            .status(HttpStatus.BAD_REQUEST.value())
            .error("Validation Failed")
            .message("Request validation failed")
            .errors(ex.getErrors())
            .path(request.getDescription(false))
            .build();
        
        return ResponseEntity.badRequest().body(error);
    }
    
    @ExceptionHandler(Exception.class)
    public ResponseEntity<ErrorResponse> handleGlobalException(
        Exception ex,
        WebRequest request) {
        
        log.error("Unexpected error", ex);
        
        ErrorResponse error = ErrorResponse.builder()
            .timestamp(LocalDateTime.now())
            .status(HttpStatus.INTERNAL_SERVER_ERROR.value())
            .error("Internal Server Error")
            .message("An unexpected error occurred")
            .path(request.getDescription(false))
            .build();
        
        return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR).body(error);
    }
}
```

### C# ASP.NET Core - IExceptionHandler (.NET 8+)

```csharp
public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    
    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }
    
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Exception occurred: {Message}", exception.Message);
        
        var (statusCode, response) = exception switch
        {
            NotFoundException notFoundEx => (
                StatusCodes.Status404NotFound,
                new ErrorResponse
                {
                    Timestamp = DateTime.UtcNow,
                    Status = StatusCodes.Status404NotFound,
                    Error = "Not Found",
                    Message = notFoundEx.Message,
                    Path = httpContext.Request.Path
                }
            ),
            ValidationException validationEx => (
                StatusCodes.Status400BadRequest,
                new ValidationErrorResponse
                {
                    Timestamp = DateTime.UtcNow,
                    Status = StatusCodes.Status400BadRequest,
                    Error = "Validation Failed",
                    Message = "Request validation failed",
                    Errors = validationEx.Errors,
                    Path = httpContext.Request.Path
                }
            ),
            BusinessException businessEx => (
                StatusCodes.Status409Conflict,
                new ErrorResponse
                {
                    Timestamp = DateTime.UtcNow,
                    Status = StatusCodes.Status409Conflict,
                    Error = "Business Rule Violation",
                    Message = businessEx.Message,
                    Path = httpContext.Request.Path
                }
            ),
            _ => (
                StatusCodes.Status500InternalServerError,
                new ErrorResponse
                {
                    Timestamp = DateTime.UtcNow,
                    Status = StatusCodes.Status500InternalServerError,
                    Error = "Internal Server Error",
                    Message = "An unexpected error occurred",
                    Path = httpContext.Request.Path
                }
            )
        };
        
        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/json";
        
        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);
        
        return true; // Exception handled
    }
}
```

## 🏗️ Error Response Models

### Java ErrorResponse

```java
@Data
@Builder
public class ErrorResponse {
    private LocalDateTime timestamp;
    private int status;
    private String error;
    private String message;
    private String path;
}

@Data
@Builder
public class ValidationErrorResponse {
    private LocalDateTime timestamp;
    private int status;
    private String error;
    private String message;
    private Map<String, String> errors;
    private String path;
}
```

### C# ErrorResponse

```csharp
public record ErrorResponse
{
    public DateTime Timestamp { get; init; }
    public int Status { get; init; }
    public string Error { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
    public string Path { get; init; } = string.Empty;
}

public record ValidationErrorResponse
{
    public DateTime Timestamp { get; init; }
    public int Status { get; init; }
    public string Error { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
    public Dictionary<string, string> Errors { get; init; } = new();
    public string Path { get; init; } = string.Empty;
}
```

## ⚙️ Configuration

### Java Spring Boot (application.yml)

```yaml
server:
  error:
    include-message: always
    include-binding-errors: always
    include-stacktrace: on-param
    include-exception: false

logging:
  level:
    com.tuapp: DEBUG
    org.springframework.web: INFO
```

### C# ASP.NET Core (Program.cs)

```csharp
var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();

// Add exception handler
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// Add ProblemDetails support (opcional)
builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        context.ProblemDetails.Instance = context.HttpContext.Request.Path;
        context.ProblemDetails.Extensions["traceId"] = context.HttpContext.TraceIdentifier;
    };
});

var app = builder.Build();

// Use exception handler (debe ir ANTES de otros middleware)
app.UseExceptionHandler();

// Development exception page (solo en desarrollo)
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
```

## 🔧 Middleware Pipeline Order

El **orden del middleware** es crítico en ASP.NET Core:

```csharp
var app = builder.Build();

// 1. Exception Handler (primero para capturar todo)
app.UseExceptionHandler();

// 2. HTTPS Redirection
app.UseHttpsRedirection();

// 3. Static Files
app.UseStaticFiles();

// 4. Routing
app.UseRouting();

// 5. CORS
app.UseCors();

// 6. Authentication
app.UseAuthentication();

// 7. Authorization
app.UseAuthorization();

// 8. Endpoints (último)
app.MapControllers();

app.Run();
```

## 🎨 Legacy Approach: ExceptionFilterAttribute (.NET 7 y anteriores)

Si estás usando .NET 7 o anterior, usa `ExceptionFilterAttribute`:

```csharp
public class GlobalExceptionFilter : IExceptionFilter
{
    private readonly ILogger<GlobalExceptionFilter> _logger;
    
    public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger)
    {
        _logger = logger;
    }
    
    public void OnException(ExceptionContext context)
    {
        _logger.LogError(context.Exception, "Exception occurred: {Message}", 
            context.Exception.Message);
        
        var (statusCode, response) = context.Exception switch
        {
            NotFoundException notFoundEx => (
                StatusCodes.Status404NotFound,
                new { message = notFoundEx.Message }
            ),
            ValidationException validationEx => (
                StatusCodes.Status400BadRequest,
                new { message = "Validation failed", errors = validationEx.Errors }
            ),
            _ => (
                StatusCodes.Status500InternalServerError,
                new { message = "Internal server error" }
            )
        };
        
        context.Result = new ObjectResult(response)
        {
            StatusCode = statusCode
        };
        
        context.ExceptionHandled = true;
    }
}

// Register in Program.cs
builder.Services.AddControllers(options =>
{
    options.Filters.Add<GlobalExceptionFilter>();
});
```

## 🌐 ProblemDetails (RFC 7807)

**ProblemDetails** es un estándar para representar errores HTTP:

```csharp
public class ProblemDetailsExceptionHandler : IExceptionHandler
{
    private readonly ILogger<ProblemDetailsExceptionHandler> _logger;
    
    public ProblemDetailsExceptionHandler(
        ILogger<ProblemDetailsExceptionHandler> logger)
    {
        _logger = logger;
    }
    
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Exception occurred: {Message}", exception.Message);
        
        var problemDetails = exception switch
        {
            NotFoundException notFoundEx => new ProblemDetails
            {
                Title = "Resource Not Found",
                Detail = notFoundEx.Message,
                Status = StatusCodes.Status404NotFound,
                Instance = httpContext.Request.Path,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4"
            },
            ValidationException validationEx => new ValidationProblemDetails(
                validationEx.Errors)
            {
                Title = "Validation Failed",
                Detail = "One or more validation errors occurred",
                Status = StatusCodes.Status400BadRequest,
                Instance = httpContext.Request.Path,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
            },
            BusinessException businessEx => new ProblemDetails
            {
                Title = "Business Rule Violation",
                Detail = businessEx.Message,
                Status = StatusCodes.Status409Conflict,
                Instance = httpContext.Request.Path,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.8"
            },
            _ => new ProblemDetails
            {
                Title = "Internal Server Error",
                Detail = "An unexpected error occurred",
                Status = StatusCodes.Status500InternalServerError,
                Instance = httpContext.Request.Path,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1"
            }
        };
        
        // Add trace ID for debugging
        problemDetails.Extensions["traceId"] = httpContext.TraceIdentifier;
        
        httpContext.Response.StatusCode = problemDetails.Status!.Value;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        
        return true;
    }
}
```

**Ejemplo de respuesta ProblemDetails:**
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.4",
  "title": "Resource Not Found",
  "status": 404,
  "detail": "Producto con ID 999 no encontrado",
  "instance": "/api/productos/999",
  "traceId": "0HMT8R5L9Q7A3"
}
```

## 📝 Logging Best Practices

```csharp
public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    
    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }
    
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        // Different log levels for different exceptions
        switch (exception)
        {
            case NotFoundException:
                _logger.LogWarning(exception, 
                    "Resource not found: {Path}", httpContext.Request.Path);
                break;
            
            case ValidationException:
                _logger.LogInformation(exception,
                    "Validation failed: {Path}", httpContext.Request.Path);
                break;
            
            case BusinessException:
                _logger.LogWarning(exception,
                    "Business rule violation: {Path}", httpContext.Request.Path);
                break;
            
            default:
                _logger.LogError(exception,
                    "Unexpected error occurred: {Path}", httpContext.Request.Path);
                break;
        }
        
        // Handle exception...
        
        return true;
    }
}
```

## 🧪 Testing Global Exception Handlers

### Java Spring Boot Test

```java
@WebMvcTest(ProductoController.class)
class GlobalExceptionHandlerTest {
    
    @Autowired
    private MockMvc mockMvc;
    
    @MockBean
    private ProductoService service;
    
    @Test
    void testNotFoundExceptionHandling() throws Exception {
        when(service.findById(999L))
            .thenThrow(new NotFoundException("Producto no encontrado"));
        
        mockMvc.perform(get("/api/productos/999"))
            .andExpect(status().isNotFound())
            .andExpect(jsonPath("$.message").value("Producto no encontrado"));
    }
    
    @Test
    void testValidationExceptionHandling() throws Exception {
        Map<String, String> errors = Map.of("precio", "Debe ser mayor que 0");
        when(service.save(any()))
            .thenThrow(new ValidationException(errors));
        
        mockMvc.perform(post("/api/productos")
                .contentType(MediaType.APPLICATION_JSON)
                .content("{}"))
            .andExpect(status().isBadRequest())
            .andExpect(jsonPath("$.errors.precio").exists());
    }
}
```

### C# ASP.NET Core Test

```csharp
public class GlobalExceptionHandlerTests
{
    private GlobalExceptionHandler _handler;
    private Mock<ILogger<GlobalExceptionHandler>> _loggerMock;
    private DefaultHttpContext _httpContext;
    
    [SetUp]
    public void Setup()
    {
        _loggerMock = new Mock<ILogger<GlobalExceptionHandler>>();
        _handler = new GlobalExceptionHandler(_loggerMock.Object);
        _httpContext = new DefaultHttpContext();
        _httpContext.Response.Body = new MemoryStream();
    }
    
    [Test]
    public async Task TryHandleAsync_NotFoundException_Returns404()
    {
        // Arrange
        var exception = new NotFoundException("Producto no encontrado");
        
        // Act
        var result = await _handler.TryHandleAsync(
            _httpContext, exception, CancellationToken.None);
        
        // Assert
        Assert.That(result, Is.True);
        Assert.That(_httpContext.Response.StatusCode, Is.EqualTo(404));
    }
    
    [Test]
    public async Task TryHandleAsync_ValidationException_Returns400()
    {
        // Arrange
        var errors = new Dictionary<string, string> 
        { 
            { "precio", "Debe ser mayor que 0" } 
        };
        var exception = new ValidationException(errors);
        
        // Act
        var result = await _handler.TryHandleAsync(
            _httpContext, exception, CancellationToken.None);
        
        // Assert
        Assert.That(result, Is.True);
        Assert.That(_httpContext.Response.StatusCode, Is.EqualTo(400));
        
        // Verify response contains errors
        _httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
        var reader = new StreamReader(_httpContext.Response.Body);
        var responseBody = await reader.ReadToEndAsync();
        Assert.That(responseBody, Does.Contain("precio"));
    }
}
```

## ✅ Ventajas de Global Exception Handlers

1. **Centralización**: Un solo lugar para manejar errores
2. **Consistencia**: Responses uniformes en toda la API
3. **Separation of Concerns**: Controllers no manejan errores
4. **Logging centralizado**: Fácil auditoría y debugging
5. **Fácil testing**: Un solo componente a testear

## ❌ Limitaciones

1. **No aplica a errores fuera del middleware pipeline**
2. **Requiere configuración correcta del orden**
3. **Puede ocultar stack traces importantes**
4. **Dificulta error handling específico por endpoint**

## 🔗 Referencias

- [ASP.NET Core Exception Handling](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/error-handling)
- [IExceptionHandler Interface](https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.diagnostics.iexceptionhandler)
- [ProblemDetails RFC 7807](https://tools.ietf.org/html/rfc7807)
- [Spring Boot @ControllerAdvice](https://spring.io/blog/2013/11/01/exception-handling-in-spring-mvc)

---

**Anterior:** [← When to Use](./when-to-use.md) | **Siguiente:** [Fondamentos ASP.NET →](../01-fundamentos-aspnet/README.md)
