using System.Text;
using System.Threading.Channels;
using GraphQL;
using GraphQL.Types;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TiendaApi.Data;
using TiendaApi.GraphQL;
using TiendaApi.GraphQL.Types;
using TiendaApi.Middleware;
using TiendaApi.Repositories;
using TiendaApi.Services;
using TiendaApi.Services.Auth;
using TiendaApi.Services.Cache;
using TiendaApi.Services.Email;
using TiendaApi.Services.Pedidos;
using TiendaApi.WebSockets;

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
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPedidosRepository, PedidosRepository>();

// Services
// Java Spring Boot: @Service classes automatically registered
builder.Services.AddScoped<CategoriaService>();
builder.Services.AddScoped<ProductoService>();
builder.Services.AddScoped<IPedidosService, PedidosService>();

// JWT Service
builder.Services.AddScoped<IJwtService, JwtService>();

// Cache Service (Redis)
builder.Services.AddStackExchangeRedisCache(options =>
{
    var redisConnection = builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379";
    options.Configuration = redisConnection;
    options.InstanceName = "TiendaApi:";
});
builder.Services.AddScoped<ICacheService, RedisCacheService>();

// Email Service with Background Worker
builder.Services.AddSingleton(Channel.CreateUnbounded<EmailMessage>());
builder.Services.AddScoped<IEmailService, MailKitEmailService>();
builder.Services.AddHostedService<EmailBackgroundService>();

// WebSocket Handler
builder.Services.AddSingleton<ProductoWebSocketHandler>();
builder.Services.AddSingleton<PedidoWebSocketHandler>();

// GraphQL Services
builder.Services.AddScoped<IDocumentExecuter, DocumentExecuter>();
builder.Services.AddScoped<ISchema, TiendaSchema>();
builder.Services.AddScoped<TiendaQuery>();
builder.Services.AddScoped<ProductoType>();
builder.Services.AddScoped<CategoriaType>();

// AutoMapper
// Java Spring Boot: ModelMapper bean configuration
builder.Services.AddAutoMapper(typeof(MappingProfile), typeof(TiendaApi.Mappings.PedidoProfile));

// ============================================================================
// AUTHENTICATION & AUTHORIZATION - Similar to Spring Security
// ============================================================================

// JWT Authentication
// Java Spring Boot: Spring Security with JWT filter
var jwtKey = builder.Configuration["Jwt:Key"] 
    ?? throw new InvalidOperationException("JWT Key not configured");
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "TiendaApi";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "TiendaApi";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ValidateIssuer = true,
        ValidIssuer = jwtIssuer,
        ValidateAudience = true,
        ValidAudience = jwtAudience,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

// Authorization policies
// Java Spring Boot: @PreAuthorize("hasRole('ADMIN')")
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("RequireAdminRole", policy => policy.RequireRole("ADMIN"))
    .AddPolicy("RequireUserRole", policy => policy.RequireRole("USER", "ADMIN"));

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

// GraphiQL UI for GraphQL queries
// Accessible at /graphiql
app.MapGet("/graphiql", async context =>
{
    context.Response.ContentType = "text/html";
    await context.Response.WriteAsync(@"
<!DOCTYPE html>
<html>
<head>
    <title>GraphiQL</title>
    <link href=""https://unpkg.com/graphiql/graphiql.min.css"" rel=""stylesheet"" />
</head>
<body style=""margin: 0;"">
    <div id=""graphiql"" style=""height: 100vh;""></div>
    <script crossorigin src=""https://unpkg.com/react/umd/react.production.min.js""></script>
    <script crossorigin src=""https://unpkg.com/react-dom/umd/react-dom.production.min.js""></script>
    <script crossorigin src=""https://unpkg.com/graphiql/graphiql.min.js""></script>
    <script>
        const fetcher = GraphiQL.createFetcher({ url: '/graphql' });
        ReactDOM.render(
            React.createElement(GraphiQL, { fetcher: fetcher }),
            document.getElementById('graphiql')
        );
    </script>
</body>
</html>
");
});

// Global Exception Handler Middleware
// Java Spring Boot: @ControllerAdvice with @ExceptionHandler
// ONLY handles exceptions from Categorías (traditional approach)
app.UseGlobalExceptionHandler();

app.UseHttpsRedirection();

// CORS
app.UseCors("AllowAll");

// Authentication & Authorization
// Java Spring Boot: Spring Security filter chain
app.UseAuthentication();
app.UseAuthorization();

// WebSocket support
// Java Spring Boot: @ServerEndpoint or WebSocketHandler
app.UseWebSockets();

// WebSocket endpoint for producto notifications
app.Map("/ws/v1/productos", async context =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        var webSocket = await context.WebSockets.AcceptWebSocketAsync();
        var handler = context.RequestServices.GetRequiredService<ProductoWebSocketHandler>();
        await handler.HandleConnectionAsync(context, webSocket);
    }
    else
    {
        context.Response.StatusCode = 400;
    }
});

// WebSocket endpoint for pedido notifications
app.Map("/ws/v1/pedidos", async context =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        var webSocket = await context.WebSockets.AcceptWebSocketAsync();
        var handler = context.RequestServices.GetRequiredService<PedidoWebSocketHandler>();
        await handler.HandleConnectionAsync(context, webSocket);
    }
    else
    {
        context.Response.StatusCode = 400;
    }
});

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
    🏬 TiendaApi - Educational Dual Error Handling Demo + Runtime Features
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
       GET    /api/productos           - List all products (with Redis cache)
       GET    /api/productos/{id}      - Get product by ID (with cache-aside)
       POST   /api/productos           - Create product (WebSocket + Email notification)
       PUT    /api/productos/{id}      - Update product (WebSocket notification)
       DELETE /api/productos/{id}      - Delete product (WebSocket notification)
       
       → Returns Result<T, AppError>
       → NO try/catch blocks
       → Pattern matching for error handling
       → Explicit, type-safe, better performance
       → Integrated with Redis cache, WebSocket, Email
    
    🔐 AUTHENTICATION (JWT):
       POST   /v1/auth/signup          - Register new user
       POST   /v1/auth/signin          - Login and get JWT token
       
       → BCrypt password hashing
       → JWT token generation
       → Use token in Authorization: Bearer <token> header
    
    🔌 WEBSOCKET (Real-time notifications):
       WS     /ws/v1/productos         - WebSocket endpoint for producto notifications
       
       → Connect with WebSocket client
       → Receive real-time notifications on producto CREATED/UPDATED/DELETED
    
    🔍 GRAPHQL:
       POST   /graphql                 - GraphQL query endpoint
       GET    /graphiql                - GraphiQL interactive UI
       
       → Query productos and categorias
       → Example: { productos { id nombre precio } }
    
    📖 Swagger Documentation: http://localhost:5000
    
    Compare both error handling approaches and explore the runtime features! 🚀
    ════════════════════════════════════════════════════════════════════════════
    """);

app.Run();
