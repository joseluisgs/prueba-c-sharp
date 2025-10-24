# 🚀 Migración Java/Spring Boot a C#/.NET - Proyecto Educativo

## Tabla de Contenidos
- [Descripción del Proyecto](#descripción-del-proyecto)
- [Objetivos Educativos](#objetivos-educativos)
- [Estructura del Repositorio](#estructura-del-repositorio)
- [Tecnologías Migradas](#tecnologías-migradas)
- [Instalación y Configuración](#instalación-y-configuración)
- [Ejemplos Disponibles](#ejemplos-disponibles)
- [Guías de Migración](#guías-de-migración)
- [Testing](#testing)
- [Docker y Deployment](#docker-y-deployment)
- [Recursos Adicionales](#recursos-adicionales)

## Descripción del Proyecto

Este repositorio contiene una **migración completa y educativa** de un proyecto de Java/Spring Boot a C#/.NET, diseñado específicamente para estudiantes que vienen del ecosistema Java y quieren aprender .NET de manera gradual y comprensible.

### Proyecto Original
- **Fuente:** [Tienda API Spring Boot](https://github.com/joseluisgs/DesarrolloWebEntornosServidor-02-Proyecto-SpringBoot)
- **Tecnología:** Java 25 + Spring Boot 3.5.6 + Gradle
- **Características:** REST API completa con auth, cache, WebSockets, email, etc.

### Proyecto Migrado
- **Tecnología:** C# 12 + ASP.NET Core 8 + Entity Framework Core
- **Arquitectura:** Por capas/dominios (manteniendo la estructura familiar)
- **Testing:** NUnit + Moq (más similar a JUnit que xUnit)

## Objetivos Educativos

### 🎯 Para Estudiantes Java
- Comprender las **similitudes y diferencias** entre Java y C#
- Migrar conceptos de **Spring Boot a ASP.NET Core**
- Aprender **Entity Framework Core** desde una perspectiva JPA
- Dominar **NUnit + Moq** viniendo de JUnit + Mockito
- Entender **async/await** vs CompletableFuture

### 🎯 Para Profesores
- Recurso didáctico completo con **ejemplos progresivos**
- Documentación exhaustiva con **comparativas lado a lado**
- Código completamente **comentado en español**
- **Mejores prácticas** profesionales aplicadas

## Estructura del Repositorio

```
📦 joseluisgs/prueba-c-sharp/
│
├── 📁 api/                          # Puerto completo Tienda API
│   ├── TiendaApi.sln               # Solution principal
│   ├── TiendaApi/                  # Web API project
│   ├── TiendaApi.Tests/            # Tests completos
│   └── docker-compose.yml          # PostgreSQL + Redis + MongoDB
│
├── 📁 ejemplos/                     # Ejemplos educativos individuales
│   ├── 01-data-access-ef/          # Entity Framework Core
│   ├── 02-data-access-adonet/      # ADO.NET manual
│   ├── 03-http-clients-refit/      # Retrofit → Refit
│   ├── 04-async-programming/       # CompletableFuture → Task
│   ├── 05-reactive-programming/    # RxJava → System.Reactive
│   ├── 06-dependency-injection/    # Spring DI → .NET DI
│   ├── 07-testing-patterns/        # JUnit/Mockito → NUnit/Moq
│   ├── 08-validation/              # Bean Validation → FluentValidation
│   ├── 09-caching/                 # Spring Cache → IMemoryCache + Redis
│   ├── 10-email-services/          # JavaMail → MailKit
│   └── 11-websockets/              # Spring WebSocket → WebSockets nativos
│
├── 📁 csharp/                       # Documentación Java → C#
│   ├── 01-fundamentos/             # Sintaxis, tipos, variables
│   ├── 02-oop/                     # OOP concepts
│   ├── 03-collections/             # Collections comparison
│   ├── 04-streams-linq/            # Stream API vs LINQ
│   ├── 05-generics/                # Generics comparison
│   ├── 06-exceptions/              # Exception handling
│   ├── 07-annotations-attributes/  # @Annotations vs [Attributes]
│   └── 08-data-access/             # EF Core vs ADO.NET
│
└── 📁 netcore/                      # Documentación Spring Boot → ASP.NET Core
    ├── 01-fundamentos/             # Framework comparison
    ├── 02-web-api/                 # Spring MVC vs ASP.NET Core
    ├── 03-data-access/             # JPA → EF Core
    ├── 04-security/                # Spring Security vs ASP.NET Identity
    ├── 05-testing/                 # JUnit → NUnit + Moq
    ├── 06-caching/                 # Spring Cache vs .NET Caching
    ├── 07-messaging/               # Email, WebSockets
    └── 08-deployment/              # Docker, deployment
```

## Tecnologías Migradas

### Framework Principal
| Java/Spring Boot | C#/.NET | Descripción |
|------------------|---------|-------------|
| Spring Boot 3.5.6 | ASP.NET Core 8 | Framework web principal |
| Spring MVC | ASP.NET Core Web API | Controllers y routing |
| Spring Data JPA | Entity Framework Core | ORM para SQL |
| Spring Data MongoDB | MongoDB.Driver | Driver NoSQL |
| Spring Security | ASP.NET Core Identity + JWT | Autenticación y autorización |

### Acceso a Datos
| Java | C# | Uso |
|------|-----|-----|
| Spring Data JPA | Entity Framework Core | ORM principal (recomendado) |
| JDBC | ADO.NET | Acceso directo (control total) |
| H2 (in-memory) | InMemory EF Provider | Base de datos en memoria |
| PostgreSQL | Npgsql.EntityFrameworkCore.PostgreSQL | Base de datos principal |

### Testing
| Java | C# | Razón |
|------|-----|-------|
| JUnit 5 | NUnit 3.14 | Más similar sintácticamente |
| Mockito | Moq | Patrón de mocking similar |
| TestContainers | TestContainers.NET | Integration testing |

### Comunicación
| Java | C# | Funcionalidad |
|------|-----|---------------|
| Retrofit | Refit | HTTP clients declarativos |
| JavaMail | MailKit | Envío de emails |
| Spring WebSocket | WebSockets nativos | Real-time communication |

### Cache y Performance
| Java | C# | Alcance |
|------|-----|--------|
| Spring Cache | IMemoryCache | Cache local |
| Spring Data Redis | StackExchange.Redis | Cache distribuido |

## Instalación y Configuración

### Prerrequisitos
- .NET 8 SDK
- Visual Studio 2022 o VS Code
- Docker Desktop
- PostgreSQL (opcional, se puede usar con Docker)

### Clonar el Repositorio
```bash
git clone https://github.com/joseluisgs/prueba-c-sharp.git
cd prueba-c-sharp
```

### Ejecutar la API Principal
```bash
cd api
docker-compose up -d  # Inicia PostgreSQL + Redis + MongoDB
cd TiendaApi
dotnet run
```

### Ejecutar Ejemplos Individuales
```bash
cd ejemplos/01-data-access-ef/EjemploEF.Console
dotnet run
```

## Ejemplos Disponibles

### 📚 Ejemplos de Acceso a Datos
- **[Entity Framework Core](./ejemplos/01-data-access-ef/)** - ORM completo (equivalente a JPA)
- **[ADO.NET Manual](./ejemplos/02-data-access-adonet/)** - Control total sobre SQL

### 🌐 Ejemplos de Comunicación
- **[Refit HTTP Client](./ejemplos/03-http-clients-refit/)** - Retrofit → Refit
- **[Async Programming](./ejemplos/04-async-programming/)** - CompletableFuture → Task/async-await
- **[Reactive Programming](./ejemplos/05-reactive-programming/)** - RxJava → System.Reactive

### 🧪 Ejemplos de Testing
- **[Testing Patterns](./ejemplos/07-testing-patterns/)** - JUnit/Mockito → NUnit/Moq completo
- **[Validation](./ejemplos/08-validation/)** - Bean Validation → FluentValidation

### ⚡ Ejemplos de Performance
- **[Caching](./ejemplos/09-caching/)** - Spring Cache → IMemoryCache + Redis
- **[WebSockets](./ejemplos/11-websockets/)** - Comunicación en tiempo real

## Guías de Migración

### 🔄 De Java a C#
- **[Fundamentos](./csharp/01-fundamentos/)** - Sintaxis y conceptos básicos
- **[OOP](./csharp/02-oop/)** - Programación orientada a objetos
- **[Collections](./csharp/03-collections/)** - Colecciones y estructuras de datos
- **[LINQ vs Stream API](./csharp/04-streams-linq/)** - Comparativa detallada

### 🔄 De Spring Boot a ASP.NET Core
- **[Fundamentos](./netcore/01-fundamentos/)** - Conceptos del framework
- **[Web API](./netcore/02-web-api/)** - Controllers y routing
- **[Data Access](./netcore/03-data-access/)** - JPA → Entity Framework Core
- **[Security](./netcore/04-security/)** - Autenticación y autorización

## Testing

### Framework de Testing: NUnit
**¿Por qué NUnit y no xUnit?**
- Sintaxis **más similar a JUnit** (`[Test]` vs `@Test`)
- Mejor transición para estudiantes Java
- `Assert.Multiple()` equivalente a `assertAll()`

### Estrategia de Testing
```csharp
[TestFixture]
public class ProductoServiceTest
{
    [SetUp]  // Equivalente a @BeforeEach
    public void Setup() { }
    
    [Test]
    [DisplayName("Debe crear un producto válido")]  // Equivalente a @DisplayName
    public async Task CrearProducto_ConDatosValidos_DeberiaCrearCorrectamente()
    {
        // Arrange
        var producto = new Producto { Nombre = "Test" };
        
        // Act
        var result = await _service.CreateAsync(producto);
        
        // Assert
        Assert.Multiple(() => {  // Equivalente a assertAll()
            Assert.IsNotNull(result);
            Assert.AreEqual("Test", result.Nombre);
        });
    }
}
```

## Docker y Deployment

### Servicios en Docker
```yaml
version: '3.8'
services:
  postgres:
    image: postgres:15
    environment:
      POSTGRES_DB: tienda
      POSTGRES_USER: admin
      POSTGRES_PASSWORD: password
    ports:
      - "5432:5432"
      
  redis:
    image: redis:7
    ports:
      - "6379:6379"
      
  mongodb:
    image: mongo:7
    ports:
      - "27017:27017"
```

### Dockerfile para la API
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["TiendaApi/TiendaApi.csproj", "TiendaApi/"]
RUN dotnet restore "TiendaApi/TiendaApi.csproj"
COPY . .
WORKDIR "/src/TiendaApi"
RUN dotnet build "TiendaApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TiendaApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TiendaApi.dll"]
```

## Recursos Adicionales

### 📖 Documentación Oficial
- [ASP.NET Core Documentation](https://docs.microsoft.com/en-us/aspnet/core/)
- [Entity Framework Core Documentation](https://docs.microsoft.com/en-us/ef/core/)
- [NUnit Documentation](https://docs.nunit.org/)

### 🔗 Repositorios Relacionados
- [Proyecto Spring Boot Original](https://github.com/joseluisgs/DesarrolloWebEntornosServidor-02-Proyecto-SpringBoot)
- [Ejemplos Java](https://github.com/joseluisgs/DesarrolloWebEntornosServidor-02-2025-2026)

### 💡 Mejores Prácticas Aplicadas
- Arquitectura por capas clara y comprensible
- Código completamente comentado en español
- Tests exhaustivos con alta cobertura
- Documentación pedagógica detallada
- Comparativas Java vs C# en cada concepto

## Autor

Codificado con :sparkling_heart: por [José Luis González Sánchez](https://twitter.com/JoseLuisGS_)

[![Twitter](https://img.shields.io/twitter/follow/JoseLuisGS_?style=social)](https://twitter.com/JoseLuisGS_)
[![GitHub](https://img.shields.io/github/followers/joseluisgs?style=social)](https://github.com/joseluisgs)
[![GitHub](https://img.shields.io/github/stars/joseluisgs?style=social)](https://github.com/joseluisgs)

### Contacto

<p>
  Cualquier cosa que necesites házmelo saber por si puedo ayudarte 💬.
</p>
<p>
 <a href="https://joseluisgs.dev" target="_blank">
        <img src="https://joseluisgs.github.io/img/favicon.png" 
    height="30">
    </a>  &nbsp;&nbsp;
    <a href="https://github.com/joseluisgs" target="_blank">
        <img src="https://distreau.com/github.svg" 
    height="30">
    </a> &nbsp;&nbsp;
        <a href="https://twitter.com/JoseLuisGS_" target="_blank">
        <img src="https://i.imgur.com/U4Uiaef.png" 
    height="30">
    </a> &nbsp;&nbsp;
    <a href="https://www.linkedin.com/in/joseluisgonsan" target="_blank">
        <img src="https://upload.wikimedia.org/wikipedia/commons/thumb/c/ca/LinkedIn_logo_initials.png/768px-LinkedIn_logo_initials.png" 
    height="30">
    </a>  &nbsp;&nbsp;
    <a href="https://g.dev/joseluisgs" target="_blank">
        <img loading="lazy" src="https://googlediscovery.com/wp-content/uploads/google-developers.png" 
    height="30">
    </a>  &nbsp;&nbsp;
<a href="https://www.youtube.com/@joseluisgs" target="_blank">
        <img loading="lazy" src="https://upload.wikimedia.org/wikipedia/commons/e/ef/Youtube_logo.png" 
    height="30">
    </a>  
</p>

## Licencia de uso

Este repositorio y todo su contenido está licenciado bajo licencia **Creative Commons**, si desea saber más, vea
la [LICENSE](https://joseluisgs.dev/docs/license/). Por favor si compartes, usas o modificas este proyecto cita a su
autor, y usa las mismas condiciones para su uso docente, formativo o educativo y no comercial.

<a rel="license" href="http://creativecommons.org/licenses/by-nc-sa/4.0/"><img alt="Licencia de Creative Commons" style="border-width:0" src="https://i.creativecommons.org/l/by-nc-sa/4.0/88x31.png" /></a><br /><span xmlns:dct="http://purl.org/dc/terms/" property="dct:title">
JoseLuisGS</span>
by <a xmlns:cc="http://creativecommons.org/ns#" href="https://joseluisgs.dev/" property="cc:attributionName" rel="cc:attributionURL">
José Luis González Sánchez</a> is licensed under
a <a rel="license" href="http://creativecommons.org/licenses/by-nc-sa/4.0/">Creative Commons
Reconocimiento-NoComercial-CompartirIgual 4.0 Internacional License</a>.<br />Creado a partir de la obra
en <a xmlns:dct="http://purl.org/dc/terms/" href="https://github.com/joseluisgs" rel="dct:source">https://github.com/joseluisgs</a>.