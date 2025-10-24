# 🗂️ Project Structure - Estructura de Proyectos

## Comparativa: Maven/Gradle vs .csproj

### Java Maven Project

```xml
<!-- pom.xml -->
<project>
    <groupId>com.example</groupId>
    <artifactId>tienda-api</artifactId>
    <version>1.0.0</version>
    <packaging>jar</packaging>
    
    <parent>
        <groupId>org.springframework.boot</groupId>
        <artifactId>spring-boot-starter-parent</artifactId>
        <version>3.2.0</version>
    </parent>
    
    <dependencies>
        <dependency>
            <groupId>org.springframework.boot</groupId>
            <artifactId>spring-boot-starter-web</artifactId>
        </dependency>
        <dependency>
            <groupId>org.springframework.boot</groupId>
            <artifactId>spring-boot-starter-data-jpa</artifactId>
        </dependency>
        <dependency>
            <groupId>org.postgresql</groupId>
            <artifactId>postgresql</artifactId>
        </dependency>
    </dependencies>
</project>
```

### C# .csproj File

```xml
<!-- TiendaApi.csproj -->
<Project Sdk="Microsoft.NET.Sdk.Web">
    <PropertyGroup>
        <TargetFramework>net8.0</TargetFramework>
        <Nullable>enable</Nullable>
        <ImplicitUsings>enable</ImplicitUsings>
    </PropertyGroup>
    
    <ItemGroup>
        <PackageReference Include="Microsoft.AspNetCore.OpenApi" Version="8.0.0" />
        <PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.0.0" />
        <PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="8.0.0" />
        <PackageReference Include="Swashbuckle.AspNetCore" Version="6.5.0" />
    </ItemGroup>
</Project>
```

## 📁 Directory Structure Comparison

### Spring Boot Structure

```
tienda-api/
├── src/
│   ├── main/
│   │   ├── java/
│   │   │   └── com/
│   │   │       └── example/
│   │   │           └── tienda/
│   │   │               ├── TiendaApplication.java          # Entry point
│   │   │               ├── controller/                      # REST Controllers
│   │   │               │   ├── CategoriaController.java
│   │   │               │   └── ProductoController.java
│   │   │               ├── service/                         # Business logic
│   │   │               │   ├── CategoriaService.java
│   │   │               │   └── ProductoService.java
│   │   │               ├── repository/                      # Data access
│   │   │               │   ├── CategoriaRepository.java
│   │   │               │   └── ProductoRepository.java
│   │   │               ├── model/                           # Domain models
│   │   │               │   ├── entity/
│   │   │               │   │   ├── Categoria.java
│   │   │               │   │   └── Producto.java
│   │   │               │   └── dto/
│   │   │               │       ├── CategoriaDto.java
│   │   │               │       └── ProductoDto.java
│   │   │               ├── config/                          # Configuration
│   │   │               │   ├── SecurityConfig.java
│   │   │               │   └── DatabaseConfig.java
│   │   │               └── exception/                       # Custom exceptions
│   │   │                   ├── NotFoundException.java
│   │   │                   └── GlobalExceptionHandler.java
│   │   └── resources/
│   │       ├── application.yml                             # Configuration
│   │       ├── application-dev.yml
│   │       ├── application-prod.yml
│   │       └── db/
│   │           └── migration/                              # Flyway migrations
│   │               ├── V1__create_tables.sql
│   │               └── V2__insert_data.sql
│   └── test/
│       └── java/
│           └── com/
│               └── example/
│                   └── tienda/
│                       ├── controller/
│                       ├── service/
│                       └── repository/
├── pom.xml                                                 # Build file
└── README.md
```

### ASP.NET Core Structure

```
TiendaApi/
├── Controllers/                                            # REST Controllers
│   ├── CategoriasController.cs
│   └── ProductosController.cs
├── Services/                                               # Business logic
│   ├── CategoriaService.cs
│   ├── ProductoService.cs
│   └── MappingProfile.cs                                   # AutoMapper
├── Repositories/                                           # Data access
│   ├── ICategoriaRepository.cs
│   ├── CategoriaRepository.cs
│   ├── IProductoRepository.cs
│   └── ProductoRepository.cs
├── Models/                                                 # Domain models
│   ├── Entities/
│   │   ├── Categoria.cs
│   │   └── Producto.cs
│   └── DTOs/
│       ├── CategoriaDto.cs
│       └── ProductoDto.cs
├── Data/                                                   # Database context
│   ├── TiendaDbContext.cs
│   └── Migrations/                                         # EF Core migrations
│       ├── 20240101000000_InitialCreate.cs
│       └── 20240102000000_AddProductos.cs
├── Middleware/                                             # Custom middleware
│   └── GlobalExceptionHandler.cs
├── Exceptions/                                             # Custom exceptions
│   ├── NotFoundException.cs
│   └── ValidationException.cs
├── Common/                                                 # Shared code
│   ├── Result.cs
│   └── AppError.cs
├── Program.cs                                              # Entry point + config
├── appsettings.json                                        # Configuration
├── appsettings.Development.json
├── appsettings.Production.json
├── TiendaApi.csproj                                        # Build file
└── README.md

TiendaApi.Tests/                                            # Test project
├── Controllers/
├── Services/
├── Repositories/
└── TiendaApi.Tests.csproj
```

## 🔑 Key Differences

### 1. Flat Structure vs Package Hierarchy

**Java:**
- Deep package hierarchy: `com.example.tienda.controller`
- Enforced by package naming convention

**C#:**
- Flat namespace: `TiendaApi.Controllers`
- Folders for organization, but not required in namespace

### 2. Entry Point

**Java:**
```java
@SpringBootApplication
public class TiendaApplication {
    public static void main(String[] args) {
        SpringApplication.run(TiendaApplication.class, args);
    }
}
```

**C# (.NET 6+):**
```csharp
// Program.cs - Top-level statements
var builder = WebApplication.CreateBuilder(args);

// Configure services
builder.Services.AddControllers();
builder.Services.AddDbContext<TiendaDbContext>();

var app = builder.Build();

// Configure middleware
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
```

### 3. Configuration Files

**Java:**
```yaml
# application.yml
spring:
  datasource:
    url: jdbc:postgresql://localhost:5432/tienda
    username: postgres
    password: password
  jpa:
    hibernate:
      ddl-auto: update
    show-sql: true

server:
  port: 8080
```

**C#:**
```json
// appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=tienda;Username=postgres;Password=password"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  },
  "AllowedHosts": "*"
}
```

### 4. Build Commands

**Java Maven:**
```bash
mvn clean install           # Build
mvn spring-boot:run        # Run
mvn test                   # Test
mvn package                # Package JAR
```

**C# dotnet CLI:**
```bash
dotnet restore             # Restore dependencies
dotnet build               # Build
dotnet run                 # Run
dotnet test                # Test
dotnet publish -c Release  # Publish
```

## 📦 Solution Structure (Multi-Project)

En .NET, típicamente organizas múltiples proyectos en una **Solution**:

```
TiendaSolution.sln                          # Solution file
├── src/
│   ├── TiendaApi/                          # Web API project
│   │   ├── Controllers/
│   │   ├── Services/
│   │   └── TiendaApi.csproj
│   ├── TiendaApi.Core/                     # Business logic project
│   │   ├── Entities/
│   │   ├── Interfaces/
│   │   └── TiendaApi.Core.csproj
│   └── TiendaApi.Infrastructure/           # Data access project
│       ├── Repositories/
│       ├── Data/
│       └── TiendaApi.Infrastructure.csproj
└── tests/
    ├── TiendaApi.Tests/                    # Unit tests
    └── TiendaApi.IntegrationTests/         # Integration tests
```

**Equivalent to Java Multi-Module Maven:**
```xml
<modules>
    <module>tienda-api</module>
    <module>tienda-core</module>
    <module>tienda-infrastructure</module>
</modules>
```

## 🛠️ Creating a New Project

### Java Spring Boot

```bash
# Using Spring Initializr
curl https://start.spring.io/starter.zip \
  -d dependencies=web,data-jpa,postgresql \
  -d groupId=com.example \
  -d artifactId=tienda-api \
  -o tienda-api.zip

unzip tienda-api.zip
cd tienda-api
mvn spring-boot:run
```

### C# ASP.NET Core

```bash
# Using dotnet CLI
dotnet new webapi -n TiendaApi
cd TiendaApi

# Add packages
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL

# Run
dotnet run
```

## 📋 Project File Comparison

### Maven Dependencies

```xml
<dependency>
    <groupId>org.springframework.boot</groupId>
    <artifactId>spring-boot-starter-web</artifactId>
</dependency>
```

### NuGet Packages

```xml
<PackageReference Include="Microsoft.AspNetCore.OpenApi" Version="8.0.0" />
```

Or via CLI:
```bash
dotnet add package Microsoft.AspNetCore.OpenApi
```

## 🔗 References

- [ASP.NET Core Project Structure](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/)
- [.NET CLI Reference](https://docs.microsoft.com/en-us/dotnet/core/tools/)
- [Maven vs .NET Comparison](https://docs.microsoft.com/en-us/dotnet/standard/library-guidance/)

---

**Anterior:** [← README](./README.md) | **Siguiente:** [Dependency Injection →](./dependency-injection.md)
