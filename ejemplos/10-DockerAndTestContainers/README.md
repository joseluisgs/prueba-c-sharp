# 🐳 Ejemplo 10: Docker & TestContainers - Integration Testing

## 🎯 Objetivo

Demostrar el uso de **TestContainers** para pruebas de integración automatizadas con **Docker**, equivalente a TestContainers en Java, permitiendo testing realista con bases de datos y servicios reales.

## 🔄 Comparativa: TestContainers Java vs TestContainers.NET

### Configuración Básica

**Java (TestContainers):**
```java
@Testcontainers
public class DatabaseTest {
    @Container
    static PostgreSQLContainer<?> postgres = new PostgreSQLContainer<>("postgres:15-alpine")
        .withDatabaseName("testdb")
        .withUsername("test")
        .withPassword("test");

    @Test
    void testConnection() {
        String jdbcUrl = postgres.getJdbcUrl();
        // Use connection...
    }
}
```

**C# (TestContainers.NET):**
```csharp
[TestFixture]
public class DatabaseTest
{
    private PostgreSqlContainer _postgres;

    [OneTimeSetUp]
    public async Task Setup()
    {
        _postgres = new PostgreSqlBuilder()
            .WithImage("postgres:15-alpine")
            .WithDatabase("testdb")
            .WithUsername("test")
            .WithPassword("test")
            .Build();

        await _postgres.StartAsync();
    }

    [Test]
    public async Task TestConnection()
    {
        var connectionString = _postgres.GetConnectionString();
        // Use connection...
    }

    [OneTimeTearDown]
    public async Task TearDown()
    {
        await _postgres.DisposeAsync();
    }
}
```

## 🏗️ Estructura del Proyecto

```
10-DockerAndTestContainers/
├── TestContainersDemo.Console/
│   ├── Program.cs                          # Demo application
│   └── TestContainersDemo.Console.csproj
├── TestContainersDemo.Tests/              # Integration tests
│   ├── PostgreSqlContainerTests.cs        # PostgreSQL integration tests
│   └── TestContainersDemo.Tests.csproj
└── README.md                              # Advanced TestContainers patterns
```

## 🔧 Tecnologías Utilizadas

### NuGet Packages
```xml
<!-- TestContainers -->
<PackageReference Include="Testcontainers" Version="3.6.0" />
<PackageReference Include="Testcontainers.PostgreSql" Version="3.6.0" />
<PackageReference Include="Testcontainers.Redis" Version="3.6.0" />
<PackageReference Include="Testcontainers.MsSql" Version="3.6.0" />
<PackageReference Include="Testcontainers.MongoDb" Version="3.6.0" />

<!-- Database Drivers -->
<PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="8.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.0.0" />
```

## 🚀 Ejecución

```bash
# Ejecutar tests (requiere Docker instalado)
cd TestContainersDemo.Tests
dotnet test

# Console App
cd TestContainersDemo.Console
dotnet run
```

## 📚 Patrones TestContainers

### 1. PostgreSQL Container

**Java:**
```java
PostgreSQLContainer postgres = new PostgreSQLContainer("postgres:15")
    .withDatabaseName("testdb")
    .withUsername("test")
    .withPassword("test")
    .withInitScript("init.sql");

postgres.start();
String jdbcUrl = postgres.getJdbcUrl();
```

**C#:**
```csharp
var postgres = new PostgreSqlBuilder()
    .WithImage("postgres:15")
    .WithDatabase("testdb")
    .WithUsername("test")
    .WithPassword("test")
    .WithResourceMapping("init.sql", "/docker-entrypoint-initdb.d/init.sql")
    .Build();

await postgres.StartAsync();
var connectionString = postgres.GetConnectionString();
```

### 2. Redis Container

**Java:**
```java
GenericContainer redis = new GenericContainer("redis:7-alpine")
    .withExposedPorts(6379);

redis.start();
String host = redis.getHost();
Integer port = redis.getMappedPort(6379);
```

**C#:**
```csharp
var redis = new RedisBuilder()
    .WithImage("redis:7-alpine")
    .Build();

await redis.StartAsync();
var connectionString = redis.GetConnectionString();
```

### 3. Multiple Containers with Network

**Java:**
```java
Network network = Network.newNetwork();

PostgreSQLContainer postgres = new PostgreSQLContainer("postgres:15")
    .withNetwork(network)
    .withNetworkAliases("postgres");

GenericContainer app = new GenericContainer("myapp:latest")
    .withNetwork(network)
    .withEnv("DB_HOST", "postgres");
```

**C#:**
```csharp
var network = new NetworkBuilder().Build();

var postgres = new PostgreSqlBuilder()
    .WithNetwork(network)
    .WithNetworkAliases("postgres")
    .Build();

var app = new ContainerBuilder()
    .WithImage("myapp:latest")
    .WithNetwork(network)
    .WithEnvironment("DB_HOST", "postgres")
    .Build();

await network.CreateAsync();
await postgres.StartAsync();
await app.StartAsync();
```

### 4. Wait Strategies

**Java:**
```java
GenericContainer container = new GenericContainer("myapp:latest")
    .waitingFor(Wait.forHttp("/health")
        .forStatusCode(200)
        .withStartupTimeout(Duration.ofMinutes(2)));
```

**C#:**
```csharp
var container = new ContainerBuilder()
    .WithImage("myapp:latest")
    .WithWaitStrategy(Wait.ForUnixContainer()
        .UntilHttpRequestIsSucceeded(request => request
            .ForPath("/health")
            .ForStatusCode(HttpStatusCode.OK)))
    .WithStartupTimeout(TimeSpan.FromMinutes(2))
    .Build();
```

### 5. Custom Container Configuration

**Java:**
```java
GenericContainer container = new GenericContainer("myimage:latest")
    .withExposedPorts(8080, 9090)
    .withEnv("ENV_VAR", "value")
    .withCommand("custom", "command")
    .withCopyFileToContainer(
        MountableFile.forClasspathResource("config.json"),
        "/app/config.json");
```

**C#:**
```csharp
var container = new ContainerBuilder()
    .WithImage("myimage:latest")
    .WithPortBinding(8080, 8080)
    .WithPortBinding(9090, 9090)
    .WithEnvironment("ENV_VAR", "value")
    .WithCommand("custom", "command")
    .WithResourceMapping("config.json", "/app/config.json")
    .Build();
```

## 🎓 Patrones Avanzados

### Fixture Compartido entre Tests

```csharp
[SetUpFixture]
public class GlobalTestFixture
{
    public static PostgreSqlContainer PostgresContainer { get; private set; }

    [OneTimeSetUp]
    public async Task GlobalSetup()
    {
        PostgresContainer = new PostgreSqlBuilder()
            .WithImage("postgres:15-alpine")
            .WithDatabase("testdb")
            .Build();

        await PostgresContainer.StartAsync();
    }

    [OneTimeTearDown]
    public async Task GlobalTearDown()
    {
        await PostgresContainer.DisposeAsync();
    }
}

[TestFixture]
public class MyTests
{
    [Test]
    public async Task Test1()
    {
        var connString = GlobalTestFixture.PostgresContainer.GetConnectionString();
        // Use connection...
    }
}
```

### Multi-Container Setup

```csharp
public class MultiContainerFixture : IAsyncDisposable
{
    private readonly INetwork _network;
    public PostgreSqlContainer PostgresContainer { get; }
    public RedisContainer RedisContainer { get; }

    public MultiContainerFixture()
    {
        _network = new NetworkBuilder().Build();

        PostgresContainer = new PostgreSqlBuilder()
            .WithNetwork(_network)
            .WithNetworkAliases("postgres")
            .Build();

        RedisContainer = new RedisBuilder()
            .WithNetwork(_network)
            .WithNetworkAliases("redis")
            .Build();
    }

    public async Task InitializeAsync()
    {
        await _network.CreateAsync();
        await PostgresContainer.StartAsync();
        await RedisContainer.StartAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await PostgresContainer.DisposeAsync();
        await RedisContainer.DisposeAsync();
        await _network.DeleteAsync();
    }
}
```

### Database Migrations con EF Core

```csharp
[OneTimeSetUp]
public async Task Setup()
{
    _postgres = new PostgreSqlBuilder().Build();
    await _postgres.StartAsync();

    var options = new DbContextOptionsBuilder<MyDbContext>()
        .UseNpgsql(_postgres.GetConnectionString())
        .Options;

    using var context = new MyDbContext(options);
    await context.Database.MigrateAsync(); // Run EF Core migrations
}
```

## 🔗 Containers Disponibles

| Service | Java Class | C# Builder | Image |
|---------|-----------|------------|-------|
| PostgreSQL | `PostgreSQLContainer` | `PostgreSqlBuilder` | `postgres` |
| MySQL | `MySQLContainer` | `MySqlBuilder` | `mysql` |
| SQL Server | `MSSQLServerContainer` | `MsSqlBuilder` | `mcr.microsoft.com/mssql/server` |
| MongoDB | `MongoDBContainer` | `MongoDbBuilder` | `mongo` |
| Redis | `GenericContainer` | `RedisBuilder` | `redis` |
| Elasticsearch | `ElasticsearchContainer` | `ElasticsearchBuilder` | `elasticsearch` |
| RabbitMQ | `RabbitMQContainer` | `RabbitMqBuilder` | `rabbitmq` |
| Kafka | `KafkaContainer` | `KafkaBuilder` | `confluentinc/cp-kafka` |

## 🎯 Ventajas de TestContainers

1. **Testing Realista**: Tests contra bases de datos y servicios reales, no mocks
2. **Aislamiento**: Cada test puede tener su propia instancia limpia
3. **CI/CD Friendly**: Funciona en cualquier entorno con Docker
4. **Reproducibilidad**: Mismo comportamiento en desarrollo y CI
5. **Cleanup Automático**: Los containers se eliminan automáticamente
6. **Multi-Database**: Soporta PostgreSQL, MySQL, MongoDB, Redis, etc.

## 🎓 Diferencias Clave Java → C#

### Nomenclatura
- **Java:** `@Container` → **C#:** `[OneTimeSetUp]` + campo de instancia
- **Java:** `start()` → **C#:** `await StartAsync()`
- **Java:** `getJdbcUrl()` → **C#:** `GetConnectionString()`

### Lifecycle
- **Java:** Annotations (`@Testcontainers`, `@Container`)
- **C#:** NUnit setup methods (`[OneTimeSetUp]`, `[OneTimeTearDown]`)

### Async
- **Java:** Synchronous API
- **C#:** Async/await nativo (`StartAsync()`, `DisposeAsync()`)

## 📝 Best Practices

1. **Reuse Containers**: Use `[OneTimeSetUp]` para compartir containers entre tests
2. **Cleanup**: Siempre use `DisposeAsync()` en `[OneTimeTearDown]`
3. **Wait Strategies**: Configure wait strategies apropiadas para evitar flaky tests
4. **Resource Limits**: Configure memory y CPU limits para CI environments
5. **Image Tags**: Use tags específicos (`postgres:15`) en lugar de `latest`

## 🔗 Recursos Adicionales

- [TestContainers.NET Documentation](https://dotnet.testcontainers.org/)
- [TestContainers Java Documentation](https://www.testcontainers.org/)
- [Docker Documentation](https://docs.docker.com/)
- [Integration Testing Best Practices](https://martinfowler.com/articles/practical-test-pyramid.html)

---

**Nota:** Este ejemplo es parte del proyecto educativo de migración Java/Spring Boot → C#/.NET

**Requisito:** Docker debe estar instalado y ejecutándose para usar TestContainers
