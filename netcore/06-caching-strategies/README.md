# ⚡ Caching Strategies

## Introducción

Estrategias de caching en ASP.NET Core: IMemoryCache, Redis, y patrones de caching.

## 📚 Contenido

- **memory-cache.md** - IMemoryCache vs Spring Cache (@Cacheable)
- **distributed-cache.md** - Redis con StackExchange.Redis vs Spring Data Redis
- **cache-aside.md** - Cache-Aside pattern implementation
- **cache-invalidation.md** - Estrategias de invalidación

## 💾 Memory Cache Quick Example

### Spring Boot
```java
@Cacheable(value = "productos", key = "#id")
public ProductoDto findById(Long id) {
    return repository.findById(id)
        .map(mapper::toDto)
        .orElseThrow();
}
```

### ASP.NET Core
```csharp
public async Task<ProductoDto> FindByIdAsync(long id)
{
    var cacheKey = $"producto:{id}";
    
    if (!_cache.TryGetValue(cacheKey, out ProductoDto? producto))
    {
        producto = await _repository.FindByIdAsync(id);
        _cache.Set(cacheKey, producto, TimeSpan.FromMinutes(10));
    }
    
    return producto!;
}
```
