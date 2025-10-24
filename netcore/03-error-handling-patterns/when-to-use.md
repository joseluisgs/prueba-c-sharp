# 🎯 When to Use - Guía Práctica de Decisión

## Introducción

La pregunta más importante: **¿Cuándo usar Exceptions y cuándo usar Result Pattern?**

Esta guía te ayudará a tomar la decisión correcta basándose en criterios objetivos.

## 🧭 Árbol de Decisión

```
¿Estás migrando código Java/Spring Boot directamente?
    ├─ SÍ → Usa EXCEPTIONS inicialmente
    └─ NO → Continúa evaluando ↓

¿El error representa una situación verdaderamente excepcional?
    ├─ SÍ → Usa EXCEPTIONS
    └─ NO → Continúa evaluando ↓

¿Es crítica la performance (>10,000 req/s)?
    ├─ SÍ → Usa RESULT PATTERN
    └─ NO → Continúa evaluando ↓

¿La lógica de negocio tiene múltiples paths de error?
    ├─ SÍ → Usa RESULT PATTERN
    └─ NO → Continúa evaluando ↓

¿Tu equipo tiene experiencia en programación funcional?
    ├─ SÍ → Usa RESULT PATTERN
    └─ NO → Usa EXCEPTIONS (por ahora)
```

## 📊 Matriz de Decisión

| Criterio | Exceptions | Result Pattern | Peso |
|----------|-----------|---------------|------|
| **Performance** | ❌ Lento | ✅ Rápido | ⭐⭐⭐ |
| **Type Safety** | ❌ Runtime | ✅ Compile-time | ⭐⭐⭐ |
| **Familiaridad Team** | ✅ Alta | ❌ Baja | ⭐⭐⭐ |
| **Testing** | ⚠️ Complejo | ✅ Simple | ⭐⭐ |
| **Legibilidad** | ✅ Familiar | ⚠️ Requiere aprendizaje | ⭐⭐ |
| **Mantenibilidad** | ⚠️ Hidden flow | ✅ Explicit flow | ⭐⭐ |
| **Composability** | ❌ Try/catch nesting | ✅ Bind/Map chaining | ⭐ |

## 🎯 Casos de Uso Específicos

### 1. CRUD Simple - REST API

**Escenario:**
- API REST básica
- Operaciones CRUD standard
- Sin lógica de negocio compleja
- Errores típicos: NotFound, Validation, Conflict

**Recomendación:** ✅ **EXCEPTIONS**

**Razones:**
- Simple y directo
- Familiar para el equipo
- GlobalExceptionHandler centraliza todo
- Overhead de performance aceptable en CRUD

**Ejemplo:**
```csharp
// Service
public async Task<CategoriaDto> FindByIdAsync(long id)
{
    var categoria = await _repository.FindByIdAsync(id);
    
    if (categoria == null)
        throw new NotFoundException($"Categoría {id} no encontrada");
    
    return _mapper.Map<CategoriaDto>(categoria);
}

// Controller
[HttpGet("{id}")]
public async Task<IActionResult> GetById(long id)
{
    try
    {
        var categoria = await _service.FindByIdAsync(id);
        return Ok(categoria);
    }
    catch (NotFoundException ex)
    {
        return NotFound(new { message = ex.Message });
    }
}
```

### 2. E-Commerce Order Processing

**Escenario:**
- Proceso complejo con múltiples pasos
- Validación de stock
- Verificación de pago
- Aplicación de descuentos
- Reserva de inventario
- Múltiples puntos de fallo posibles

**Recomendación:** ✅ **RESULT PATTERN**

**Razones:**
- Lógica de negocio compleja
- Múltiples paths de error
- Necesitas composición de operaciones
- Errores son parte del flujo normal

**Ejemplo:**
```csharp
public async Task<Result<OrderDto, AppError>> CreateOrderAsync(CreateOrderDto dto)
{
    return await ValidateOrderDto(dto)
        .Bind(async validDto => await CheckProductStock(validDto))
        .Bind(async validDto => await CheckUserCredit(validDto))
        .Bind(async validDto => await ApplyDiscounts(validDto))
        .Bind(async validDto => await ReserveInventory(validDto))
        .Bind(async validDto => await ProcessPayment(validDto))
        .Tap(order => _logger.LogInformation("Order created: {OrderId}", order.Id))
        .Tap(order => _eventBus.Publish(new OrderCreatedEvent(order)))
        .Map(order => _mapper.Map<OrderDto>(order));
}
```

### 3. High-Throughput API (Microservices)

**Escenario:**
- Microservicio con alta carga
- 50,000+ requests/segundo
- Performance es crítica
- Latencia debe ser < 10ms

**Recomendación:** ✅ **RESULT PATTERN**

**Razones:**
- Exceptions tienen overhead significativo
- Stack unwinding es costoso
- Necesitas máxima performance
- Errores frecuentes (ej: rate limiting, cache miss)

**Benchmark:**
```
Exception throwing: ~100,000 ns
Result returning:      ~500 ns
Diferencia: 200x más rápido
```

### 4. External API Integration

**Escenario:**
- Integración con API externa
- Errores de red frecuentes
- Timeouts
- Rate limiting
- Respuestas no esperadas

**Recomendación:** ✅ **RESULT PATTERN**

**Razones:**
- Errores son parte del flujo normal
- Retries y circuit breaker patterns
- Composición de operaciones
- No son situaciones "excepcionales"

**Ejemplo:**
```csharp
public async Task<Result<WeatherData, AppError>> GetWeatherAsync(string city)
{
    return await CheckRateLimit()
        .Bind(async _ => await FetchFromCache(city))
        .Bind(async cached => cached != null 
            ? Result<WeatherData, AppError>.Success(cached)
            : await FetchFromApi(city))
        .Tap(data => _cache.Set(city, data, TimeSpan.FromMinutes(30)))
        .MapError(error => error.Type == ErrorType.Timeout 
            ? AppError.Business("Weather service temporarily unavailable")
            : error);
}
```

### 5. File Processing Pipeline

**Escenario:**
- Procesamiento de archivos
- Validación de formato
- Transformación de datos
- Guardado en base de datos
- Generación de reportes

**Recomendación:** ✅ **RESULT PATTERN**

**Razones:**
- Pipeline de operaciones
- Cada paso puede fallar
- Railway Oriented Programming perfecto
- Fácil rollback

**Ejemplo:**
```csharp
public async Task<Result<ProcessingReport, AppError>> ProcessFileAsync(
    IFormFile file)
{
    return await ValidateFileFormat(file)
        .Bind(async validFile => await ParseFileContent(validFile))
        .Bind(async data => await ValidateData(data))
        .Bind(async validData => await TransformData(validData))
        .Bind(async transformed => await SaveToDatabase(transformed))
        .Tap(saved => _logger.LogInformation("File processed successfully"))
        .Map(saved => GenerateReport(saved));
}
```

### 6. Background Job Processing

**Escenario:**
- Procesamiento en background
- Envío de emails
- Generación de reportes
- Limpieza de datos
- Errores no afectan al usuario inmediatamente

**Recomendación:** ⚖️ **DEPENDE**

**EXCEPTIONS si:**
- Job simple
- Errores raros
- Quieres propagación automática a sistema de logging

**RESULT PATTERN si:**
- Job complejo con múltiples pasos
- Necesitas retry logic sofisticada
- Quieres reportar progreso detallado

### 7. Authentication/Authorization

**Escenario:**
- Login de usuarios
- Validación de tokens
- Verificación de permisos

**Recomendación:** ⚖️ **DEPENDE**

**EXCEPTIONS para:**
- Middleware de autenticación global
- Errores de configuración
- Token malformado o inválido

**RESULT PATTERN para:**
- Login endpoint (credenciales incorrectas no son "excepcionales")
- Password reset flow
- Permission checks en business logic

**Ejemplo híbrido:**
```csharp
// Middleware - usa exceptions
public class JwtMiddleware
{
    public async Task InvokeAsync(HttpContext context)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault();
        
        if (string.IsNullOrEmpty(token))
            throw new UnauthorizedException("Token missing");
        
        // Validar token...
    }
}

// Login endpoint - usa Result Pattern
public async Task<Result<LoginResponse, AppError>> LoginAsync(LoginDto dto)
{
    var user = await _userRepository.FindByEmailAsync(dto.Email);
    
    if (user == null)
        return Result<LoginResponse, AppError>.Failure(
            AppError.Validation("Invalid credentials"));
    
    if (!_passwordHasher.Verify(dto.Password, user.PasswordHash))
        return Result<LoginResponse, AppError>.Failure(
            AppError.Validation("Invalid credentials"));
    
    var token = _tokenGenerator.Generate(user);
    return Result<LoginResponse, AppError>.Success(
        new LoginResponse(token, user));
}
```

## 🏢 Consideraciones por Tipo de Equipo

### Startup / Prototipo Rápido

**Usa:** ✅ **EXCEPTIONS**
- Velocity > Performance
- Team pequeño, comunicación fácil
- Prioridad: lanzar rápido

### Enterprise / Aplicación Madura

**Usa:** ✅ **RESULT PATTERN**
- Performance importante
- Codebase grande
- Múltiples equipos
- Necesitas predictibilidad

### Team Junior / Sin experiencia FP

**Usa:** ✅ **EXCEPTIONS**
- Familiar y fácil de entender
- Menor curva de aprendizaje
- Documentación abundante

### Team Senior / Con experiencia FP

**Usa:** ✅ **RESULT PATTERN**
- Aprovecha conocimientos FP
- Código más mantenible
- Type safety apreciado

## 🔄 Estrategia de Migración Gradual

### Fase 1: Start with Exceptions (Mes 1-2)
```csharp
// Todo el código usa exceptions
public class CategoriaService { /* usa exceptions */ }
public class ProductoService { /* usa exceptions */ }
public class UserService { /* usa exceptions */ }
```

### Fase 2: Introduce Result Pattern (Mes 3-4)
```csharp
// Nuevos servicios usan Result Pattern
public class OrderService { /* usa Result Pattern */ }
public class PaymentService { /* usa Result Pattern */ }

// Servicios existentes mantienen exceptions
public class CategoriaService { /* sigue usando exceptions */ }
```

### Fase 3: Hybrid Approach (Mes 5-6)
```csharp
// Critical paths usan Result Pattern
public class OrderService { /* Result Pattern */ }
public class PaymentService { /* Result Pattern */ }
public class InventoryService { /* Result Pattern */ }

// Simple CRUD mantiene exceptions
public class CategoriaService { /* exceptions */ }
public class TagService { /* exceptions */ }
```

### Fase 4: Evaluar y Decidir (Mes 7+)
- Medir performance de ambos enfoques
- Feedback del equipo
- Análisis de bugs/issues
- Decisión: migrar todo o mantener híbrido

## 📈 Métricas para Evaluar

### Performance Metrics

```csharp
// Benchmark ambos enfoques
[Benchmark]
public async Task<CategoriaDto> FindById_Exception()
{
    return await _serviceException.FindByIdAsync(1);
}

[Benchmark]
public async Task<Result<CategoriaDto, AppError>> FindById_Result()
{
    return await _serviceResult.FindByIdAsync(1);
}
```

**Métricas a medir:**
- Latencia p50, p95, p99
- Throughput (requests/segundo)
- Memory allocation
- CPU usage

### Team Productivity Metrics

- **Time to implement new feature**: ¿Cuál es más rápido?
- **Bugs in production**: ¿Cuál tiene menos errores?
- **Code review time**: ¿Cuál es más fácil de revisar?
- **Onboarding time**: ¿Cuál es más fácil de aprender?

### Code Quality Metrics

- **Test coverage**: ¿Cuál tiene mejor coverage?
- **Cyclomatic complexity**: ¿Cuál es más simple?
- **Lines of code**: ¿Cuál es más conciso?
- **Maintainability index**: ¿Cuál es más mantenible?

## 🎓 Recomendaciones Finales

### Para Estudiantes

1. **Aprende AMBOS enfoques**
2. Comienza con **Exceptions** (familiar)
3. Practica **Result Pattern** en proyectos personales
4. Compara ambos en el mismo proyecto (como TiendaApi)

### Para Equipos Profesionales

1. **Evalúa tu contexto específico**
2. **No hay solución única** - usa ambos donde corresponda
3. **Mide performance** si es crítico
4. **Prioriza legibilidad** sobre cleverness
5. **Documenta tu decisión** para el equipo

### Regla de Oro

> **"Use exceptions for exceptional situations, Results for expected error cases"**

Si el error es parte del flujo normal de negocio → **Result Pattern**
Si el error es verdaderamente excepcional → **Exceptions**

## 📚 Recursos Adicionales

- [Choosing Between Exceptions and Results](https://enterprisecraftsmanship.com/posts/exceptions-for-flow-control/)
- [Performance of Exceptions](https://mattwarren.org/2016/12/20/Why-Exceptions-should-be-Exceptional/)
- [Railway Oriented Programming in Practice](https://medium.com/@dogancancoskun/railway-oriented-programming-in-c-17f12f8d53a3)

## 🔄 Casos Especiales: Enfoque Híbrido

En la práctica real, **la mejor solución es a menudo HÍBRIDA**:

```csharp
public class OrderService
{
    // Business logic: Result Pattern
    public async Task<Result<OrderDto, AppError>> CreateOrderAsync(CreateOrderDto dto)
    {
        return await ValidateOrder(dto)
            .Bind(async v => await ProcessOrder(v));
    }
    
    // Infrastructure errors: Exceptions
    private async Task<Order> SaveToDatabaseAsync(Order order)
    {
        // Si hay error de BD (connection, timeout, etc.), deja que lance exception
        return await _repository.SaveAsync(order);
        // GlobalExceptionHandler lo manejará
    }
}
```

**Guideline:**
- **Business logic errors** → Result Pattern
- **Infrastructure errors** → Exceptions (DB, Network, File I/O)
- **Configuration errors** → Exceptions
- **Programmer errors** → Exceptions (ArgumentException, etc.)

---

**Anterior:** [← Result Pattern](./result-pattern.md) | **Siguiente:** [Global Handlers →](./global-handlers.md)
