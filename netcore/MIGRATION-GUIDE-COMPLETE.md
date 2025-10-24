# 🚀 Guía Maestra de Migración Java/Spring Boot → C#/.NET

## 📚 Programa Completo de 12 Semanas para Desarrolladores Java

Esta guía te llevará desde principiante en C#/.NET hasta desarrollador competente, con un enfoque estructurado y progresivo basado en tu experiencia Java/Spring Boot.

---

## 🎯 Objetivos del Programa

Al completar este programa de 12 semanas, serás capaz de:

1. ✅ Escribir código C# idiomático y moderno
2. ✅ Desarrollar APIs REST completas con ASP.NET Core
3. ✅ Implementar patrones avanzados de manejo de errores
4. ✅ Trabajar con múltiples bases de datos (SQL y NoSQL)
5. ✅ Implementar autenticación y autorización
6. ✅ Escribir tests comprehensivos
7. ✅ Desplegar aplicaciones containerizadas

---

## 📅 Cronograma Completo

| Semana | Tema | Checkpoint |
|--------|------|-----------|
| 1-2 | Fundamentos C# | ✅ Proyecto básico ASP.NET Core |
| 3-4 | Web API Básica | ✅ API REST funcionando |
| 5-6 | **Manejo de Errores** ⭐ | ✅ Exception + Result Pattern |
| 7-8 | Data Access | ✅ Base de datos híbrida |
| 9-10 | Seguridad | ✅ Auth & Authorization |
| 11-12 | Temas Avanzados | ✅ Aplicación completa |

---

## 🗓️ SEMANA 1-2: Fundamentos C#

### Objetivos
Aprender la sintaxis de C# y conceptos fundamentales, comparándolos con Java.

### 📚 Contenido Teórico

#### Día 1-2: Sintaxis Básica
**Java vs C# - Conceptos Fundamentales**

| Concepto | Java | C# |
|----------|------|-----|
| **Package** | `package com.example;` | `namespace Example;` |
| **Import** | `import java.util.*;` | `using System.Collections.Generic;` |
| **Class** | `public class User { }` | `public class User { }` |
| **Main** | `public static void main(String[] args)` | `static void Main(string[] args)` |

**Ejercicio 1:** Crear programa "Hello World"
```csharp
// Program.cs
Console.WriteLine("Hello from C#!");

// Equivalente Java:
// System.out.println("Hello from Java!");
```

#### Día 3-4: Tipos de Datos y Null Safety

**Java:**
```java
String name = null;  // Puede ser null
Integer age = 25;
```

**C#:**
```csharp
string? name = null;  // Nullable reference type
int age = 25;         // Value type, nunca null
int? optionalAge = null;  // Nullable value type
```

**Ejercicio 2:** Programa que maneje valores null de forma segura

#### Día 5-6: Properties vs Getters/Setters

**Java:**
```java
public class User {
    private String name;
    
    public String getName() {
        return name;
    }
    
    public void setName(String name) {
        this.name = name;
    }
}
```

**C#:**
```csharp
public class User
{
    public string Name { get; set; }  // Auto-property
    
    // O con validación:
    private string _name = string.Empty;
    public string Name 
    { 
        get => _name;
        set => _name = string.IsNullOrEmpty(value) 
            ? throw new ArgumentException("Name required")
            : value;
    }
}
```

**Ejercicio 3:** Crear clase User con properties

#### Día 7-8: LINQ Basics vs Stream API

**Java Stream API:**
```java
List<Integer> numbers = List.of(1, 2, 3, 4, 5);
List<Integer> evens = numbers.stream()
    .filter(n -> n % 2 == 0)
    .collect(Collectors.toList());
```

**C# LINQ:**
```csharp
List<int> numbers = new() { 1, 2, 3, 4, 5 };
List<int> evens = numbers
    .Where(n => n % 2 == 0)
    .ToList();

// O con query syntax:
var evens = from n in numbers
            where n % 2 == 0
            select n;
```

**Ejercicio 4:** Procesar una lista de objetos con LINQ

#### Día 9-10: Dependency Injection Container

**Java Spring:**
```java
@Configuration
public class AppConfig {
    @Bean
    public UserService userService() {
        return new UserService();
    }
}
```

**C# ASP.NET Core:**
```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
```

**Ejercicio 5:** Configurar DI básico

### ✅ CHECKPOINT 1: Crear Proyecto Básico ASP.NET Core

**Proyecto:** API REST básica con un endpoint
```bash
dotnet new webapi -n MiPrimeraApi
cd MiPrimeraApi
dotnet run
```

**Requisitos:**
- [x] Proyecto creado con dotnet CLI
- [x] Endpoint GET /api/hello que retorna "Hello World"
- [x] Usar dependency injection para un servicio
- [x] Tests básicos con NUnit

**Evaluación:**
- ¿El proyecto compila sin errores?
- ¿El endpoint responde correctamente?
- ¿El DI está configurado correctamente?

---

## 🗓️ SEMANA 3-4: Web API Básica

### Objetivos
Crear APIs REST completas con controllers, routing, model binding y validation.

### 📚 Contenido Teórico

#### Día 1-3: Controllers y Routing

**Java Spring MVC:**
```java
@RestController
@RequestMapping("/api/users")
public class UserController {
    
    @GetMapping("/{id}")
    public ResponseEntity<User> getById(@PathVariable Long id) {
        return ResponseEntity.ok(user);
    }
    
    @PostMapping
    public ResponseEntity<User> create(@RequestBody User user) {
        return ResponseEntity.status(HttpStatus.CREATED).body(user);
    }
}
```

**C# ASP.NET Core:**
```csharp
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    [HttpGet("{id}")]
    public IActionResult GetById(long id)
    {
        return Ok(user);
    }
    
    [HttpPost]
    public IActionResult Create([FromBody] User user)
    {
        return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
    }
}
```

**Ejercicio 6:** Crear CRUD completo para entidad User

#### Día 4-6: Model Binding y Validation

**Java Bean Validation:**
```java
public class CreateUserDto {
    @NotNull(message = "Email is required")
    @Email
    private String email;
    
    @Size(min = 6, message = "Password must be at least 6 characters")
    private String password;
}
```

**C# FluentValidation:**
```csharp
public class CreateUserDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class CreateUserDtoValidator : AbstractValidator<CreateUserDto>
{
    public CreateUserDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress();
        
        RuleFor(x => x.Password)
            .MinimumLength(6).WithMessage("Password must be at least 6 characters");
    }
}
```

**Ejercicio 7:** Implementar validación con FluentValidation

#### Día 7-10: Action Results

**Comparativa de Returns:**

| Escenario | Java | C# |
|-----------|------|-----|
| Success (200) | `ResponseEntity.ok(data)` | `Ok(data)` |
| Created (201) | `ResponseEntity.status(CREATED).body(data)` | `CreatedAtAction(...)` |
| No Content (204) | `ResponseEntity.noContent().build()` | `NoContent()` |
| Bad Request (400) | `ResponseEntity.badRequest().body(error)` | `BadRequest(error)` |
| Not Found (404) | `ResponseEntity.notFound().build()` | `NotFound()` |

**Ejercicio 8:** Implementar todos los tipos de responses

### ✅ CHECKPOINT 2: API REST Básica Funcionando

**Proyecto:** API de Gestión de Tareas (Todo List)
```
TodoApi/
├── Controllers/
│   └── TodosController.cs
├── Models/
│   ├── Todo.cs
│   └── CreateTodoDto.cs
├── Services/
│   └── TodoService.cs
└── Program.cs
```

**Requisitos:**
- [x] CRUD completo para Todos (GET, POST, PUT, DELETE)
- [x] Validación con FluentValidation
- [x] Manejo de errores básico
- [x] Swagger documentation
- [x] Tests de controllers

**Evaluación:**
- ¿Todos los endpoints funcionan correctamente?
- ¿La validación rechaza datos inválidos?
- ¿Swagger muestra la documentación?

---

## 🗓️ SEMANA 5-6: Manejo de Errores ⭐ **TEMA CENTRAL**

### Objetivos
Dominar **AMBOS** enfoques de manejo de errores: Exceptions y Result Pattern.

### 📚 Contenido Teórico

#### Día 1-3: Traditional Exception Handling

**Implementar GlobalExceptionHandler**

Ver documentación completa en:
- [traditional-exceptions.md](./03-error-handling-patterns/traditional-exceptions.md)

**Ejercicio 9:** Implementar exception handling en TodoApi
- Custom exceptions (NotFoundException, ValidationException)
- GlobalExceptionHandler middleware
- Try/catch en controllers

#### Día 4-7: Result Pattern con Railway Oriented Programming

**Implementar Result Pattern**

Ver documentación completa en:
- [result-pattern.md](./03-error-handling-patterns/result-pattern.md)

**Ejercicio 10:** Migrar TodoApi a Result Pattern
- Crear Result<T, E> type
- Implementar AppError
- Usar Bind/Map/Tap operations
- Controllers con pattern matching

#### Día 8-10: Comparación y Decisión

**Analizar ambos enfoques:**

Ver guía completa en:
- [when-to-use.md](./03-error-handling-patterns/when-to-use.md)

**Ejercicio 11:** Benchmark de performance
```csharp
[Benchmark]
public async Task FindById_Exception()
{
    return await _serviceWithExceptions.FindByIdAsync(1);
}

[Benchmark]
public async Task FindById_Result()
{
    return await _serviceWithResult.FindByIdAsync(1);
}
```

### ✅ CHECKPOINT 3: Implementar Ambos Patrones

**Proyecto:** Ampliar TodoApi con AMBOS enfoques

**Estructura:**
```
TodoApi/
├── Controllers/
│   ├── TodosController.cs          # Exception-based
│   └── TasksController.cs          # Result Pattern-based
├── Services/
│   ├── TodoService.cs              # Throws exceptions
│   └── TaskService.cs              # Returns Result<T, E>
└── Common/
    ├── Result.cs
    └── AppError.cs
```

**Requisitos:**
- [x] TodosController usa exception handling
- [x] TasksController usa Result Pattern
- [x] Documentar diferencias en README
- [x] Performance benchmarks
- [x] Tests para ambos enfoques

**Evaluación:**
- ¿Entiendes cuándo usar cada patrón?
- ¿Puedes explicar pros y contras de cada uno?
- ¿Los benchmarks muestran diferencias de performance?

---

## 🗓️ SEMANA 7-8: Data Access

### Objetivos
Dominar acceso a datos con Entity Framework Core, ADO.NET y MongoDB.

### 📚 Contenido Teórico

#### Día 1-4: Entity Framework Core vs JPA/Hibernate

**Comparativa completa en:**
- [04-data-access-complete/ef-core-vs-jpa.md](./04-data-access-complete/ef-core-vs-jpa.md)

**Entity Configuration:**

**Java JPA:**
```java
@Entity
@Table(name = "productos")
public class Producto {
    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;
    
    @Column(nullable = false, length = 100)
    private String nombre;
    
    @ManyToOne
    @JoinColumn(name = "categoria_id")
    private Categoria categoria;
}
```

**C# EF Core:**
```csharp
public class Producto
{
    public long Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public long CategoriaId { get; set; }
    public Categoria Categoria { get; set; } = null!;
}

// DbContext configuration
public class TiendaDbContext : DbContext
{
    public DbSet<Producto> Productos => Set<Producto>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Producto>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
            entity.HasOne(e => e.Categoria)
                .WithMany(c => c.Productos)
                .HasForeignKey(e => e.CategoriaId);
        });
    }
}
```

**Ejercicio 12:** Crear entidades con relaciones

#### Día 5-7: Repository Pattern y LINQ Queries

**Java Repository:**
```java
public interface ProductoRepository extends JpaRepository<Producto, Long> {
    List<Producto> findByCategoriaId(Long categoriaId);
    
    @Query("SELECT p FROM Producto p WHERE p.precio < :maxPrecio")
    List<Producto> findByPrecioLessThan(@Param("maxPrecio") BigDecimal maxPrecio);
}
```

**C# Repository:**
```csharp
public interface IProductoRepository
{
    Task<IEnumerable<Producto>> FindByCategoriaIdAsync(long categoriaId);
    Task<IEnumerable<Producto>> FindByPrecioLessThanAsync(decimal maxPrecio);
}

public class ProductoRepository : IProductoRepository
{
    private readonly TiendaDbContext _context;
    
    public async Task<IEnumerable<Producto>> FindByCategoriaIdAsync(long categoriaId)
    {
        return await _context.Productos
            .Where(p => p.CategoriaId == categoriaId)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<Producto>> FindByPrecioLessThanAsync(decimal maxPrecio)
    {
        return await _context.Productos
            .Where(p => p.Precio < maxPrecio)
            .ToListAsync();
    }
}
```

**Ejercicio 13:** Implementar repository pattern

#### Día 8-10: MongoDB Integration

**Java Spring Data MongoDB:**
```java
@Document(collection = "pedidos")
public class Pedido {
    @Id
    private String id;
    private Long userId;
    private List<PedidoItem> items;
}

public interface PedidoRepository extends MongoRepository<Pedido, String> {
    List<Pedido> findByUserId(Long userId);
}
```

**C# MongoDB.Driver:**
```csharp
public class Pedido
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;
    
    public long UserId { get; set; }
    public List<PedidoItem> Items { get; set; } = new();
}

public class PedidoRepository
{
    private readonly IMongoCollection<Pedido> _pedidos;
    
    public async Task<List<Pedido>> FindByUserIdAsync(long userId)
    {
        return await _pedidos
            .Find(p => p.UserId == userId)
            .ToListAsync();
    }
}
```

**Ejercicio 14:** Integrar MongoDB para pedidos

### ✅ CHECKPOINT 4: Base de Datos Híbrida

**Proyecto:** TiendaApi con PostgreSQL + MongoDB

**Estructura de BD:**
- **PostgreSQL**: Categorías, Productos, Users (relational data)
- **MongoDB**: Pedidos (document data)

**Requisitos:**
- [x] EF Core con PostgreSQL configurado
- [x] MongoDB configurado para pedidos
- [x] Migrations de EF Core funcionando
- [x] Repository pattern implementado
- [x] CRUD completo en ambas BDs
- [x] Tests de integración con TestContainers

**Evaluación:**
- ¿Ambas bases de datos funcionan correctamente?
- ¿Las migraciones se aplican correctamente?
- ¿Los tests de integración pasan?

---

## 🗓️ SEMANA 9-10: Seguridad y Autenticación

### Objetivos
Implementar autenticación JWT y autorización basada en roles.

### 📚 Contenido Teórico

#### Día 1-4: JWT Authentication

**Ver documentación completa:**
- [05-authentication-security/jwt-authentication.md](./05-authentication-security/jwt-authentication.md)

**Java Spring Security:**
```java
@Configuration
@EnableWebSecurity
public class SecurityConfig {
    @Bean
    public SecurityFilterChain filterChain(HttpSecurity http) throws Exception {
        return http
            .csrf(csrf -> csrf.disable())
            .authorizeHttpRequests(auth -> auth
                .requestMatchers("/api/auth/**").permitAll()
                .anyRequest().authenticated())
            .sessionManagement(session -> 
                session.sessionCreationPolicy(SessionCreationPolicy.STATELESS))
            .addFilterBefore(jwtAuthFilter, UsernamePasswordAuthenticationFilter.class)
            .build();
    }
}
```

**C# ASP.NET Core:**
```csharp
// Program.cs
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

app.UseAuthentication();
app.UseAuthorization();
```

**Ejercicio 15:** Implementar login y registro con JWT

#### Día 5-7: Authorization con Roles

**Java:**
```java
@PreAuthorize("hasRole('ADMIN')")
@DeleteMapping("/{id}")
public ResponseEntity<Void> delete(@PathVariable Long id) {
    service.delete(id);
    return ResponseEntity.noContent().build();
}
```

**C#:**
```csharp
[Authorize(Roles = "ADMIN")]
[HttpDelete("{id}")]
public IActionResult Delete(long id)
{
    _service.Delete(id);
    return NoContent();
}
```

**Ejercicio 16:** Implementar autorización por roles

#### Día 8-10: Security Middleware y Claims

**Ejercicio 17:** Custom authorization policies

### ✅ CHECKPOINT 5: Autenticación y Autorización Completas

**Requisitos:**
- [x] Login endpoint retorna JWT
- [x] Register endpoint crea usuarios
- [x] Endpoints protegidos con [Authorize]
- [x] Authorization por roles (USER, ADMIN)
- [x] Tests de autenticación

**Evaluación:**
- ¿El JWT se genera correctamente?
- ¿Los endpoints protegidos rechazan requests sin token?
- ¿Los roles funcionan correctamente?

---

## 🗓️ SEMANA 11-12: Temas Avanzados

### Objetivos
Completar conocimientos con caching, WebSockets, testing y deployment.

### 📚 Contenido por Día

#### Día 1-2: Caching con IMemoryCache y Redis

**Ejercicio 18:** Implementar cache-aside pattern

#### Día 3-4: WebSockets Nativos

**Ejercicio 19:** Chat en tiempo real con WebSockets

#### Día 5-6: Background Services

**Ejercicio 20:** Tarea programada que ejecuta cada hora

#### Día 7-8: Email con MailKit

**Ejercicio 21:** Envío de emails de confirmación

#### Día 9-10: Testing Completo

**Ejercicio 22:** 
- Unit tests con NUnit y Moq
- Integration tests con TestContainers
- API tests con HttpClient

#### Día 11-12: Docker Deployment

**Ejercicio 23:** 
- Dockerfile multi-stage
- docker-compose con API + PostgreSQL + MongoDB + Redis

### ✅ CHECKPOINT FINAL: Aplicación Completa Deployada

**Proyecto Final: TiendaApi Completa**

**Features:**
- [x] REST API completa con CRUD
- [x] Exception handling Y Result Pattern
- [x] PostgreSQL + MongoDB
- [x] JWT Authentication
- [x] Role-based authorization
- [x] Caching con Redis
- [x] WebSockets para notificaciones
- [x] Email notifications
- [x] Background jobs
- [x] Tests comprehensivos (>80% coverage)
- [x] Docker Compose deployment
- [x] CI/CD pipeline básico

**Evaluación Final:**
- ¿La aplicación funciona end-to-end?
- ¿Todos los tests pasan?
- ¿Se puede deployar con Docker?
- ¿La documentación está completa?

---

## 📊 Rúbricas de Evaluación

### Evaluación por Semana

| Semana | Peso | Criterios |
|--------|------|-----------|
| 1-2 | 10% | Sintaxis C# y fundamentos |
| 3-4 | 15% | Web API y controllers |
| **5-6** | **30%** | **Manejo de errores (tema principal)** |
| 7-8 | 20% | Data access y repositories |
| 9-10 | 15% | Security y authentication |
| 11-12 | 10% | Temas avanzados |

### Criterios de Evaluación Detallados

#### Sintaxis y Conversión (20%)
- ✅ Código C# idiomático (no código Java traducido literalmente)
- ✅ Uso correcto de properties en lugar de getters/setters
- ✅ Null safety con nullable reference types
- ✅ LINQ en lugar de loops tradicionales
- ✅ Async/await en lugar de Task.Run

#### Patrones de Diseño (25%)
- ✅ Dependency Injection correctamente configurado
- ✅ Repository Pattern implementado
- ✅ Service Layer con lógica de negocio
- ✅ DTOs separados de entidades
- ✅ Separation of Concerns

#### Manejo de Errores (25%) - **TEMA PRINCIPAL**
- ✅ Entiende cuándo usar Exceptions vs Result Pattern
- ✅ GlobalExceptionHandler implementado correctamente
- ✅ Result Pattern con Railway Oriented Programming
- ✅ Error responses consistentes
- ✅ Tests para ambos enfoques

#### Data Access (15%)
- ✅ Entity Framework Core configurado correctamente
- ✅ Migrations funcionando
- ✅ Repository Pattern
- ✅ LINQ queries eficientes
- ✅ Integration tests con TestContainers

#### Security (10%)
- ✅ JWT authentication implementado
- ✅ Password hashing correcto
- ✅ Role-based authorization
- ✅ Security best practices

#### Proyecto Final (5%)
- ✅ Aplicación completa funcionando
- ✅ Tests comprehensivos
- ✅ Docker deployment
- ✅ Documentación completa

---

## 🎓 Recursos Adicionales

### Documentación Oficial
- [ASP.NET Core Documentation](https://docs.microsoft.com/en-us/aspnet/core/)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)
- [C# Programming Guide](https://docs.microsoft.com/en-us/dotnet/csharp/)

### Libros Recomendados
- **C# in Depth** por Jon Skeet
- **ASP.NET Core in Action** por Andrew Lock
- **Entity Framework Core in Action** por Jon P Smith

### Cursos Online
- Microsoft Learn - [ASP.NET Core Path](https://docs.microsoft.com/en-us/learn/aspnet/)
- Pluralsight - [ASP.NET Core Path](https://www.pluralsight.com/paths/aspnet-core)
- Udemy - [Complete ASP.NET Core Bootcamp](https://www.udemy.com/topic/asp-net-core/)

### Comunidades
- [r/dotnet](https://www.reddit.com/r/dotnet/)
- [Stack Overflow - ASP.NET Core](https://stackoverflow.com/questions/tagged/asp.net-core)
- [.NET Discord](https://discord.gg/dotnet)

---

## 🏆 Certificación y Próximos Pasos

### Después de Completar el Programa

#### 1. Contribuir a Open Source
- Busca proyectos .NET en GitHub
- Contribuye con bug fixes y features
- Aprende de code reviews

#### 2. Certificaciones Microsoft
- [Microsoft Certified: Azure Developer Associate](https://docs.microsoft.com/en-us/learn/certifications/azure-developer/)
- [Microsoft Certified: .NET Developer](https://docs.microsoft.com/en-us/learn/certifications/)

#### 3. Especialización
- **Microservices con .NET**
- **Blazor para Full Stack**
- **Azure Cloud Development**
- **Performance Optimization**

#### 4. Proyecto Personal
- Crea una aplicación completa
- Publícala en GitHub
- Desp lóyala en Azure o AWS
- Compártela en LinkedIn

---

## 📞 Soporte y Preguntas

### ¿Necesitas Ayuda?

1. **Revisa la documentación** en los directorios específicos
2. **Consulta los ejemplos** en el directorio `/ejemplos`
3. **Mira el código** de TiendaApi en `/api`
4. **Abre un issue** en GitHub si encuentras errores

### Feedback

Este programa es un trabajo en progreso. Si tienes sugerencias para mejorarlo:
- Abre un issue en GitHub
- Envía un pull request
- Contacta al autor

---

## 🎉 ¡Éxito en tu Migración!

Recuerda: **La migración de Java a C# no es solo aprender nueva sintaxis, es aprender a pensar de manera diferente**.

C# y .NET tienen sus propias idiosincrasias, patrones y mejores prácticas. ¡Disfruta el viaje! 🚀

---

**Autor:** José Luis González Sánchez  
**Repositorio:** [joseluisgs/prueba-c-sharp](https://github.com/joseluisgs/prueba-c-sharp)  
**Licencia:** Creative Commons  

---

**¡Comienza ahora con [Semana 1-2: Fundamentos C#](./01-fundamentos-aspnet/README.md)!**
