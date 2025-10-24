# 🌐 Web API Controllers

## Introducción

Los **Controllers** son el componente principal para crear APIs REST en ASP.NET Core, equivalentes a los `@RestController` de Spring Boot.

## 📚 Contenido de Esta Sección

### 1. [routing.md](./routing.md)
- Attribute routing vs Convention routing
- @RequestMapping vs [Route]
- Path parameters y query strings
- Route constraints

### 2. [model-binding.md](./model-binding.md)
- @RequestBody vs [FromBody]
- @PathVariable vs [FromRoute]
- @RequestParam vs [FromQuery]
- Model binding automático

### 3. [action-results.md](./action-results.md)
- ResponseEntity vs IActionResult
- HTTP status codes
- Content negotiation
- Custom responses

### 4. [validation.md](./validation.md)
- Bean Validation vs FluentValidation
- @Valid vs [ApiController]
- Custom validators
- Error messages

## 🔄 Comparativa Rápida: Controller Básico

### Java Spring Boot

```java
@RestController
@RequestMapping("/api/productos")
@RequiredArgsConstructor
public class ProductoController {
    
    private final ProductoService service;
    
    @GetMapping
    public ResponseEntity<List<ProductoDto>> getAll() {
        List<ProductoDto> productos = service.findAll();
        return ResponseEntity.ok(productos);
    }
    
    @GetMapping("/{id}")
    public ResponseEntity<ProductoDto> getById(@PathVariable Long id) {
        ProductoDto producto = service.findById(id);
        return ResponseEntity.ok(producto);
    }
    
    @PostMapping
    public ResponseEntity<ProductoDto> create(
        @Valid @RequestBody CreateProductoDto dto) {
        
        ProductoDto created = service.save(dto);
        URI location = ServletUriComponentsBuilder
            .fromCurrentRequest()
            .path("/{id}")
            .buildAndExpand(created.getId())
            .toUri();
        return ResponseEntity.created(location).body(created);
    }
    
    @PutMapping("/{id}")
    public ResponseEntity<ProductoDto> update(
        @PathVariable Long id,
        @Valid @RequestBody UpdateProductoDto dto) {
        
        ProductoDto updated = service.update(id, dto);
        return ResponseEntity.ok(updated);
    }
    
    @DeleteMapping("/{id}")
    public ResponseEntity<Void> delete(@PathVariable Long id) {
        service.deleteById(id);
        return ResponseEntity.noContent().build();
    }
}
```

### C# ASP.NET Core

```csharp
[ApiController]
[Route("api/[controller]")]
public class ProductosController : ControllerBase
{
    private readonly ProductoService _service;
    
    public ProductosController(ProductoService service)
    {
        _service = service;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var productos = await _service.FindAllAsync();
        return Ok(productos);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id)
    {
        var producto = await _service.FindByIdAsync(id);
        return Ok(producto);
    }
    
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductoDto dto)
    {
        var created = await _service.SaveAsync(dto);
        return CreatedAtAction(
            nameof(GetById), 
            new { id = created.Id }, 
            created);
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        long id, 
        [FromBody] UpdateProductoDto dto)
    {
        var updated = await _service.UpdateAsync(id, dto);
        return Ok(updated);
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        await _service.DeleteByIdAsync(id);
        return NoContent();
    }
}
```

## 🎯 Diferencias Clave

| Aspecto | Spring Boot | ASP.NET Core |
|---------|-------------|--------------|
| **Annotation** | `@RestController` | `[ApiController]` |
| **Routing** | `@RequestMapping("/api/productos")` | `[Route("api/[controller]")]` |
| **HTTP Methods** | `@GetMapping`, `@PostMapping` | `[HttpGet]`, `[HttpPost]` |
| **Path Variables** | `@PathVariable Long id` | `long id` (inferido) |
| **Request Body** | `@RequestBody` | `[FromBody]` |
| **Validation** | `@Valid` | Automático con `[ApiController]` |
| **Return Type** | `ResponseEntity<T>` | `IActionResult` o `ActionResult<T>` |
| **DI** | `@Autowired` o constructor | Constructor injection (obligatorio) |

## 📝 Action Results Comunes

### Spring Boot ResponseEntity

```java
// 200 OK
return ResponseEntity.ok(data);

// 201 Created
return ResponseEntity.created(location).body(data);

// 204 No Content
return ResponseEntity.noContent().build();

// 400 Bad Request
return ResponseEntity.badRequest().body(errors);

// 404 Not Found
return ResponseEntity.notFound().build();

// 500 Internal Server Error
return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR).body(error);
```

### ASP.NET Core IActionResult

```csharp
// 200 OK
return Ok(data);

// 201 Created
return CreatedAtAction(nameof(GetById), new { id = data.Id }, data);

// 204 No Content
return NoContent();

// 400 Bad Request
return BadRequest(errors);

// 404 Not Found
return NotFound();

// 500 Internal Server Error
return StatusCode(500, error);
```

## 🔧 [ApiController] Attribute

El attribute `[ApiController]` en ASP.NET Core proporciona comportamiento automático:

```csharp
[ApiController]  // Habilita:
[Route("api/[controller]")]
public class ProductosController : ControllerBase
{
    // 1. Model binding automático desde [FromBody]
    [HttpPost]
    public IActionResult Create(CreateProductoDto dto)  // Automático!
    {
        // dto ya viene del body
    }
    
    // 2. Validación automática - retorna 400 si ModelState.IsValid == false
    [HttpPost]
    public IActionResult Create([FromBody] CreateProductoDto dto)
    {
        // No necesitas validar manualmente
        // ASP.NET Core lo hace automáticamente
    }
    
    // 3. Inferencia de binding source
    [HttpGet("{id}")]  // id viene de route
    [HttpGet]          // name viene de query
    public IActionResult Search(long id, string name)
    {
        // id: [FromRoute]
        // name: [FromQuery]
    }
    
    // 4. ProblemDetails automático para errores 400
}
```

**Equivalente en Spring Boot:**
```java
@RestController  // Similar a [ApiController]
@RequestMapping("/api/productos")
@Validated  // Para validación automática
public class ProductoController {
    // Spring también valida automáticamente con @Valid
}
```

## 🚀 Async/Await

**ASP.NET Core usa async/await extensivamente:**

```csharp
[HttpGet]
public async Task<IActionResult> GetAll()
{
    // await libera el thread mientras espera I/O
    var productos = await _service.FindAllAsync();
    return Ok(productos);
}
```

**Equivalente Java:**
```java
// Java no tiene await nativo (hasta Project Loom)
// Pero puedes usar CompletableFuture:
@GetMapping
public CompletableFuture<ResponseEntity<List<ProductoDto>>> getAll() {
    return service.findAllAsync()
        .thenApply(ResponseEntity::ok);
}
```

## 📊 Content Negotiation

**Spring Boot:**
```java
@GetMapping(produces = {MediaType.APPLICATION_JSON_VALUE, MediaType.APPLICATION_XML_VALUE})
public ResponseEntity<ProductoDto> getById(@PathVariable Long id) {
    return ResponseEntity.ok(producto);
}
```

**ASP.NET Core:**
```csharp
[HttpGet("{id}")]
[Produces("application/json", "application/xml")]
public IActionResult GetById(long id)
{
    return Ok(producto);  // Automáticamente serializa según Accept header
}
```

## 🎓 Objetivos de Aprendizaje

Al completar esta sección, serás capaz de:

1. ✅ Crear controllers RESTful en ASP.NET Core
2. ✅ Entender routing y attribute routing
3. ✅ Usar model binding correctamente
4. ✅ Retornar action results apropiados
5. ✅ Implementar validación con FluentValidation
6. ✅ Manejar content negotiation

## 🔗 Referencias

- [ASP.NET Core Controllers](https://docs.microsoft.com/en-us/aspnet/core/web-api/)
- [Routing in ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/routing)
- [Model Binding](https://docs.microsoft.com/en-us/aspnet/core/mvc/models/model-binding)

---

**Siguiente:** [Routing →](./routing.md)
