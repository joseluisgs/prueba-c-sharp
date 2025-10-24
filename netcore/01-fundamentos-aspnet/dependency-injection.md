# 💉 Dependency Injection - DI Container

## Introducción

Dependency Injection es fundamental tanto en Spring Boot como en ASP.NET Core. Esta guía compara ambos sistemas.

## 🔄 Conceptos Básicos

### Spring IoC Container

```java
@Configuration
public class AppConfig {
    
    @Bean
    public UserService userService(UserRepository repository) {
        return new UserService(repository);
    }
    
    @Bean
    @Scope("singleton")  // Default
    public CacheService cacheService() {
        return new CacheService();
    }
    
    @Bean
    @Scope("prototype")  // New instance cada vez
    public RequestProcessor requestProcessor() {
        return new RequestProcessor();
    }
}

// O con component scanning:
@Component
public class UserService {
    private final UserRepository repository;
    
    @Autowired  // O constructor injection (recomendado)
    public UserService(UserRepository repository) {
        this.repository = repository;
    }
}
```

### .NET DI Container

```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

// Singleton - Una instancia para toda la aplicación
builder.Services.AddSingleton<ICacheService, CacheService>();

// Scoped - Una instancia por request HTTP
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Transient - Nueva instancia cada vez
builder.Services.AddTransient<IRequestProcessor, RequestProcessor>();

// Con factory
builder.Services.AddScoped<IEmailService>(provider => 
{
    var config = provider.GetRequiredService<IConfiguration>();
    return new EmailService(config["SmtpHost"]!);
});

var app = builder.Build();
```

## 📊 Lifetimes Comparison

| Spring | .NET | Descripción |
|--------|------|-------------|
| `@Scope("singleton")` | `AddSingleton<T>` | Una instancia para toda la app |
| `@Scope("request")` | `AddScoped<T>` | Una instancia por HTTP request |
| `@Scope("prototype")` | `AddTransient<T>` | Nueva instancia cada vez |
| N/A | `AddScoped<T>` | Específico de .NET para request scope |

## 🏗️ Annotations vs Extension Methods

### Java Annotations

```java
@Component  // Generic component
public class MyComponent { }

@Service  // Business logic
public class UserService { }

@Repository  // Data access
public class UserRepository { }

@Controller  // Web controller
public class UserController { }

@Configuration  // Configuration class
public class AppConfig { }
```

### C# Extension Methods

```csharp
// No hay annotations equivalentes en C#
// Todo se registra en Program.cs:

// Services (business logic)
builder.Services.AddScoped<IUserService, UserService>();

// Repositories (data access)
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Controllers (automático con AddControllers())
builder.Services.AddControllers();

// DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));
```

## 🎯 Constructor Injection (Recomendado)

### Java

```java
@Service
public class OrderService {
    private final OrderRepository orderRepository;
    private final PaymentService paymentService;
    private final EmailService emailService;
    
    // Constructor injection (recomendado)
    @Autowired  // Opcional en constructores únicos desde Spring 4.3
    public OrderService(
        OrderRepository orderRepository,
        PaymentService paymentService,
        EmailService emailService) {
        
        this.orderRepository = orderRepository;
        this.paymentService = paymentService;
        this.emailService = emailService;
    }
}
```

### C#

```csharp
public class OrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IPaymentService _paymentService;
    private readonly IEmailService _emailService;
    
    // Constructor injection (única forma en ASP.NET Core)
    public OrderService(
        IOrderRepository orderRepository,
        IPaymentService paymentService,
        IEmailService emailService)
    {
        _orderRepository = orderRepository;
        _paymentService = paymentService;
        _emailService = emailService;
    }
}
```

**Nota importante:** En .NET, **constructor injection es la única forma soportada oficialmente**. No hay equivalente a `@Autowired` en fields o setters.

## 🔧 Configuration con Options Pattern

### Java @ConfigurationProperties

```java
@ConfigurationProperties(prefix = "app.email")
@Component
public class EmailConfig {
    private String smtpHost;
    private int smtpPort;
    private String username;
    private String password;
    
    // Getters y setters
}

@Service
public class EmailService {
    private final EmailConfig config;
    
    @Autowired
    public EmailService(EmailConfig config) {
        this.config = config;
    }
}
```

### C# Options Pattern

```csharp
// appsettings.json
{
  "Email": {
    "SmtpHost": "smtp.gmail.com",
    "SmtpPort": 587,
    "Username": "user@example.com",
    "Password": "password"
  }
}

// EmailConfig.cs
public class EmailConfig
{
    public string SmtpHost { get; set; } = string.Empty;
    public int SmtpPort { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

// Program.cs
builder.Services.Configure<EmailConfig>(
    builder.Configuration.GetSection("Email"));

// EmailService.cs
public class EmailService
{
    private readonly EmailConfig _config;
    
    public EmailService(IOptions<EmailConfig> options)
    {
        _config = options.Value;
    }
}
```

## 🔍 Resolving Dependencies Manually

### Java

```java
@Service
public class SomeService {
    
    @Autowired
    private ApplicationContext context;
    
    public void doSomething() {
        // Resolver en runtime
        UserService userService = context.getBean(UserService.class);
        userService.doWork();
    }
}
```

### C#

```csharp
public class SomeService
{
    private readonly IServiceProvider _serviceProvider;
    
    public SomeService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public void DoSomething()
    {
        // Resolver en runtime (usar con cuidado)
        using var scope = _serviceProvider.CreateScope();
        var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
        userService.DoWork();
    }
}
```

**⚠️ Warning:** Resolver manualmente es un anti-pattern. Prefiere constructor injection.

## 🎨 Multiple Implementations

### Java Qualifier

```java
public interface NotificationService {
    void send(String message);
}

@Service
@Qualifier("email")
public class EmailNotificationService implements NotificationService { }

@Service
@Qualifier("sms")
public class SmsNotificationService implements NotificationService { }

@Service
public class OrderService {
    private final NotificationService emailNotificationService;
    
    @Autowired
    public OrderService(@Qualifier("email") NotificationService notificationService) {
        this.emailNotificationService = notificationService;
    }
}
```

### C# Named Services

```csharp
public interface INotificationService
{
    void Send(string message);
}

public class EmailNotificationService : INotificationService { }
public class SmsNotificationService : INotificationService { }

// Program.cs - Registrar ambos
builder.Services.AddKeyedScoped<INotificationService, EmailNotificationService>("email");
builder.Services.AddKeyedScoped<INotificationService, SmsNotificationService>("sms");

// O registrar todos
builder.Services.AddScoped<EmailNotificationService>();
builder.Services.AddScoped<SmsNotificationService>();

// OrderService.cs
public class OrderService
{
    private readonly EmailNotificationService _emailService;
    
    // Opción 1: Inyectar implementación específica
    public OrderService(EmailNotificationService emailService)
    {
        _emailService = emailService;
    }
    
    // Opción 2: Inyectar todas y elegir
    public OrderService(IEnumerable<INotificationService> services)
    {
        _emailService = services.OfType<EmailNotificationService>().First();
    }
}
```

## 🔄 Conditional Registration

### Java Conditional

```java
@Configuration
public class AppConfig {
    
    @Bean
    @ConditionalOnProperty(name = "cache.enabled", havingValue = "true")
    public CacheService cacheService() {
        return new RedisCacheService();
    }
    
    @Bean
    @ConditionalOnProperty(name = "cache.enabled", havingValue = "false")
    public CacheService memoryCacheService() {
        return new InMemoryCacheService();
    }
}
```

### C# Conditional

```csharp
// Program.cs
if (builder.Configuration.GetValue<bool>("Cache:Enabled"))
{
    builder.Services.AddScoped<ICacheService, RedisCacheService>();
}
else
{
    builder.Services.AddScoped<ICacheService, InMemoryCacheService>();
}

// O más limpio con extension methods:
builder.Services.AddCacheService(builder.Configuration);

// Extension method
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCacheService(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        if (configuration.GetValue<bool>("Cache:Enabled"))
        {
            services.AddScoped<ICacheService, RedisCacheService>();
        }
        else
        {
            services.AddScoped<ICacheService, InMemoryCacheService>();
        }
        
        return services;
    }
}
```

## 🧪 Testing con DI

### Java

```java
@SpringBootTest
class OrderServiceTest {
    
    @Autowired
    private OrderService orderService;
    
    @MockBean
    private OrderRepository orderRepository;
    
    @Test
    void testCreateOrder() {
        when(orderRepository.save(any())).thenReturn(order);
        
        Order result = orderService.create(orderDto);
        
        assertNotNull(result);
    }
}
```

### C#

```csharp
[TestFixture]
public class OrderServiceTests
{
    private Mock<IOrderRepository> _orderRepositoryMock;
    private OrderService _orderService;
    
    [SetUp]
    public void Setup()
    {
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _orderService = new OrderService(_orderRepositoryMock.Object);
    }
    
    [Test]
    public async Task CreateOrder_ValidOrder_ReturnsOrder()
    {
        // Arrange
        _orderRepositoryMock.Setup(r => r.SaveAsync(It.IsAny<Order>()))
            .ReturnsAsync(order);
        
        // Act
        var result = await _orderService.CreateAsync(orderDto);
        
        // Assert
        Assert.That(result, Is.Not.Null);
    }
}
```

## ✅ Best Practices

### Spring Boot
1. ✅ Prefiere constructor injection sobre `@Autowired` en fields
2. ✅ Usa `@Qualifier` para múltiples implementaciones
3. ✅ Marca dependencias como `final` en constructor injection
4. ✅ Usa `@ConfigurationProperties` para configuración tipada
5. ✅ Evita circular dependencies

### ASP.NET Core
1. ✅ **SIEMPRE** usa constructor injection (única opción oficial)
2. ✅ Usa interfaces para abstracción
3. ✅ Marca dependencias como `readonly`
4. ✅ Usa Options Pattern para configuración
5. ✅ Evita `IServiceProvider` manual resolution
6. ✅ Registra services en orden lógico (DbContext → Repositories → Services)

## 🔗 Referencias

- [ASP.NET Core Dependency Injection](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection)
- [Spring Framework IoC Container](https://docs.spring.io/spring-framework/reference/core/beans.html)
- [Options Pattern in .NET](https://docs.microsoft.com/en-us/dotnet/core/extensions/options)

---

**Anterior:** [← Project Structure](./project-structure.md) | **Siguiente:** [Configuration →](./configuration.md)
