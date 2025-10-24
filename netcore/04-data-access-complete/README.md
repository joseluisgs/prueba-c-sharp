# 💾 Data Access Complete

## Introducción

Esta sección cubre **todos** los enfoques de acceso a datos en .NET, comparándolos con sus equivalentes en Java/Spring.

## 📚 Contenido de Esta Sección

### 1. [ef-core-vs-jpa.md](./ef-core-vs-jpa.md)
- Entity Framework Core vs JPA/Hibernate
- Code-First vs Database-First
- LINQ vs JPQL
- Migrations vs Flyway
- Performance y optimización

### 2. [adonet-vs-jdbc.md](./adonet-vs-jdbc.md)
- ADO.NET vs JDBC
- Raw SQL queries
- Stored procedures
- DataReader vs ResultSet
- Cuándo usar acceso directo

### 3. [mongodb-integration.md](./mongodb-integration.md)
- MongoDB.Driver vs Spring Data MongoDB
- Document modeling
- Queries y aggregations
- Integration patterns

### 4. [repository-pattern.md](./repository-pattern.md)
- Repository Pattern implementation
- Generic repositories
- Unit of Work
- Specification Pattern

### 5. [migrations.md](./migrations.md)
- EF Core Migrations vs Flyway
- Database versioning
- Seeding data
- Production strategies

## 🎯 Comparativa de Tecnologías

| Tecnología | Java/Spring | .NET/C# | Uso Recomendado |
|-----------|-------------|---------|-----------------|
| **ORM Completo** | JPA/Hibernate | Entity Framework Core | CRUD estándar, relaciones complejas |
| **Micro-ORM** | MyBatis | Dapper | Performance crítica, queries complejos |
| **Raw SQL** | JDBC | ADO.NET | Control total, stored procedures |
| **NoSQL** | Spring Data MongoDB | MongoDB.Driver | Datos no estructurados |

## 🏗️ Entity Framework Core vs JPA/Hibernate

### Configuración de Entidades

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
    
    @Column(precision = 10, scale = 2)
    private BigDecimal precio;
    
    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "categoria_id")
    private Categoria categoria;
    
    @OneToMany(mappedBy = "producto", cascade = CascadeType.ALL)
    private List<ProductoImage> imagenes = new ArrayList<>();
    
    @CreatedDate
    private LocalDateTime createdAt;
    
    @LastModifiedDate
    private LocalDateTime updatedAt;
}
```

**C# EF Core (Fluent API - Recomendado):**
```csharp
public class Producto
{
    public long Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public long CategoriaId { get; set; }
    public Categoria Categoria { get; set; } = null!;
    public ICollection<ProductoImage> Imagenes { get; set; } = new List<ProductoImage>();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

// DbContext con Fluent API
public class TiendaDbContext : DbContext
{
    public DbSet<Producto> Productos => Set<Producto>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Producto>(entity =>
        {
            entity.ToTable("productos");
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Nombre)
                .IsRequired()
                .HasMaxLength(100);
            
            entity.Property(e => e.Precio)
                .HasPrecision(10, 2);
            
            entity.HasOne(e => e.Categoria)
                .WithMany(c => c.Productos)
                .HasForeignKey(e => e.CategoriaId);
            
            entity.HasMany(e => e.Imagenes)
                .WithOne(i => i.Producto)
                .HasForeignKey(i => i.ProductoId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        });
    }
}
```

## 🔍 Queries Comparison

### Java JPA Repository

```java
public interface ProductoRepository extends JpaRepository<Producto, Long> {
    // Query methods
    List<Producto> findByCategoriaId(Long categoriaId);
    List<Producto> findByNombreContainingIgnoreCase(String nombre);
    
    // JPQL
    @Query("SELECT p FROM Producto p WHERE p.precio < :maxPrecio")
    List<Producto> findByPrecioLessThan(@Param("maxPrecio") BigDecimal maxPrecio);
    
    // Native SQL
    @Query(value = "SELECT * FROM productos WHERE stock > 0", nativeQuery = true)
    List<Producto> findInStock();
    
    // Projection
    @Query("SELECT new com.example.dto.ProductoDto(p.id, p.nombre, p.precio) FROM Producto p")
    List<ProductoDto> findAllProjected();
}
```

### C# EF Core Repository

```csharp
public interface IProductoRepository
{
    Task<IEnumerable<Producto>> FindByCategoriaIdAsync(long categoriaId);
    Task<IEnumerable<Producto>> FindByNombreAsync(string nombre);
    Task<IEnumerable<Producto>> FindByPrecioLessThanAsync(decimal maxPrecio);
    Task<IEnumerable<Producto>> FindInStockAsync();
    Task<IEnumerable<ProductoDto>> FindAllProjectedAsync();
}

public class ProductoRepository : IProductoRepository
{
    private readonly TiendaDbContext _context;
    
    public ProductoRepository(TiendaDbContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<Producto>> FindByCategoriaIdAsync(long categoriaId)
    {
        return await _context.Productos
            .Where(p => p.CategoriaId == categoriaId)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<Producto>> FindByNombreAsync(string nombre)
    {
        return await _context.Productos
            .Where(p => p.Nombre.Contains(nombre))
            .ToListAsync();
    }
    
    public async Task<IEnumerable<Producto>> FindByPrecioLessThanAsync(decimal maxPrecio)
    {
        return await _context.Productos
            .Where(p => p.Precio < maxPrecio)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<Producto>> FindInStockAsync()
    {
        // Raw SQL
        return await _context.Productos
            .FromSqlRaw("SELECT * FROM productos WHERE stock > 0")
            .ToListAsync();
    }
    
    public async Task<IEnumerable<ProductoDto>> FindAllProjectedAsync()
    {
        // Projection con Select
        return await _context.Productos
            .Select(p => new ProductoDto
            {
                Id = p.Id,
                Nombre = p.Nombre,
                Precio = p.Precio
            })
            .ToListAsync();
    }
}
```

## 🚀 Advanced LINQ Queries

### Complex Queries

```csharp
// Query con múltiples joins y filtros
var query = await _context.Productos
    .Include(p => p.Categoria)              // JOIN categorias
    .Include(p => p.Imagenes)               // JOIN imagenes
    .Where(p => p.Precio < 1000)            // WHERE precio < 1000
    .Where(p => !p.IsDeleted)               // AND is_deleted = false
    .OrderByDescending(p => p.CreatedAt)    // ORDER BY created_at DESC
    .Skip(20)                               // OFFSET 20
    .Take(10)                               // LIMIT 10
    .ToListAsync();

// GroupBy y Aggregation
var productosPorCategoria = await _context.Productos
    .GroupBy(p => p.CategoriaId)
    .Select(g => new
    {
        CategoriaId = g.Key,
        Count = g.Count(),
        AvgPrice = g.Average(p => p.Precio),
        MaxPrice = g.Max(p => p.Precio)
    })
    .ToListAsync();

// Subqueries
var productosConImagenes = await _context.Productos
    .Where(p => _context.ProductoImages
        .Any(i => i.ProductoId == p.Id))
    .ToListAsync();
```

**Equivalente JPQL:**
```java
// Query con joins
@Query("""
    SELECT p FROM Producto p
    JOIN FETCH p.categoria
    LEFT JOIN FETCH p.imagenes
    WHERE p.precio < :maxPrecio
    AND p.isDeleted = false
    ORDER BY p.createdAt DESC
    """)
List<Producto> findWithDetails(@Param("maxPrecio") BigDecimal maxPrecio);

// GroupBy
@Query("""
    SELECT new com.example.dto.ProductoStats(
        p.categoriaId,
        COUNT(p),
        AVG(p.precio),
        MAX(p.precio)
    )
    FROM Producto p
    GROUP BY p.categoriaId
    """)
List<ProductoStats> getStatsByCategoria();
```

## 🔥 Performance Optimization

### Eager Loading vs Lazy Loading

**Java JPA:**
```java
// Eager loading
@ManyToOne(fetch = FetchType.EAGER)
private Categoria categoria;

// Lazy loading (default)
@ManyToOne(fetch = FetchType.LAZY)
private Categoria categoria;

// Query-time control
@EntityGraph(attributePaths = {"categoria", "imagenes"})
List<Producto> findAll();
```

**C# EF Core:**
```csharp
// Eager loading con Include
var productos = await _context.Productos
    .Include(p => p.Categoria)
    .Include(p => p.Imagenes)
    .ToListAsync();

// Explicit loading
var producto = await _context.Productos.FindAsync(id);
await _context.Entry(producto)
    .Reference(p => p.Categoria)
    .LoadAsync();

// Lazy loading (requiere configuración)
// builder.Services.AddDbContext<TiendaDbContext>(options =>
//     options.UseLazyLoadingProxies());
```

### Query Splitting

**EF Core permite split queries para performance:**

```csharp
// Single query (puede ser lento con múltiples collections)
var productos = await _context.Productos
    .Include(p => p.Categoria)
    .Include(p => p.Imagenes)
    .ToListAsync();

// Split query (múltiples queries, mejor performance)
var productos = await _context.Productos
    .Include(p => p.Categoria)
    .Include(p => p.Imagenes)
    .AsSplitQuery()
    .ToListAsync();
```

## 💾 ADO.NET vs JDBC (Raw SQL)

### Java JDBC

```java
try (Connection conn = dataSource.getConnection();
     PreparedStatement stmt = conn.prepareStatement(
         "SELECT * FROM productos WHERE categoria_id = ?")) {
    
    stmt.setLong(1, categoriaId);
    
    try (ResultSet rs = stmt.executeQuery()) {
        while (rs.next()) {
            Producto p = new Producto();
            p.setId(rs.getLong("id"));
            p.setNombre(rs.getString("nombre"));
            p.setPrecio(rs.getBigDecimal("precio"));
            productos.add(p);
        }
    }
}
```

### C# ADO.NET

```csharp
using var connection = new NpgsqlConnection(connectionString);
await connection.OpenAsync();

using var command = new NpgsqlCommand(
    "SELECT * FROM productos WHERE categoria_id = @categoriaId", 
    connection);
command.Parameters.AddWithValue("@categoriaId", categoriaId);

using var reader = await command.ExecuteReaderAsync();
while (await reader.ReadAsync())
{
    var producto = new Producto
    {
        Id = reader.GetInt64("id"),
        Nombre = reader.GetString("nombre"),
        Precio = reader.GetDecimal("precio")
    };
    productos.Add(producto);
}
```

## 🍃 MongoDB Integration

### Java Spring Data MongoDB

```java
@Document(collection = "pedidos")
public class Pedido {
    @Id
    private String id;
    private Long userId;
    private List<PedidoItem> items;
    private LocalDateTime fechaPedido;
}

public interface PedidoRepository extends MongoRepository<Pedido, String> {
    List<Pedido> findByUserId(Long userId);
    
    @Query("{ 'fechaPedido': { $gte: ?0, $lt: ?1 } }")
    List<Pedido> findByFechaBetween(LocalDateTime start, LocalDateTime end);
}
```

### C# MongoDB.Driver

```csharp
public class Pedido
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;
    
    public long UserId { get; set; }
    public List<PedidoItem> Items { get; set; } = new();
    public DateTime FechaPedido { get; set; }
}

public class PedidoRepository
{
    private readonly IMongoCollection<Pedido> _pedidos;
    
    public PedidoRepository(IMongoDatabase database)
    {
        _pedidos = database.GetCollection<Pedido>("pedidos");
    }
    
    public async Task<List<Pedido>> FindByUserIdAsync(long userId)
    {
        return await _pedidos
            .Find(p => p.UserId == userId)
            .ToListAsync();
    }
    
    public async Task<List<Pedido>> FindByFechaBetweenAsync(
        DateTime start, 
        DateTime end)
    {
        var filter = Builders<Pedido>.Filter.And(
            Builders<Pedido>.Filter.Gte(p => p.FechaPedido, start),
            Builders<Pedido>.Filter.Lt(p => p.FechaPedido, end)
        );
        
        return await _pedidos.Find(filter).ToListAsync();
    }
}
```

## 🎯 Cuándo Usar Cada Tecnología

### Entity Framework Core (ORM)
✅ **Usar cuando:**
- CRUD estándar
- Relaciones complejas entre entidades
- Quieres Code-First approach
- Migrations automáticas
- Prototipado rápido

❌ **Evitar cuando:**
- Performance crítica con queries complejos
- Necesitas control total del SQL
- Stored procedures extensivos

### ADO.NET (Raw SQL)
✅ **Usar cuando:**
- Performance crítica
- Queries SQL muy complejos
- Stored procedures
- Bulk operations
- Reportes complejos

❌ **Evitar cuando:**
- CRUD simple
- Quieres abstracción de BD
- Team sin experiencia SQL

### Dapper (Micro-ORM)
✅ **Usar cuando:**
- Balance entre performance y productividad
- Queries personalizados
- No necesitas change tracking
- Read-heavy operations

## 🔗 Referencias

- [Entity Framework Core Documentation](https://docs.microsoft.com/en-us/ef/core/)
- [ADO.NET Overview](https://docs.microsoft.com/en-us/dotnet/framework/data/adonet/)
- [MongoDB .NET Driver](https://mongodb.github.io/mongo-csharp-driver/)
- [Dapper Documentation](https://github.com/DapperLib/Dapper)

---

**Siguiente:** [EF Core vs JPA →](./ef-core-vs-jpa.md)
