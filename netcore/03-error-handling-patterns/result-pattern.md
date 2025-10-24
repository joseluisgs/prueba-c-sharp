# 🟢 Result Pattern - Functional Error Handling

## Introducción

El **Result Pattern** es un enfoque funcional para manejar errores sin usar exceptions. En lugar de `throw`, retornamos un tipo `Result<T, E>` que puede ser `Success` o `Failure`.

Este patrón viene de la programación funcional y es el estándar en lenguajes como Rust, F#, y Haskell.

## 📋 Conceptos Fundamentales

### ¿Qué es Result<T, E>?

Result es un **discriminated union** (también llamado "sum type") que puede ser:

```csharp
Result<CategoriaDto, AppError>  // Success: CategoriaDto ó Failure: AppError
```

- **`T`**: Tipo del valor de éxito (CategoriaDto)
- **`E`**: Tipo del error (AppError)

### Equivalentes en otros lenguajes

| Lenguaje | Tipo |
|----------|------|
| **Rust** | `Result<T, E>` |
| **F#** | `Result<'T, 'TError>` |
| **Haskell** | `Either a b` |
| **Scala** | `Either[A, B]` |
| **Java (Vavr)** | `Either<L, R>` |
| **C# (nosotros)** | `Result<T, E>` |

## 🏗️ Implementación desde Cero

### 1. Definir AppError (Tipo de Error)

```csharp
public enum ErrorType
{
    NotFound,
    Validation,
    Business,
    Internal
}

public record AppError
{
    public ErrorType Type { get; init; }
    public string Message { get; init; }
    public Dictionary<string, string>? ValidationErrors { get; init; }
    
    private AppError(ErrorType type, string message, 
        Dictionary<string, string>? validationErrors = null)
    {
        Type = type;
        Message = message;
        ValidationErrors = validationErrors;
    }
    
    // Factory methods
    public static AppError NotFound(string message) =>
        new(ErrorType.NotFound, message);
    
    public static AppError Validation(string message, 
        Dictionary<string, string> errors) =>
        new(ErrorType.Validation, message, errors);
    
    public static AppError Business(string message) =>
        new(ErrorType.Business, message);
    
    public static AppError Internal(string message) =>
        new(ErrorType.Internal, message);
}
```

### 2. Implementar Result<T, E>

```csharp
public class Result<T, E>
{
    private readonly T? _value;
    private readonly E? _error;
    private readonly bool _isSuccess;
    
    private Result(T value)
    {
        _value = value;
        _error = default;
        _isSuccess = true;
    }
    
    private Result(E error)
    {
        _value = default;
        _error = error;
        _isSuccess = false;
    }
    
    public bool IsSuccess => _isSuccess;
    public bool IsFailure => !_isSuccess;
    
    public T Value => _isSuccess 
        ? _value! 
        : throw new InvalidOperationException("Cannot access value of a failure result");
    
    public E Error => !_isSuccess 
        ? _error! 
        : throw new InvalidOperationException("Cannot access error of a success result");
    
    // Factory methods
    public static Result<T, E> Success(T value) => new(value);
    public static Result<T, E> Failure(E error) => new(error);
    
    // Pattern matching helper
    public TResult Match<TResult>(
        Func<T, TResult> onSuccess,
        Func<E, TResult> onFailure)
    {
        return _isSuccess ? onSuccess(_value!) : onFailure(_error!);
    }
}
```

## 🔄 Railway Oriented Programming

### El Concepto de "Railway"

Imagina dos vías de tren:
- **Vía superior**: Success path (todo funciona)
- **Vía inferior**: Failure path (errores)

```
Input ──→ [Operation 1] ──→ [Operation 2] ──→ [Operation 3] ──→ Output ✅
             ↓ error           ↓ error           ↓ error
          ────────────────────────────────────────────────────→ Error ❌
```

Una vez que entramos en el "failure track", **permanecemos ahí**.

### Operaciones del Railway

#### Map - Transformar el éxito

```csharp
public Result<TNew, E> Map<TNew>(Func<T, TNew> func)
{
    return _isSuccess 
        ? Result<TNew, E>.Success(func(_value!))
        : Result<TNew, E>.Failure(_error!);
}
```

**Uso:**
```csharp
Result<int, string> result = Result<int, string>.Success(5);
Result<int, string> doubled = result.Map(x => x * 2); // Success(10)

Result<int, string> error = Result<int, string>.Failure("Error");
Result<int, string> mapped = error.Map(x => x * 2); // Failure("Error") - sin ejecutar
```

#### Bind - Encadenar operaciones que retornan Result

```csharp
public Result<TNew, E> Bind<TNew>(Func<T, Result<TNew, E>> func)
{
    return _isSuccess 
        ? func(_value!)
        : Result<TNew, E>.Failure(_error!);
}
```

**Uso:**
```csharp
Result<int, string> GetUserId() => Result<int, string>.Success(42);
Result<User, string> GetUserById(int id) => 
    id > 0 
        ? Result<User, string>.Success(new User { Id = id })
        : Result<User, string>.Failure("Invalid ID");

// Encadenar operaciones
Result<User, string> result = GetUserId()
    .Bind(id => GetUserById(id)); // Success(User) o Failure("Invalid ID")
```

#### Tap - Ejecutar side effects sin cambiar el Result

```csharp
public Result<T, E> Tap(Action<T> action)
{
    if (_isSuccess)
        action(_value!);
    return this;
}
```

**Uso:**
```csharp
var result = GetUser()
    .Tap(user => _logger.LogInformation("Found user: {Id}", user.Id))
    .Tap(user => _cache.Set(user.Id, user))
    .Map(user => _mapper.Map<UserDto>(user));
```

## 🏗️ Implementación Completa en Service Layer

### Java con Vavr Either

```java
@Service
public class ProductoService {
    
    @Autowired
    private ProductoRepository repository;
    
    @Autowired
    private CategoriaRepository categoriaRepository;
    
    @Autowired
    private ModelMapper mapper;
    
    public Either<AppError, ProductoDto> findById(Long id) {
        return repository.findById(id)
            .map(producto -> mapper.map(producto, ProductoDto.class))
            .map(Either::<AppError, ProductoDto>right)
            .orElse(Either.left(
                AppError.notFound("Producto con ID " + id + " no encontrado")));
    }
    
    public Either<AppError, ProductoDto> save(ProductoCreateDto dto) {
        // Validación
        if (dto.getPrecio() <= 0) {
            return Either.left(
                AppError.validation("El precio debe ser mayor que 0"));
        }
        
        // Verificar categoría existe
        if (!categoriaRepository.existsById(dto.getCategoriaId())) {
            return Either.left(
                AppError.notFound("Categoría no encontrada"));
        }
        
        // Validación de negocio
        if (repository.existsByNombre(dto.getNombre())) {
            return Either.left(
                AppError.business("Ya existe un producto con ese nombre"));
        }
        
        Producto producto = mapper.map(dto, Producto.class);
        producto = repository.save(producto);
        ProductoDto result = mapper.map(producto, ProductoDto.class);
        
        return Either.right(result);
    }
    
    public Either<AppError, Void> deleteById(Long id) {
        return repository.findById(id)
            .map(producto -> {
                producto.setIsDeleted(true);
                repository.save(producto);
                return Either.<AppError, Void>right(null);
            })
            .orElse(Either.left(
                AppError.notFound("Producto con ID " + id + " no encontrado")));
    }
}
```

### C# con Result Pattern

```csharp
public class ProductoService
{
    private readonly IProductoRepository _repository;
    private readonly ICategoriaRepository _categoriaRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<ProductoService> _logger;
    
    public ProductoService(
        IProductoRepository repository,
        ICategoriaRepository categoriaRepository,
        IMapper mapper,
        ILogger<ProductoService> logger)
    {
        _repository = repository;
        _categoriaRepository = categoriaRepository;
        _mapper = mapper;
        _logger = logger;
    }
    
    public async Task<Result<ProductoDto, AppError>> FindByIdAsync(long id)
    {
        var producto = await _repository.FindByIdAsync(id);
        
        if (producto == null)
            return Result<ProductoDto, AppError>.Failure(
                AppError.NotFound($"Producto con ID {id} no encontrado"));
        
        var dto = _mapper.Map<ProductoDto>(producto);
        return Result<ProductoDto, AppError>.Success(dto);
    }
    
    public async Task<Result<IEnumerable<ProductoDto>, AppError>> FindAllAsync()
    {
        var productos = await _repository.FindAllAsync();
        var dtos = _mapper.Map<IEnumerable<ProductoDto>>(productos);
        return Result<IEnumerable<ProductoDto>, AppError>.Success(dtos);
    }
    
    public async Task<Result<ProductoDto, AppError>> SaveAsync(ProductoCreateDto dto)
    {
        // Validación
        if (dto.Precio <= 0)
            return Result<ProductoDto, AppError>.Failure(
                AppError.Validation("El precio debe ser mayor que 0", 
                    new Dictionary<string, string> { { "precio", "Debe ser mayor que 0" } }));
        
        if (string.IsNullOrWhiteSpace(dto.Nombre))
            return Result<ProductoDto, AppError>.Failure(
                AppError.Validation("El nombre es requerido",
                    new Dictionary<string, string> { { "nombre", "Campo requerido" } }));
        
        // Verificar categoría existe
        var categoriaExists = await _categoriaRepository.ExistsByIdAsync(dto.CategoriaId);
        if (!categoriaExists)
            return Result<ProductoDto, AppError>.Failure(
                AppError.NotFound($"Categoría con ID {dto.CategoriaId} no encontrada"));
        
        // Validación de negocio
        var nombreExists = await _repository.ExistsByNombreAsync(dto.Nombre);
        if (nombreExists)
            return Result<ProductoDto, AppError>.Failure(
                AppError.Business($"Ya existe un producto con el nombre: {dto.Nombre}"));
        
        // Todo OK - crear producto
        var producto = _mapper.Map<Producto>(dto);
        producto = await _repository.SaveAsync(producto);
        
        var resultDto = _mapper.Map<ProductoDto>(producto);
        return Result<ProductoDto, AppError>.Success(resultDto);
    }
    
    public async Task<Result<ProductoDto, AppError>> UpdateAsync(long id, ProductoUpdateDto dto)
    {
        var producto = await _repository.FindByIdAsync(id);
        
        if (producto == null)
            return Result<ProductoDto, AppError>.Failure(
                AppError.NotFound($"Producto con ID {id} no encontrado"));
        
        // Validación
        if (dto.Precio <= 0)
            return Result<ProductoDto, AppError>.Failure(
                AppError.Validation("El precio debe ser mayor que 0",
                    new Dictionary<string, string> { { "precio", "Debe ser mayor que 0" } }));
        
        // Actualizar campos
        producto.Nombre = dto.Nombre;
        producto.Descripcion = dto.Descripcion;
        producto.Precio = dto.Precio;
        producto.Stock = dto.Stock;
        producto.Imagen = dto.Imagen;
        
        producto = await _repository.SaveAsync(producto);
        
        var resultDto = _mapper.Map<ProductoDto>(producto);
        return Result<ProductoDto, AppError>.Success(resultDto);
    }
    
    public async Task<Result<bool, AppError>> DeleteByIdAsync(long id)
    {
        var producto = await _repository.FindByIdAsync(id);
        
        if (producto == null)
            return Result<bool, AppError>.Failure(
                AppError.NotFound($"Producto con ID {id} no encontrado"));
        
        // Soft delete
        producto.IsDeleted = true;
        await _repository.SaveAsync(producto);
        
        return Result<bool, AppError>.Success(true);
    }
}
```

## 🎮 Controller Implementation

### Java Controller con Either

```java
@RestController
@RequestMapping("/api/productos")
public class ProductoController {
    
    @Autowired
    private ProductoService service;
    
    @GetMapping("/{id}")
    public ResponseEntity<?> getById(@PathVariable Long id) {
        return service.findById(id)
            .fold(
                error -> switch (error.getType()) {
                    case NOT_FOUND -> ResponseEntity.notFound().build();
                    case VALIDATION -> ResponseEntity.badRequest().body(error);
                    case BUSINESS -> ResponseEntity.status(HttpStatus.CONFLICT).body(error);
                    default -> ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR).body(error);
                },
                producto -> ResponseEntity.ok(producto)
            );
    }
    
    @PostMapping
    public ResponseEntity<?> create(@RequestBody ProductoCreateDto dto) {
        return service.save(dto)
            .fold(
                error -> switch (error.getType()) {
                    case NOT_FOUND -> ResponseEntity.notFound().build();
                    case VALIDATION -> ResponseEntity.badRequest().body(error);
                    case BUSINESS -> ResponseEntity.status(HttpStatus.CONFLICT).body(error);
                    default -> ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR).body(error);
                },
                producto -> ResponseEntity.status(HttpStatus.CREATED).body(producto)
            );
    }
}
```

### C# Controller con Result Pattern

```csharp
[ApiController]
[Route("api/[controller]")]
public class ProductosController : ControllerBase
{
    private readonly ProductoService _service;
    
    public ProductosController(ProductoService service)
    {
        _service = service;
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id)
    {
        var resultado = await _service.FindByIdAsync(id);
        
        return resultado.Match(
            onSuccess: producto => Ok(producto),
            onFailure: error => error.Type switch
            {
                ErrorType.NotFound => NotFound(new { message = error.Message }),
                ErrorType.Validation => BadRequest(new 
                { 
                    message = error.Message, 
                    errors = error.ValidationErrors 
                }),
                ErrorType.Business => Conflict(new { message = error.Message }),
                _ => StatusCode(500, new { message = error.Message })
            }
        );
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var resultado = await _service.FindAllAsync();
        
        return resultado.Match(
            onSuccess: productos => Ok(productos),
            onFailure: error => StatusCode(500, new { message = error.Message })
        );
    }
    
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ProductoCreateDto dto)
    {
        var resultado = await _service.SaveAsync(dto);
        
        return resultado.Match(
            onSuccess: producto => CreatedAtAction(
                nameof(GetById), 
                new { id = producto.Id }, 
                producto),
            onFailure: error => error.Type switch
            {
                ErrorType.NotFound => NotFound(new { message = error.Message }),
                ErrorType.Validation => BadRequest(new 
                { 
                    message = error.Message, 
                    errors = error.ValidationErrors 
                }),
                ErrorType.Business => Conflict(new { message = error.Message }),
                _ => StatusCode(500, new { message = error.Message })
            }
        );
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(long id, [FromBody] ProductoUpdateDto dto)
    {
        var resultado = await _service.UpdateAsync(id, dto);
        
        return resultado.Match(
            onSuccess: producto => Ok(producto),
            onFailure: error => error.Type switch
            {
                ErrorType.NotFound => NotFound(new { message = error.Message }),
                ErrorType.Validation => BadRequest(new 
                { 
                    message = error.Message, 
                    errors = error.ValidationErrors 
                }),
                ErrorType.Business => Conflict(new { message = error.Message }),
                _ => StatusCode(500, new { message = error.Message })
            }
        );
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        var resultado = await _service.DeleteByIdAsync(id);
        
        return resultado.Match(
            onSuccess: _ => NoContent(),
            onFailure: error => error.Type switch
            {
                ErrorType.NotFound => NotFound(new { message = error.Message }),
                _ => StatusCode(500, new { message = error.Message })
            }
        );
    }
}
```

## 🔄 Railway Operations - Ejemplos Avanzados

### Encadenar múltiples operaciones

```csharp
public async Task<Result<OrderDto, AppError>> CreateOrderAsync(CreateOrderDto dto)
{
    return await ValidateOrderDto(dto)
        .Bind(validDto => CheckProductStock(validDto))
        .Bind(validDto => CheckUserCredit(validDto))
        .Tap(order => _logger.LogInformation("Creating order for user {UserId}", order.UserId))
        .Bind(validDto => SaveOrderToDatabase(validDto))
        .Tap(order => _eventBus.Publish(new OrderCreatedEvent(order.Id)))
        .Map(order => _mapper.Map<OrderDto>(order));
}

private Result<CreateOrderDto, AppError> ValidateOrderDto(CreateOrderDto dto)
{
    if (dto.Items == null || !dto.Items.Any())
        return Result<CreateOrderDto, AppError>.Failure(
            AppError.Validation("Order must have at least one item", 
                new Dictionary<string, string> { { "items", "Required" } }));
    
    return Result<CreateOrderDto, AppError>.Success(dto);
}

private async Task<Result<CreateOrderDto, AppError>> CheckProductStock(CreateOrderDto dto)
{
    foreach (var item in dto.Items)
    {
        var producto = await _productoRepository.FindByIdAsync(item.ProductoId);
        if (producto == null)
            return Result<CreateOrderDto, AppError>.Failure(
                AppError.NotFound($"Producto {item.ProductoId} no encontrado"));
        
        if (producto.Stock < item.Cantidad)
            return Result<CreateOrderDto, AppError>.Failure(
                AppError.Business($"Stock insuficiente para producto {producto.Nombre}"));
    }
    
    return Result<CreateOrderDto, AppError>.Success(dto);
}
```

## ✅ Ventajas del Result Pattern

1. **Type-safe**: Errores en compile-time
2. **Explicit**: El tipo de retorno muestra que puede fallar
3. **No exceptions**: Sin overhead de performance
4. **Composable**: Fácil de encadenar operaciones
5. **Testeable**: Tests más simples
6. **Railway Oriented**: Flujo claro y predecible
7. **Functional**: Estilo de programación funcional

## ❌ Desventajas del Result Pattern

1. **Learning curve**: Conceptos funcionales nuevos
2. **Verbosity**: Más código que throw/catch
3. **No familiar**: Team acostumbrado a exceptions
4. **Type complexity**: Result<T, E> puede ser complejo
5. **Library dependency**: Necesitas CSharpFunctionalExtensions o similar

## 📊 Comparativa Directa

| Aspecto | Exceptions | Result Pattern |
|---------|-----------|---------------|
| **Performance** | ~100,000 ns | ~500 ns |
| **Type Safety** | Runtime | Compile-time |
| **Testability** | Complex mocking | Simple assertions |
| **Readability** | Hidden flow | Explicit flow |
| **Composability** | Try/catch nesting | Bind/Map chaining |
| **Team learning** | Easy (familiar) | Medium (new concepts) |

## 🧪 Testing con Result Pattern

```csharp
[Test]
public async Task FindByIdAsync_NotFound_ReturnsFailure()
{
    // Arrange
    _repositoryMock.Setup(r => r.FindByIdAsync(999))
        .ReturnsAsync((Producto)null);
    
    // Act
    var result = await _service.FindByIdAsync(999);
    
    // Assert
    Assert.That(result.IsFailure, Is.True);
    Assert.That(result.Error.Type, Is.EqualTo(ErrorType.NotFound));
    Assert.That(result.Error.Message, Does.Contain("999"));
}

[Test]
public async Task FindByIdAsync_Success_ReturnsProducto()
{
    // Arrange
    var producto = new Producto { Id = 1, Nombre = "Test" };
    _repositoryMock.Setup(r => r.FindByIdAsync(1))
        .ReturnsAsync(producto);
    
    // Act
    var result = await _service.FindByIdAsync(1);
    
    // Assert
    Assert.That(result.IsSuccess, Is.True);
    Assert.That(result.Value.Nombre, Is.EqualTo("Test"));
}

[Test]
public async Task SaveAsync_InvalidPrice_ReturnsValidationError()
{
    // Arrange
    var dto = new ProductoCreateDto { Nombre = "Test", Precio = -10 };
    
    // Act
    var result = await _service.SaveAsync(dto);
    
    // Assert
    Assert.That(result.IsFailure, Is.True);
    Assert.That(result.Error.Type, Is.EqualTo(ErrorType.Validation));
    Assert.That(result.Error.ValidationErrors, Contains.Key("precio"));
}
```

## 🎯 Cuándo Usar Result Pattern

### ✅ USA Result Pattern cuando:

1. **Performance crítica**: Necesitas máxima velocidad
2. **Functional style**: Tu team prefiere FP
3. **Type safety**: Quieres compile-time checking
4. **Complex business logic**: Múltiples paths de error
5. **Composable operations**: Encadenamiento de operaciones
6. **Clean testing**: Tests simples y claros

### ❌ EVITA Result Pattern cuando:

1. **Team nuevo en FP**: Curva de aprendizaje alta
2. **Quick prototype**: Necesitas velocidad de desarrollo
3. **Simple CRUD**: Overkill para operaciones básicas
4. **Legacy migration**: Migración directa desde Java
5. **No library support**: No puedes usar CSharpFunctionalExtensions

## 📚 Librería Recomendada: CSharpFunctionalExtensions

En lugar de implementar Result desde cero, usa la librería popular:

```bash
dotnet add package CSharpFunctionalExtensions
```

**Uso:**
```csharp
using CSharpFunctionalExtensions;

public async Task<Result<ProductoDto>> FindByIdAsync(long id)
{
    var producto = await _repository.FindByIdAsync(id);
    
    if (producto == null)
        return Result.Failure<ProductoDto>("Producto no encontrado");
    
    return Result.Success(_mapper.Map<ProductoDto>(producto));
}
```

## 🔗 Referencias

- [Railway Oriented Programming](https://fsharpforfunandprofit.com/rop/)
- [CSharpFunctionalExtensions](https://github.com/vkhorikov/CSharpFunctionalExtensions)
- [Functional C# - Vladimir Khorikov](https://enterprisecraftsmanship.com/posts/functional-c-handling-failures-input-errors/)
- [Java Vavr Library](https://www.vavr.io/)

---

**Anterior:** [← Traditional Exceptions](./traditional-exceptions.md) | **Siguiente:** [When to Use →](./when-to-use.md)
