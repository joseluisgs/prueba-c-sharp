# ⚙️ Configuration - Configuración de Aplicaciones

## Introducción

Manejo de configuración en Spring Boot vs ASP.NET Core: archivos de configuración, profiles, variables de entorno.

## 📁 Archivos de Configuración

### Spring Boot

**application.yml:**
```yaml
server:
  port: 8080
  
spring:
  application:
    name: tienda-api
  datasource:
    url: jdbc:postgresql://localhost:5432/tienda
    username: postgres
    password: password
    driver-class-name: org.postgresql.Driver
  jpa:
    hibernate:
      ddl-auto: update
    show-sql: true
    properties:
      hibernate:
        format_sql: true
  
app:
  jwt:
    secret: mi-secreto-super-seguro-cambiar-en-produccion
    expiration: 86400000
  email:
    smtp-host: smtp.gmail.com
    smtp-port: 587
```

**application.properties (alternativa):**
```properties
server.port=8080
spring.application.name=tienda-api
spring.datasource.url=jdbc:postgresql://localhost:5432/tienda
spring.datasource.username=postgres
spring.datasource.password=password
```

### ASP.NET Core

**appsettings.json:**
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=tienda;Username=postgres;Password=password"
  },
  "Jwt": {
    "Secret": "mi-secreto-super-seguro-cambiar-en-produccion",
    "Issuer": "https://localhost:5000",
    "Audience": "https://localhost:5000",
    "ExpirationInMinutes": 1440
  },
  "Email": {
    "SmtpHost": "smtp.gmail.com",
    "SmtpPort": 587,
    "Username": "user@example.com",
    "Password": "password"
  }
}
```

## 🎭 Environment-Specific Configuration

### Spring Boot Profiles

**application-dev.yml:**
```yaml
spring:
  datasource:
    url: jdbc:h2:mem:testdb
  h2:
    console:
      enabled: true
logging:
  level:
    com.example: DEBUG
```

**application-prod.yml:**
```yaml
spring:
  datasource:
    url: ${DATABASE_URL}
    username: ${DB_USERNAME}
    password: ${DB_PASSWORD}
logging:
  level:
    com.example: WARN
```

**Activar profile:**
```bash
# En application.yml
spring:
  profiles:
    active: dev

# O por variable de entorno
export SPRING_PROFILES_ACTIVE=prod

# O por argumento
java -jar app.jar --spring.profiles.active=prod
```

### ASP.NET Core Environments

**appsettings.Development.json:**
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=tienda_dev;Username=dev;Password=dev"
  }
}
```

**appsettings.Production.json:**
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Host=prod-server;Database=tienda;Username=prod_user;Password=${DB_PASSWORD}"
  }
}
```

**Activar environment:**
```bash
# Variable de entorno
export ASPNETCORE_ENVIRONMENT=Development  # O Production, Staging

# En launchSettings.json (desarrollo)
{
  "profiles": {
    "Development": {
      "commandName": "Project",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  }
}

# En producción (Docker, IIS, etc.)
ASPNETCORE_ENVIRONMENT=Production dotnet TiendaApi.dll
```

## 📖 Leer Configuración en Código

### Java @Value y @ConfigurationProperties

```java
@Service
public class EmailService {
    
    // Inyección simple
    @Value("${app.email.smtp-host}")
    private String smtpHost;
    
    @Value("${app.email.smtp-port}")
    private int smtpPort;
    
    // Con valor por defecto
    @Value("${app.email.timeout:5000}")
    private int timeout;
}

// O mejor: @ConfigurationProperties
@ConfigurationProperties(prefix = "app.email")
@Component
public class EmailConfig {
    private String smtpHost;
    private int smtpPort;
    private int timeout = 5000;  // valor por defecto
    
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

### C# IConfiguration y Options Pattern

```csharp
// Inyección directa de IConfiguration
public class EmailService
{
    private readonly IConfiguration _configuration;
    
    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public void SendEmail()
    {
        var smtpHost = _configuration["Email:SmtpHost"];
        var smtpPort = _configuration.GetValue<int>("Email:SmtpPort");
        var timeout = _configuration.GetValue<int>("Email:Timeout", 5000); // default
    }
}

// MEJOR: Options Pattern (recomendado)
public class EmailConfig
{
    public string SmtpHost { get; set; } = string.Empty;
    public int SmtpPort { get; set; }
    public int Timeout { get; set; } = 5000;
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
    
    public void SendEmail()
    {
        var smtp = new SmtpClient(_config.SmtpHost, _config.SmtpPort);
        // ...
    }
}
```

## 🔐 Secrets y Variables de Entorno

### Spring Boot

```yaml
# application.yml
spring:
  datasource:
    url: ${DATABASE_URL:jdbc:postgresql://localhost:5432/tienda}
    username: ${DB_USERNAME:postgres}
    password: ${DB_PASSWORD:password}

app:
  jwt:
    secret: ${JWT_SECRET:default-secret-change-in-production}
```

**User Secrets (desarrollo):**
```bash
# Usar Spring Cloud Config o Environment Variables
export DB_PASSWORD=my-secret-password
export JWT_SECRET=super-secret-jwt-key
```

### ASP.NET Core

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=${DB_HOST};Database=${DB_NAME};Username=${DB_USER};Password=${DB_PASSWORD}"
  },
  "Jwt": {
    "Secret": "${JWT_SECRET}"
  }
}
```

**User Secrets (desarrollo):**
```bash
# Inicializar User Secrets
dotnet user-secrets init

# Agregar secrets
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Database=tienda_dev;Username=dev;Password=dev123"
dotnet user-secrets set "Jwt:Secret" "dev-jwt-secret-key"

# Listar secrets
dotnet user-secrets list
```

**Environment Variables (producción):**
```bash
# Linux/Mac
export ConnectionStrings__DefaultConnection="Host=prod;Database=tienda;..."
export Jwt__Secret="prod-jwt-secret"

# Windows
set ConnectionStrings__DefaultConnection=Host=prod;Database=tienda;...
set Jwt__Secret=prod-jwt-secret

# Docker
docker run -e ConnectionStrings__DefaultConnection="..." \
           -e Jwt__Secret="..." \
           myapp:latest
```

**Nota:** En .NET, usa `__` (double underscore) para jerarquía en environment variables.

## 🎯 Configuration Binding

### Java Record/Class Binding

```java
@ConfigurationProperties(prefix = "app.database")
@Component
public class DatabaseConfig {
    private String host;
    private int port;
    private String database;
    private PoolSettings pool;
    
    public static class PoolSettings {
        private int minSize;
        private int maxSize;
        private int timeout;
        // Getters y setters
    }
    
    // Getters y setters
}

// application.yml
app:
  database:
    host: localhost
    port: 5432
    database: tienda
    pool:
      min-size: 5
      max-size: 20
      timeout: 30
```

### C# Strong-Typed Configuration

```csharp
public class DatabaseConfig
{
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; }
    public string Database { get; set; } = string.Empty;
    public PoolSettings Pool { get; set; } = new();
    
    public class PoolSettings
    {
        public int MinSize { get; set; }
        public int MaxSize { get; set; }
        public int Timeout { get; set; }
    }
}

// Program.cs
builder.Services.Configure<DatabaseConfig>(
    builder.Configuration.GetSection("Database"));

// appsettings.json
{
  "Database": {
    "Host": "localhost",
    "Port": 5432,
    "Database": "tienda",
    "Pool": {
      "MinSize": 5,
      "MaxSize": 20,
      "Timeout": 30
    }
  }
}

// Usage
public class SomeService
{
    private readonly DatabaseConfig _dbConfig;
    
    public SomeService(IOptions<DatabaseConfig> options)
    {
        _dbConfig = options.Value;
    }
}
```

## 🔄 Configuration Reload

### Spring Boot

```yaml
# application.yml
spring:
  cloud:
    config:
      refresh:
        enabled: true

management:
  endpoints:
    web:
      exposure:
        include: refresh
```

```bash
# Refresh configuration
curl -X POST http://localhost:8080/actuator/refresh
```

### ASP.NET Core

```csharp
// Program.cs - Configuration se recarga automáticamente por defecto
builder.Configuration.AddJsonFile("appsettings.json", 
    optional: false, 
    reloadOnChange: true);  // ← Auto-reload

// Para reaccionar a cambios
public class SomeService
{
    private readonly IOptionsMonitor<AppSettings> _options;
    
    public SomeService(IOptionsMonitor<AppSettings> options)
    {
        _options = options;
        
        // Reaccionar a cambios
        _options.OnChange(settings =>
        {
            // Configuration changed!
            Console.WriteLine($"New timeout: {settings.Timeout}");
        });
    }
}
```

## 🗂️ Configuration Sources Priority

### Spring Boot (orden de precedencia)

1. Command line arguments
2. SPRING_APPLICATION_JSON properties
3. ServletConfig/ServletContext init parameters
4. JNDI attributes
5. Java System properties
6. OS environment variables
7. Profile-specific properties (application-{profile}.yml)
8. Application properties (application.yml)
9. @PropertySource annotations
10. Default properties

### ASP.NET Core (orden de precedencia)

1. Command-line arguments
2. Environment variables
3. User secrets (Development only)
4. appsettings.{Environment}.json
5. appsettings.json
6. Host configuration

```csharp
// Program.cs - Custom configuration
var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables()
    .AddCommandLine(args)
    .AddUserSecrets<Program>();  // Development only
```

## 📊 Configuration Validation

### Java Validation

```java
@ConfigurationProperties(prefix = "app.email")
@Validated
@Component
public class EmailConfig {
    
    @NotBlank
    private String smtpHost;
    
    @Min(1)
    @Max(65535)
    private int smtpPort;
    
    @Email
    private String username;
    
    // Getters y setters
}
```

### C# Validation

```csharp
public class EmailConfig
{
    [Required]
    public string SmtpHost { get; set; } = string.Empty;
    
    [Range(1, 65535)]
    public int SmtpPort { get; set; }
    
    [EmailAddress]
    public string Username { get; set; } = string.Empty;
}

// Program.cs
builder.Services.AddOptions<EmailConfig>()
    .Bind(builder.Configuration.GetSection("Email"))
    .ValidateDataAnnotations()
    .ValidateOnStart();  // Validate on startup

// O validación custom
builder.Services.AddOptions<EmailConfig>()
    .Bind(builder.Configuration.GetSection("Email"))
    .Validate(config =>
    {
        if (config.SmtpPort < 1 || config.SmtpPort > 65535)
            return false;
        return true;
    }, "SmtpPort must be between 1 and 65535");
```

## 🔗 Referencias

- [ASP.NET Core Configuration](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/)
- [Options Pattern](https://docs.microsoft.com/en-us/dotnet/core/extensions/options)
- [Spring Boot Configuration](https://docs.spring.io/spring-boot/reference/features/external-config.html)
- [User Secrets in .NET](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets)

---

**Anterior:** [← Dependency Injection](./dependency-injection.md) | **Siguiente:** [Web API Controllers →](../02-web-api-controllers/README.md)
