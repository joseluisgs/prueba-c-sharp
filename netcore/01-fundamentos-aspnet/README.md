# 🏗️ Fundamentos ASP.NET Core

## Introducción

Esta sección compara los conceptos fundamentales de **Spring Boot** con **ASP.NET Core**, proporcionando una guía completa para desarrolladores Java que migran a .NET.

## 📚 Contenido de Esta Sección

### 1. [project-structure.md](./project-structure.md)
- Estructura de proyectos: Maven/Gradle vs .csproj
- Convenciones de directorios
- Archivos de configuración
- Build system comparativo

### 2. [dependency-injection.md](./dependency-injection.md)
- Spring IoC Container vs .NET DI Container
- @Autowired vs Constructor Injection
- @Component, @Service, @Repository vs AddScoped/AddSingleton
- Lifetimes: Singleton, Scoped, Transient

### 3. [configuration.md](./configuration.md)
- application.properties/yml vs appsettings.json
- Profile-specific configuration
- Environment variables
- Configuration binding

## 🎯 Objetivos de Aprendizaje

Al completar esta sección, entenderás:

1. ✅ Diferencias arquitecturales entre Spring Boot y ASP.NET Core
2. ✅ Cómo estructurar proyectos .NET
3. ✅ Sistema de Dependency Injection en .NET
4. ✅ Configuración y profiles en ASP.NET Core
5. ✅ Migración de conceptos Spring a .NET

## 🔄 Comparativa Rápida

| Concepto | Spring Boot | ASP.NET Core |
|----------|-------------|--------------|
| **Framework** | Spring Framework | ASP.NET Core |
| **Build Tool** | Maven/Gradle | dotnet CLI/.csproj |
| **Entry Point** | @SpringBootApplication | Program.cs |
| **DI Container** | Spring IoC | Built-in DI |
| **Config Files** | application.yml | appsettings.json |
| **Profiles** | spring.profiles.active | ASPNETCORE_ENVIRONMENT |

## 🚀 Inicio Rápido

### Spring Boot Project

```
my-spring-app/
├── src/
│   ├── main/
│   │   ├── java/
│   │   │   └── com/example/app/
│   │   │       ├── Application.java
│   │   │       ├── controller/
│   │   │       ├── service/
│   │   │       └── repository/
│   │   └── resources/
│   │       ├── application.yml
│   │       └── application-dev.yml
│   └── test/
└── pom.xml
```

### ASP.NET Core Project

```
MyDotnetApp/
├── Controllers/
├── Services/
├── Repositories/
├── Models/
├── Program.cs
├── appsettings.json
├── appsettings.Development.json
└── MyDotnetApp.csproj
```

## 🏁 Spring Boot Application

```java
@SpringBootApplication
public class Application {
    public static void main(String[] args) {
        SpringApplication.run(Application.class, args);
    }
}

@RestController
@RequestMapping("/api")
public class HelloController {
    
    @Autowired
    private HelloService service;
    
    @GetMapping("/hello")
    public ResponseEntity<String> hello() {
        return ResponseEntity.ok(service.getMessage());
    }
}

@Service
public class HelloService {
    
    @Value("${app.message}")
    private String message;
    
    public String getMessage() {
        return message;
    }
}
```

## 🏁 ASP.NET Core Application

```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddScoped<HelloService>();

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

// Controllers/HelloController.cs
[ApiController]
[Route("api/[controller]")]
public class HelloController : ControllerBase
{
    private readonly HelloService _service;
    
    public HelloController(HelloService service)
    {
        _service = service;
    }
    
    [HttpGet("hello")]
    public IActionResult Hello()
    {
        return Ok(_service.GetMessage());
    }
}

// Services/HelloService.cs
public class HelloService
{
    private readonly IConfiguration _configuration;
    
    public HelloService(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public string GetMessage()
    {
        return _configuration["App:Message"] ?? "Hello World";
    }
}
```

## 📊 Conceptos Clave

### Minimal API vs Traditional Controllers

**ASP.NET Core ofrece dos enfoques:**

#### Minimal API (similar a Express.js)
```csharp
var app = WebApplication.CreateBuilder(args).Build();

app.MapGet("/hello", () => "Hello World!");
app.MapPost("/users", (User user) => Results.Created($"/users/{user.Id}", user));

app.Run();
```

#### Traditional Controllers (similar a Spring MVC)
```csharp
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    [HttpGet]
    public IActionResult GetAll() => Ok(users);
    
    [HttpPost]
    public IActionResult Create([FromBody] User user) 
        => Created($"/users/{user.Id}", user);
}
```

**Recomendación para migración Java:**
- Usa **Traditional Controllers** - más similar a Spring MVC
- Minimal API es bueno para microservicios pequeños

## 🔗 Referencias

- [ASP.NET Core Overview](https://docs.microsoft.com/en-us/aspnet/core/introduction-to-aspnet-core)
- [Dependency Injection in ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection)
- [Configuration in ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/)

---

**Siguiente:** [Project Structure →](./project-structure.md)
