# 🔴 Traditional Exception Handling

## Introducción

El manejo tradicional de excepciones es el enfoque más familiar para desarrolladores que vienen de Java/Spring Boot. Este documento explica cómo implementar exception handling en ASP.NET Core y lo compara con Spring Boot.

## 📋 Conceptos Clave

### En Java/Spring Boot
```java
@ControllerAdvice
public class GlobalExceptionHandler {
    @ExceptionHandler(NotFoundException.class)
    public ResponseEntity<ErrorResponse> handleNotFound(NotFoundException ex) {
        ErrorResponse error = new ErrorResponse(ex.getMessage());
        return ResponseEntity.status(HttpStatus.NOT_FOUND).body(error);
    }
}
```

### En C#/ASP.NET Core
```csharp
public class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is NotFoundException notFoundEx)
        {
            httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
            await httpContext.Response.WriteAsJsonAsync(
                new { message = notFoundEx.Message },
                cancellationToken);
            return true;
        }
        
        return false;
    }
}
```

## 🏗️ Arquitectura Completa

### 1. Custom Exceptions

**Java:**
```java
public class NotFoundException extends RuntimeException {
    public NotFoundException(String message) {
        super(message);
    }
}

public class ValidationException extends RuntimeException {
    private Map<String, String> errors;
    
    public ValidationException(Map<String, String> errors) {
        super("Validation failed");
        this.errors = errors;
    }
    
    public Map<String, String> getErrors() {
        return errors;
    }
}

public class BusinessException extends RuntimeException {
    public BusinessException(String message) {
        super(message);
    }
}
```

**C#:**
```csharp
public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }
}

public class ValidationException : Exception
{
    public Dictionary<string, string> Errors { get; }
    
    public ValidationException(Dictionary<string, string> errors) 
        : base("Validation failed")
    {
        Errors = errors;
    }
}

public class BusinessException : Exception
{
    public BusinessException(string message) : base(message) { }
}
```

### 2. Service Layer

**Java Service:**
```java
@Service
public class CategoriaService {
    
    @Autowired
    private CategoriaRepository repository;
    
    @Autowired
    private ModelMapper mapper;
    
    public CategoriaDto findById(Long id) {
        Categoria categoria = repository.findById(id)
            .orElseThrow(() -> new NotFoundException(
                "Categoría con ID " + id + " no encontrada"));
        
        return mapper.map(categoria, CategoriaDto.class);
    }
    
    public CategoriaDto save(CategoriaCreateDto dto) {
        // Validación
        if (dto.getNombre() == null || dto.getNombre().isBlank()) {
            Map<String, String> errors = new HashMap<>();
            errors.put("nombre", "El nombre es requerido");
            throw new ValidationException(errors);
        }
        
        // Validación de negocio
        if (repository.existsByNombre(dto.getNombre())) {
            throw new BusinessException(
                "Ya existe una categoría con el nombre: " + dto.getNombre());
        }
        
        Categoria categoria = mapper.map(dto, Categoria.class);
        categoria = repository.save(categoria);
        return mapper.map(categoria, CategoriaDto.class);
    }
    
    public void deleteById(Long id) {
        Categoria categoria = repository.findById(id)
            .orElseThrow(() -> new NotFoundException(
                "Categoría con ID " + id + " no encontrada"));
        
        // Soft delete
        categoria.setIsDeleted(true);
        repository.save(categoria);
    }
}
```

**C# Service:**
```csharp
public class CategoriaService
{
    private readonly ICategoriaRepository _repository;
    private readonly IMapper _mapper;
    
    public CategoriaService(ICategoriaRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }
    
    public async Task<CategoriaDto> FindByIdAsync(long id)
    {
        var categoria = await _repository.FindByIdAsync(id);
        
        if (categoria == null)
            throw new NotFoundException($"Categoría con ID {id} no encontrada");
        
        return _mapper.Map<CategoriaDto>(categoria);
    }
    
    public async Task<CategoriaDto> SaveAsync(CategoriaCreateDto dto)
    {
        // Validación
        if (string.IsNullOrWhiteSpace(dto.Nombre))
        {
            var errors = new Dictionary<string, string>
            {
                { "nombre", "El nombre es requerido" }
            };
            throw new ValidationException(errors);
        }
        
        // Validación de negocio
        if (await _repository.ExistsByNombreAsync(dto.Nombre))
        {
            throw new BusinessException(
                $"Ya existe una categoría con el nombre: {dto.Nombre}");
        }
        
        var categoria = _mapper.Map<Categoria>(dto);
        categoria = await _repository.SaveAsync(categoria);
        return _mapper.Map<CategoriaDto>(categoria);
    }
    
    public async Task DeleteByIdAsync(long id)
    {
        var categoria = await _repository.FindByIdAsync(id);
        
        if (categoria == null)
            throw new NotFoundException($"Categoría con ID {id} no encontrada");
        
        // Soft delete
        categoria.IsDeleted = true;
        await _repository.SaveAsync(categoria);
    }
}
```

### 3. Controller Layer

**Java Controller:**
```java
@RestController
@RequestMapping("/api/categorias")
public class CategoriaController {
    
    @Autowired
    private CategoriaService service;
    
    @GetMapping("/{id}")
    public ResponseEntity<CategoriaDto> getById(@PathVariable Long id) {
        try {
            CategoriaDto categoria = service.findById(id);
            return ResponseEntity.ok(categoria);
        } catch (NotFoundException ex) {
            return ResponseEntity.notFound().build();
        }
    }
    
    @GetMapping
    public ResponseEntity<List<CategoriaDto>> getAll() {
        List<CategoriaDto> categorias = service.findAll();
        return ResponseEntity.ok(categorias);
    }
    
    @PostMapping
    public ResponseEntity<CategoriaDto> create(@RequestBody CategoriaCreateDto dto) {
        try {
            CategoriaDto categoria = service.save(dto);
            return ResponseEntity.status(HttpStatus.CREATED).body(categoria);
        } catch (ValidationException ex) {
            return ResponseEntity.badRequest().body(null);
        } catch (BusinessException ex) {
            return ResponseEntity.status(HttpStatus.CONFLICT).body(null);
        }
    }
    
    @DeleteMapping("/{id}")
    public ResponseEntity<Void> delete(@PathVariable Long id) {
        try {
            service.deleteById(id);
            return ResponseEntity.noContent().build();
        } catch (NotFoundException ex) {
            return ResponseEntity.notFound().build();
        }
    }
}
```

**C# Controller:**
```csharp
[ApiController]
[Route("api/[controller]")]
public class CategoriasController : ControllerBase
{
    private readonly CategoriaService _service;
    
    public CategoriasController(CategoriaService service)
    {
        _service = service;
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id)
    {
        try
        {
            var categoria = await _service.FindByIdAsync(id);
            return Ok(categoria);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var categorias = await _service.FindAllAsync();
        return Ok(categorias);
    }
    
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CategoriaCreateDto dto)
    {
        try
        {
            var categoria = await _service.SaveAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = categoria.Id }, categoria);
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { errors = ex.Errors });
        }
        catch (BusinessException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        try
        {
            await _service.DeleteByIdAsync(id);
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
```

### 4. Global Exception Handler

**Java @ControllerAdvice:**
```java
@ControllerAdvice
public class GlobalExceptionHandler {
    
    @ExceptionHandler(NotFoundException.class)
    public ResponseEntity<ErrorResponse> handleNotFound(NotFoundException ex) {
        ErrorResponse error = new ErrorResponse(
            HttpStatus.NOT_FOUND.value(),
            ex.getMessage(),
            LocalDateTime.now()
        );
        return ResponseEntity.status(HttpStatus.NOT_FOUND).body(error);
    }
    
    @ExceptionHandler(ValidationException.class)
    public ResponseEntity<ValidationErrorResponse> handleValidation(
        ValidationException ex) {
        ValidationErrorResponse error = new ValidationErrorResponse(
            HttpStatus.BAD_REQUEST.value(),
            "Validation failed",
            ex.getErrors(),
            LocalDateTime.now()
        );
        return ResponseEntity.badRequest().body(error);
    }
    
    @ExceptionHandler(BusinessException.class)
    public ResponseEntity<ErrorResponse> handleBusiness(BusinessException ex) {
        ErrorResponse error = new ErrorResponse(
            HttpStatus.CONFLICT.value(),
            ex.getMessage(),
            LocalDateTime.now()
        );
        return ResponseEntity.status(HttpStatus.CONFLICT).body(error);
    }
    
    @ExceptionHandler(Exception.class)
    public ResponseEntity<ErrorResponse> handleGeneral(Exception ex) {
        ErrorResponse error = new ErrorResponse(
            HttpStatus.INTERNAL_SERVER_ERROR.value(),
            "Internal server error",
            LocalDateTime.now()
        );
        return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR).body(error);
    }
}
```

**C# IExceptionHandler:**
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
                new { message = notFoundEx.Message }
            ),
            ValidationException validationEx => (
                StatusCodes.Status400BadRequest,
                new { message = "Validation failed", errors = validationEx.Errors }
            ),
            BusinessException businessEx => (
                StatusCodes.Status409Conflict,
                new { message = businessEx.Message }
            ),
            _ => (
                StatusCodes.Status500InternalServerError,
                new { message = "Internal server error" }
            )
        };
        
        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);
        
        return true;
    }
}
```

### 5. Configuration

**Java (Spring Boot):**
```java
// No configuration needed - @ControllerAdvice is auto-detected
```

**C# (Program.cs):**
```csharp
var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddScoped<CategoriaService>();
builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Add exception handler
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

// Use exception handler
app.UseExceptionHandler();

app.MapControllers();
app.Run();
```

## ✅ Ventajas del Enfoque Tradicional

1. **Familiar**: Todos los desarrolladores conocen try/catch
2. **Simple**: Fácil de entender y explicar
3. **Centralizado**: GlobalExceptionHandler maneja todos los errores
4. **Separación**: Lógica de negocio separada del manejo de errores
5. **Stack traces**: Información completa de debugging
6. **Migration friendly**: Fácil migrar desde Java/Spring Boot

## ❌ Desventajas del Enfoque Tradicional

1. **Hidden control flow**: Las exceptions saltan el flujo normal
2. **Performance overhead**: Stack unwinding es costoso
3. **No type-safe**: Errores en runtime, no compile-time
4. **Try/catch pollution**: Controllers llenos de try/catch
5. **Difficult testing**: Requiere mocking de exceptions
6. **Lost context**: Stack unwinding puede perder contexto

## 📊 Performance Considerations

### Benchmark Comparison

```csharp
// Exception-based: ~100,000 ns
public async Task<CategoriaDto> FindByIdWithException(long id)
{
    var categoria = await _repository.FindByIdAsync(id);
    if (categoria == null)
        throw new NotFoundException("Not found"); // ❌ Slow
    return _mapper.Map<CategoriaDto>(categoria);
}

// Result-based: ~500 ns  
public async Task<Result<CategoriaDto>> FindByIdWithResult(long id)
{
    var categoria = await _repository.FindByIdAsync(id);
    if (categoria == null)
        return Result.Failure("Not found"); // ✅ Fast
    return Result.Success(_mapper.Map<CategoriaDto>(categoria));
}
```

**Exception throwing es ~200x más lento** que retornar un Result.

## 🧪 Testing

**Java Test:**
```java
@Test
public void testFindById_NotFound() {
    when(repository.findById(999L)).thenReturn(Optional.empty());
    
    assertThrows(NotFoundException.class, () -> {
        service.findById(999L);
    });
}

@Test
public void testFindById_Success() {
    Categoria categoria = new Categoria(1L, "Electrónica");
    when(repository.findById(1L)).thenReturn(Optional.of(categoria));
    
    CategoriaDto result = service.findById(1L);
    
    assertNotNull(result);
    assertEquals("Electrónica", result.getNombre());
}
```

**C# Test:**
```csharp
[Test]
public void FindByIdAsync_NotFound_ThrowsNotFoundException()
{
    // Arrange
    _repositoryMock.Setup(r => r.FindByIdAsync(999))
        .ReturnsAsync((Categoria)null);
    
    // Act & Assert
    Assert.ThrowsAsync<NotFoundException>(async () => 
        await _service.FindByIdAsync(999));
}

[Test]
public async Task FindByIdAsync_Success_ReturnsCategoria()
{
    // Arrange
    var categoria = new Categoria { Id = 1, Nombre = "Electrónica" };
    _repositoryMock.Setup(r => r.FindByIdAsync(1))
        .ReturnsAsync(categoria);
    
    // Act
    var result = await _service.FindByIdAsync(1);
    
    // Assert
    Assert.That(result, Is.Not.Null);
    Assert.That(result.Nombre, Is.EqualTo("Electrónica"));
}
```

## 🎯 Cuándo Usar Exception Handling

### ✅ USA Exceptions cuando:

1. **Migrando desde Java/Spring Boot**: Mantén el mismo patrón inicialmente
2. **Team nuevo en .NET**: Familiar y fácil de entender
3. **CRUD simple**: No requiere complejidad adicional
4. **Prototipos rápidos**: Velocity sobre performance
5. **Errores excepcionales**: Situaciones verdaderamente excepcionales

### ❌ EVITA Exceptions cuando:

1. **Performance crítica**: Millones de requests/segundo
2. **Lógica de negocio compleja**: Múltiples paths de error
3. **Funcional style preferred**: Team con background funcional
4. **Testing intensivo**: Requieres tests simples y rápidos
5. **Type safety importante**: Quieres compile-time error checking

## 📚 Próximos Pasos

Ahora que entiendes el enfoque tradicional, aprende sobre el [Result Pattern](./result-pattern.md) para comparar ambos enfoques.

## 🔗 Referencias

- [ASP.NET Core Exception Handling](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/error-handling)
- [IExceptionHandler Interface](https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.diagnostics.iexceptionhandler)
- [Spring Boot @ControllerAdvice](https://spring.io/blog/2013/11/01/exception-handling-in-spring-mvc)

---

**Anterior:** [← README](./README.md) | **Siguiente:** [Result Pattern →](./result-pattern.md)
