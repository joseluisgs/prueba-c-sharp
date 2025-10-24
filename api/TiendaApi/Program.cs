using Microsoft.EntityFrameworkCore;
using TiendaApi.Data;
using TiendaApi.Middleware;
using TiendaApi.Repositories;
using TiendaApi.Services;

var builder = WebApplication.CreateBuilder(args);

// ============================================================================
// CONFIGURATION - Similar to Spring Boot's application.properties
// ============================================================================

// Add Controllers (MVC pattern)
// Java Spring Boot: @RestController classes automatically scanned
builder.Services.AddControllers();

// Add Swagger/OpenAPI documentation
// Java Spring Boot: SpringDoc OpenAPI (springdoc-openapi-ui)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() 
    { 
        Title = "TiendaApi - Dual Error Handling Demo",
        Version = "v1",
        Description = @"API REST educativa demostrando DOS enfoques de manejo de errores:
        
**Categorías**: Enfoque Tradicional con Exceptions (familiar para Java devs)
**Productos**: Result Pattern Moderno (functional programming)

Compara ambos enfoques para aprender cuándo usar cada uno."
    });
});

// ============================================================================
// DATABASE CONFIGURATION
// ============================================================================

// PostgreSQL with Entity Framework Core
// Java Spring Boot: spring.datasource.url + JPA configuration
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Host=localhost;Database=tienda;Username=admin;Password=admin123";

builder.Services.AddDbContext<TiendaDbContext>(options =>
    options.UseNpgsql(connectionString));

// ============================================================================
// DEPENDENCY INJECTION - Similar to Spring's @Autowired
// ============================================================================

// Repositories
// Java Spring Boot: @Repository classes automatically registered
builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();
builder.Services.AddScoped<IProductoRepository, ProductoRepository>();

// Services
// Java Spring Boot: @Service classes automatically registered
builder.Services.AddScoped<CategoriaService>();
builder.Services.AddScoped<ProductoService>();

// AutoMapper
// Java Spring Boot: ModelMapper bean configuration
builder.Services.AddAutoMapper(typeof(MappingProfile));

// ============================================================================
// CORS Configuration (for frontend apps)
// Java Spring Boot: @CrossOrigin or WebMvcConfigurer
// ============================================================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// ============================================================================
// BUILD APPLICATION
// ============================================================================
var app = builder.Build();

// ============================================================================
// MIDDLEWARE PIPELINE - Similar to Spring Security filter chain
// Java Spring Boot: Filter chain and @ControllerAdvice
// ============================================================================

// Development-only: Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "TiendaApi v1");
        options.RoutePrefix = string.Empty; // Swagger at root URL
    });
}

// Global Exception Handler Middleware
// Java Spring Boot: @ControllerAdvice with @ExceptionHandler
// ONLY handles exceptions from Categorías (traditional approach)
app.UseGlobalExceptionHandler();

app.UseHttpsRedirection();

// CORS
app.UseCors("AllowAll");

// Map Controllers
// Java Spring Boot: @RestController classes automatically mapped
app.MapControllers();

// ============================================================================
// DATABASE INITIALIZATION
// ============================================================================

// Apply migrations and seed data on startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<TiendaDbContext>();
        
        // Create database if it doesn't exist
        context.Database.EnsureCreated();
        
        // Or apply pending migrations (use this for production)
        // context.Database.Migrate();
        
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogInformation("Database initialized successfully");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while initializing the database");
    }
}

// ============================================================================
// RUN APPLICATION
// ============================================================================

app.Logger.LogInformation("""
    
    ════════════════════════════════════════════════════════════════════════════
    🏬 TiendaApi - Educational Dual Error Handling Demo
    ════════════════════════════════════════════════════════════════════════════
    
    📚 EDUCATIONAL ENDPOINTS:
    
    🔴 TRADITIONAL EXCEPTIONS (like Java/Spring Boot):
       GET    /api/categorias          - List all categories
       GET    /api/categorias/{id}     - Get category by ID
       POST   /api/categorias          - Create category
       PUT    /api/categorias/{id}     - Update category
       DELETE /api/categorias/{id}     - Delete category
       
       → Uses try/catch, throws exceptions
       → GlobalExceptionHandler catches exceptions
       → Familiar to Java/Spring Boot developers
    
    🟢 MODERN RESULT PATTERN (functional programming):
       GET    /api/productos           - List all products
       GET    /api/productos/{id}      - Get product by ID
       POST   /api/productos           - Create product
       PUT    /api/productos/{id}      - Update product
       DELETE /api/productos/{id}      - Delete product
       
       → Returns Result<T, AppError>
       → NO try/catch blocks
       → Pattern matching for error handling
       → Explicit, type-safe, better performance
    
    📖 Swagger Documentation: http://localhost:5000
    
    Compare both approaches to learn when to use each pattern! 🚀
    ════════════════════════════════════════════════════════════════════════════
    """);

app.Run();
