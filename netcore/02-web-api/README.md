# 🚀 De Spring Boot a ASP.NET Core - Guía Completa

## Tabla de Contenidos
- [Introducción](#introducción)
- [Arquitectura y Conceptos](#arquitectura-y-conceptos)
- [Estructura de Proyectos](#estructura-de-proyectos)
- [Configuración](#configuración)
- [Dependency Injection](#dependency-injection)
- [Controladores y Routing](#controladores-y-routing)
- [Middleware vs Filters](#middleware-vs-filters)
- [Validación](#validación)
- [Exception Handling](#exception-handling)

## Introducción

Esta guía compara **Spring Boot 3.x** con **ASP.NET Core 8**, dos de los frameworks web más populares para Java y C# respectivamente.

### Comparación de Alto Nivel

| Aspecto | Spring Boot | ASP.NET Core |
|---------|------------|--------------|
| **Lenguaje** | Java | C# |
| **Paradigma** | Convention over Configuration | Convention over Configuration |
| **DI Container** | Spring IoC | Built-in DI Container |
| **Web Framework** | Spring MVC | ASP.NET Core MVC/Web API |
| **ORM** | JPA/Hibernate | Entity Framework Core |
| **Build Tool** | Maven/Gradle | MSBuild/dotnet CLI |
| **Config Files** | application.properties/yml | appsettings.json |
| **Testing** | JUnit + Mockito | xUnit/NUnit + Moq |

## Arquitectura y Conceptos

### Ciclo de Vida de la Aplicación

#### Spring Boot
```java
@SpringBootApplication
public class Application {
    public static void main(String[] args) {
        SpringApplication.run(Application.class, args);
    }
}
```

#### ASP.NET Core (Minimal API Style)
```csharp
var builder = WebApplication.CreateBuilder(args);

// Configurar servicios (equivalente a @Configuration)
builder.Services.AddControllers();

var app = builder.Build();

// Configurar middleware pipeline
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
```

#### ASP.NET Core (Tradicional con Startup)
```csharp
public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
}

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        // Equivalente a @Configuration
        services.AddControllers();
    }

    public void Configure(IApplicationBuilder app)
    {
        // Configurar middleware pipeline
        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthorization();
        app.UseEndpoints(endpoints => endpoints.MapControllers());
    }
}
```

## Estructura de Proyectos

### Spring Boot (Estructura Típica)
```
spring-boot-app/
├── src/
│   ├── main/
│   │   ├── java/
│   │   │   └── com/
│   │   │       └── ejemplo/
│   │   │           ├── Application.java
│   │   │           ├── controller/
│   │   │           │   └── ProductoController.java
│   │   │           ├── service/
│   │   │           │   ├── ProductoService.java
│   │   │           │   └── ProductoServiceImpl.java
│   │   │           ├── repository/
│   │   │           │   └── ProductoRepository.java
│   │   │           ├── model/
│   │   │           │   └── Producto.java
│   │   │           ├── dto/
│   │   │           │   └── ProductoDTO.java
│   │   │           └── config/
│   │   │               └── DatabaseConfig.java
│   │   └── resources/
│   │       ├── application.properties
│   │       ├── application-dev.properties
│   │       └── application-prod.properties
│   └── test/
│       └── java/
├── pom.xml (o build.gradle)
└── README.md
```

### ASP.NET Core (Estructura Típica)
```
AspNetCoreApp/
├── Controllers/
│   └── ProductoController.cs
├── Services/
│   ├── IProductoService.cs
│   └── ProductoService.cs
├── Repositories/
│   ├── IProductoRepository.cs
│   └── ProductoRepository.cs
├── Models/
│   └── Producto.cs
├── DTOs/
│   └── ProductoDTO.cs
├── Data/
│   └── ApplicationDbContext.cs
├── Middleware/
│   └── ExceptionMiddleware.cs
├── appsettings.json
├── appsettings.Development.json
├── appsettings.Production.json
├── Program.cs
├── AspNetCoreApp.csproj
└── README.md
```

## Configuración

### Archivos de Configuración

#### Spring Boot (application.properties)
```properties
# Server
server.port=8080
server.servlet.context-path=/api

# Database
spring.datasource.url=jdbc:postgresql://localhost:5432/productos_db
spring.datasource.username=admin
spring.datasource.password=admin123
spring.jpa.hibernate.ddl-auto=update
spring.jpa.show-sql=true

# Logging
logging.level.root=INFO
logging.level.com.ejemplo=DEBUG
```

#### ASP.NET Core (appsettings.json)
```json
{
  "Kestrel": {
    "Endpoints": {
      "Http": {
        "Url": "http://localhost:5000"
      }
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=productos_db;Username=admin;Password=admin123"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Com.Ejemplo": "Debug"
    }
  }
}
```

### Leer Configuración

#### Spring Boot
```java
@Configuration
public class DatabaseConfig {
    @Value("${spring.datasource.url}")
    private String dbUrl;
    
    @Value("${spring.datasource.username}")
    private String dbUsername;
}

// O con @ConfigurationProperties
@Configuration
@ConfigurationProperties(prefix = "spring.datasource")
public class DataSourceProperties {
    private String url;
    private String username;
    private String password;
    // getters y setters
}
```

#### ASP.NET Core
```csharp
// En Program.cs o Startup.cs
var configuration = builder.Configuration;
var dbUrl = configuration["ConnectionStrings:DefaultConnection"];

// O con Options Pattern
public class DatabaseSettings
{
    public string Url { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
}

// Registrar en DI
builder.Services.Configure<DatabaseSettings>(
    builder.Configuration.GetSection("Database"));

// Inyectar en controllers/services
public class ProductoService
{
    private readonly DatabaseSettings _settings;
    
    public ProductoService(IOptions<DatabaseSettings> settings)
    {
        _settings = settings.Value;
    }
}
```

## Dependency Injection

### Registro de Servicios

#### Spring Boot
```java
// Interfaces
public interface ProductoService {
    List<Producto> findAll();
}

// Implementación
@Service  // o @Component
public class ProductoServiceImpl implements ProductoService {
    private final ProductoRepository repository;
    
    @Autowired  // opcional en constructor único
    public ProductoServiceImpl(ProductoRepository repository) {
        this.repository = repository;
    }
    
    @Override
    public List<Producto> findAll() {
        return repository.findAll();
    }
}

// Repositorio
@Repository
public interface ProductoRepository extends JpaRepository<Producto, Long> {
}

// Configuración adicional
@Configuration
public class AppConfig {
    @Bean
    public RestTemplate restTemplate() {
        return new RestTemplate();
    }
}
```

#### ASP.NET Core
```csharp
// Interfaces
public interface IProductoService
{
    Task<List<Producto>> GetAllAsync();
}

// Implementación
public class ProductoService : IProductoService
{
    private readonly IProductoRepository _repository;
    
    // Constructor injection (recomendado)
    public ProductoService(IProductoRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<List<Producto>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }
}

// Repositorio
public interface IProductoRepository
{
    Task<List<Producto>> GetAllAsync();
}

public class ProductoRepository : IProductoRepository
{
    private readonly ApplicationDbContext _context;
    
    public ProductoRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<List<Producto>> GetAllAsync()
    {
        return await _context.Productos.ToListAsync();
    }
}

// Registro en Program.cs
builder.Services.AddScoped<IProductoService, ProductoService>();
builder.Services.AddScoped<IProductoRepository, ProductoRepository>();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// HttpClient (equivalente a RestTemplate)
builder.Services.AddHttpClient();
```

### Scopes de Servicios

| Spring Boot | ASP.NET Core | Descripción |
|------------|--------------|-------------|
| `@Scope("singleton")` | `AddSingleton<T>()` | Una instancia para toda la app |
| `@Scope("prototype")` | `AddTransient<T>()` | Nueva instancia cada vez |
| `@Scope("request")` | `AddScoped<T>()` | Una instancia por request HTTP |

## Controladores y Routing

### REST Controllers

#### Spring Boot
```java
@RestController
@RequestMapping("/api/productos")
public class ProductoController {
    
    private final ProductoService service;
    
    @Autowired
    public ProductoController(ProductoService service) {
        this.service = service;
    }
    
    // GET /api/productos
    @GetMapping
    public ResponseEntity<List<Producto>> getAll() {
        return ResponseEntity.ok(service.findAll());
    }
    
    // GET /api/productos/{id}
    @GetMapping("/{id}")
    public ResponseEntity<Producto> getById(@PathVariable Long id) {
        return service.findById(id)
            .map(ResponseEntity::ok)
            .orElse(ResponseEntity.notFound().build());
    }
    
    // POST /api/productos
    @PostMapping
    public ResponseEntity<Producto> create(@Valid @RequestBody ProductoDTO dto) {
        Producto producto = service.create(dto);
        return ResponseEntity.status(HttpStatus.CREATED).body(producto);
    }
    
    // PUT /api/productos/{id}
    @PutMapping("/{id}")
    public ResponseEntity<Producto> update(
        @PathVariable Long id,
        @Valid @RequestBody ProductoDTO dto) {
        return ResponseEntity.ok(service.update(id, dto));
    }
    
    // DELETE /api/productos/{id}
    @DeleteMapping("/{id}")
    public ResponseEntity<Void> delete(@PathVariable Long id) {
        service.delete(id);
        return ResponseEntity.noContent().build();
    }
    
    // Query parameters: GET /api/productos/search?nombre=laptop&precio=1000
    @GetMapping("/search")
    public ResponseEntity<List<Producto>> search(
        @RequestParam String nombre,
        @RequestParam Double precio) {
        return ResponseEntity.ok(service.search(nombre, precio));
    }
}
```

#### ASP.NET Core
```csharp
[ApiController]
[Route("api/[controller]")]
public class ProductosController : ControllerBase
{
    private readonly IProductoService _service;
    
    public ProductosController(IProductoService service)
    {
        _service = service;
    }
    
    // GET api/productos
    [HttpGet]
    public async Task<ActionResult<List<Producto>>> GetAll()
    {
        var productos = await _service.GetAllAsync();
        return Ok(productos);
    }
    
    // GET api/productos/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Producto>> GetById(long id)
    {
        var producto = await _service.GetByIdAsync(id);
        if (producto == null)
            return NotFound();
        
        return Ok(producto);
    }
    
    // POST api/productos
    [HttpPost]
    public async Task<ActionResult<Producto>> Create([FromBody] ProductoDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        var producto = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = producto.Id }, producto);
    }
    
    // PUT api/productos/5
    [HttpPut("{id}")]
    public async Task<ActionResult<Producto>> Update(long id, [FromBody] ProductoDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        var producto = await _service.UpdateAsync(id, dto);
        return Ok(producto);
    }
    
    // DELETE api/productos/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
    
    // Query parameters: GET api/productos/search?nombre=laptop&precio=1000
    [HttpGet("search")]
    public async Task<ActionResult<List<Producto>>> Search(
        [FromQuery] string nombre,
        [FromQuery] double precio)
    {
        var productos = await _service.SearchAsync(nombre, precio);
        return Ok(productos);
    }
}
```

### Tabla de Anotaciones/Atributos

| Spring Boot | ASP.NET Core | Función |
|------------|--------------|---------|
| `@RestController` | `[ApiController]` | Marca clase como REST controller |
| `@Controller` | `[Controller]` | Marca clase como MVC controller |
| `@RequestMapping("/path")` | `[Route("path")]` | Ruta base del controller |
| `@GetMapping` | `[HttpGet]` | Método HTTP GET |
| `@PostMapping` | `[HttpPost]` | Método HTTP POST |
| `@PutMapping` | `[HttpPut]` | Método HTTP PUT |
| `@DeleteMapping` | `[HttpDelete]` | Método HTTP DELETE |
| `@PathVariable` | `{id}` en ruta | Variable de ruta |
| `@RequestParam` | `[FromQuery]` | Parámetro de query string |
| `@RequestBody` | `[FromBody]` | Cuerpo de la petición |
| `@RequestHeader` | `[FromHeader]` | Header HTTP |
| `@Valid` | `[FromBody]` + ModelState | Validación automática |

## Middleware vs Filters

### Spring Boot - Filters e Interceptors

#### Filter (nivel servlet)
```java
@Component
public class LoggingFilter implements Filter {
    @Override
    public void doFilter(ServletRequest request, ServletResponse response, FilterChain chain)
            throws IOException, ServletException {
        HttpServletRequest req = (HttpServletRequest) request;
        System.out.println("Request: " + req.getMethod() + " " + req.getRequestURI());
        
        chain.doFilter(request, response);
        
        System.out.println("Response completed");
    }
}
```

#### Interceptor (nivel Spring MVC)
```java
@Component
public class LoggingInterceptor implements HandlerInterceptor {
    @Override
    public boolean preHandle(HttpServletRequest request, HttpServletResponse response, Object handler) {
        System.out.println("Before handling request");
        return true;
    }
    
    @Override
    public void postHandle(HttpServletRequest request, HttpServletResponse response, Object handler, ModelAndView modelAndView) {
        System.out.println("After handling request");
    }
}

@Configuration
public class WebConfig implements WebMvcConfigurer {
    @Autowired
    private LoggingInterceptor loggingInterceptor;
    
    @Override
    public void addInterceptors(InterceptorRegistry registry) {
        registry.addInterceptor(loggingInterceptor);
    }
}
```

### ASP.NET Core - Middleware

```csharp
// Middleware personalizado
public class LoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<LoggingMiddleware> _logger;
    
    public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        _logger.LogInformation($"Request: {context.Request.Method} {context.Request.Path}");
        
        await _next(context);
        
        _logger.LogInformation($"Response: {context.Response.StatusCode}");
    }
}

// Extension method para registrar
public static class LoggingMiddlewareExtensions
{
    public static IApplicationBuilder UseLoggingMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<LoggingMiddleware>();
    }
}

// Registro en Program.cs
app.UseLoggingMiddleware();

// O inline
app.Use(async (context, next) =>
{
    Console.WriteLine($"Request: {context.Request.Path}");
    await next();
    Console.WriteLine($"Response: {context.Response.StatusCode}");
});
```

## Validación

### Spring Boot Validation

```java
// DTO con anotaciones de validación
public class ProductoDTO {
    @NotNull(message = "El nombre no puede ser nulo")
    @Size(min = 3, max = 100, message = "El nombre debe tener entre 3 y 100 caracteres")
    private String nombre;
    
    @NotNull(message = "El precio no puede ser nulo")
    @DecimalMin(value = "0.0", inclusive = false, message = "El precio debe ser mayor que 0")
    private Double precio;
    
    @Email(message = "Email inválido")
    private String email;
    
    // getters y setters
}

// Controller
@PostMapping
public ResponseEntity<?> create(@Valid @RequestBody ProductoDTO dto, BindingResult result) {
    if (result.hasErrors()) {
        Map<String, String> errors = result.getFieldErrors().stream()
            .collect(Collectors.toMap(
                FieldError::getField,
                FieldError::getDefaultMessage
            ));
        return ResponseEntity.badRequest().body(errors);
    }
    
    return ResponseEntity.ok(service.create(dto));
}
```

### ASP.NET Core Validation (Data Annotations)

```csharp
// DTO con atributos de validación
public class ProductoDTO
{
    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 100 caracteres")]
    public string Nombre { get; set; }
    
    [Required(ErrorMessage = "El precio es obligatorio")]
    [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor que 0")]
    public double Precio { get; set; }
    
    [EmailAddress(ErrorMessage = "Email inválido")]
    public string? Email { get; set; }
}

// Controller con validación automática gracias a [ApiController]
[HttpPost]
public async Task<ActionResult<Producto>> Create([FromBody] ProductoDTO dto)
{
    // [ApiController] valida automáticamente y retorna 400 si hay errores
    // No necesitas verificar ModelState manualmente
    
    var producto = await _service.CreateAsync(dto);
    return CreatedAtAction(nameof(GetById), new { id = producto.Id }, producto);
}

// Si quieres manejo manual:
[HttpPost]
public async Task<ActionResult<Producto>> CreateManual([FromBody] ProductoDTO dto)
{
    if (!ModelState.IsValid)
    {
        var errors = ModelState
            .Where(x => x.Value.Errors.Count > 0)
            .ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
            );
        return BadRequest(errors);
    }
    
    var producto = await _service.CreateAsync(dto);
    return Ok(producto);
}
```

### ASP.NET Core - FluentValidation (Recomendado)

```csharp
// Install: FluentValidation.AspNetCore

public class ProductoDTOValidator : AbstractValidator<ProductoDTO>
{
    public ProductoDTOValidator()
    {
        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre es obligatorio")
            .Length(3, 100).WithMessage("El nombre debe tener entre 3 y 100 caracteres");
        
        RuleFor(x => x.Precio)
            .GreaterThan(0).WithMessage("El precio debe ser mayor que 0");
        
        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Email inválido")
            .When(x => !string.IsNullOrEmpty(x.Email));
    }
}

// Registro en Program.cs
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<ProductoDTOValidator>();
```

## Exception Handling

### Spring Boot

```java
// Exception personalizada
public class ProductoNotFoundException extends RuntimeException {
    public ProductoNotFoundException(Long id) {
        super("Producto no encontrado con ID: " + id);
    }
}

// Global exception handler
@RestControllerAdvice
public class GlobalExceptionHandler {
    
    @ExceptionHandler(ProductoNotFoundException.class)
    public ResponseEntity<ErrorResponse> handleProductoNotFound(ProductoNotFoundException ex) {
        ErrorResponse error = new ErrorResponse(
            HttpStatus.NOT_FOUND.value(),
            ex.getMessage(),
            LocalDateTime.now()
        );
        return ResponseEntity.status(HttpStatus.NOT_FOUND).body(error);
    }
    
    @ExceptionHandler(Exception.class)
    public ResponseEntity<ErrorResponse> handleGenericException(Exception ex) {
        ErrorResponse error = new ErrorResponse(
            HttpStatus.INTERNAL_SERVER_ERROR.value(),
            "Error interno del servidor",
            LocalDateTime.now()
        );
        return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR).body(error);
    }
}
```

### ASP.NET Core

```csharp
// Exception personalizada
public class ProductoNotFoundException : Exception
{
    public ProductoNotFoundException(long id)
        : base($"Producto no encontrado con ID: {id}")
    {
    }
}

// Middleware de excepción global
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    
    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ProductoNotFoundException ex)
        {
            _logger.LogWarning(ex, "Producto no encontrado");
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            await context.Response.WriteAsJsonAsync(new
            {
                statusCode = 404,
                message = ex.Message,
                timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error no controlado");
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(new
            {
                statusCode = 500,
                message = "Error interno del servidor",
                timestamp = DateTime.UtcNow
            });
        }
    }
}

// Registro
app.UseMiddleware<ExceptionHandlingMiddleware>();

// O usar el middleware built-in
app.UseExceptionHandler("/error");
```

## Recursos Adicionales

### Documentación Oficial
- [ASP.NET Core Documentation](https://docs.microsoft.com/en-us/aspnet/core/)
- [Spring Boot Reference](https://docs.spring.io/spring-boot/docs/current/reference/html/)

### Siguientes Pasos
1. **[Data Access](../03-data-access/)** - JPA → Entity Framework Core
2. **[Security](../04-security/)** - Spring Security → ASP.NET Core Identity
3. **[Testing](../05-testing/)** - JUnit → NUnit/xUnit

---

## Autor

Codificado con ❤️ por **José Luis González Sánchez**

## Licencia

Creative Commons BY-NC-SA 4.0
