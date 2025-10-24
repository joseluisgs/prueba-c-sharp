# 🚨 Error Handling Patterns - Overview

## Introducción

El manejo de errores es uno de los aspectos más importantes del desarrollo de aplicaciones. Esta sección explora **dos enfoques fundamentales** para manejar errores en aplicaciones C#/.NET, comparándolos con sus equivalentes en Java/Spring Boot.

## 📚 Dos Enfoques Principales

### 🔴 1. Traditional Exception Handling (Enfoque Tradicional)
El patrón clásico que todos conocemos: `throw` exceptions y capturarlas con `try/catch`.

**Familiar para desarrolladores:**
- Java/Spring Boot con `@ControllerAdvice`
- JavaScript/TypeScript con try/catch
- Python con exception handling
- Cualquier lenguaje orientado a objetos tradicional

### 🟢 2. Result Pattern (Enfoque Funcional Moderno)
Un enfoque funcional que retorna `Result<T, E>` en lugar de lanzar exceptions.

**Inspirado en:**
- Functional languages (F#, Haskell, Rust)
- Java Vavr `Either<L, R>`
- Scala `Either` and `Try`
- Railway Oriented Programming

## 🎯 Comparativa Rápida

| Aspecto | Exceptions | Result Pattern |
|---------|-----------|---------------|
| **Control de flujo** | Oculto (saltos de stack) | Explícito (return values) |
| **Performance** | Overhead significativo | Sin overhead de exceptions |
| **Type Safety** | Runtime | Compile-time |
| **Testabilidad** | Requiere mocking complejo | Fácil de testear |
| **Legibilidad** | Familiar pero oculta flujo | Explícita pero requiere aprendizaje |
| **Curva aprendizaje** | Baja (familiar) | Media (conceptos funcionales) |

## 📂 Contenido de Esta Sección

### 1. [traditional-exceptions.md](./traditional-exceptions.md)
- Global exception handling con IExceptionHandler
- Comparación con Spring Boot @ControllerAdvice
- Custom exceptions creation
- Controller implementation
- Pros y contras del enfoque

### 2. [result-pattern.md](./result-pattern.md)
- Tutorial completo del Result Pattern
- CSharpFunctionalExtensions library
- Railway Oriented Programming
- Bind, Map, Tap operations
- Comparación con Java Either/Vavr

### 3. [global-handlers.md](./global-handlers.md)
- IExceptionHandler vs @ExceptionHandler
- Middleware pipeline en ASP.NET Core
- Exception filters
- ProblemDetails y RFC 7807

### 4. [when-to-use.md](./when-to-use.md)
- Guía práctica de decisión
- Performance implications
- Team considerations
- Casos de uso recomendados
- Ejemplos del mundo real

## 🏗️ Implementación en TiendaApi

En el proyecto **TiendaApi**, implementamos AMBOS enfoques para que puedas compararlos directamente:

### 🔴 Categorías Controller - Exception Handling
```csharp
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

### 🟢 Productos Controller - Result Pattern
```csharp
[HttpGet("{id}")]
public async Task<IActionResult> GetById(long id)
{
    var resultado = await _service.FindByIdAsync(id);
    
    return resultado.Match(
        onSuccess: producto => Ok(producto),
        onFailure: error => error.Type switch
        {
            ErrorType.NotFound => NotFound(new { message = error.Message }),
            _ => StatusCode(500, new { message = error.Message })
        }
    );
}
```

## 🎓 Objetivos de Aprendizaje

Al completar esta sección, serás capaz de:

1. ✅ Implementar exception handling tradicional en ASP.NET Core
2. ✅ Crear y usar GlobalExceptionHandler middleware
3. ✅ Implementar Result Pattern con CSharpFunctionalExtensions
4. ✅ Aplicar Railway Oriented Programming
5. ✅ Decidir cuándo usar cada enfoque
6. ✅ Migrar código Java/Spring Boot a ambos patrones
7. ✅ Escribir tests para ambos enfoques

## 📊 Resultado Esperado del Aprendizaje

**Antes:**
- Solo conoces exception handling tradicional
- No sabes cuándo usar cada enfoque
- Migración directa de Java sin optimizaciones

**Después:**
- Dominas ambos enfoques
- Puedes elegir el mejor patrón para cada situación
- Escribes código C# idiomático y moderno
- Entiendes conceptos de programación funcional

## 🚀 Comenzar

Comienza con [traditional-exceptions.md](./traditional-exceptions.md) si vienes de Java/Spring Boot, o ve directo a [result-pattern.md](./result-pattern.md) si quieres aprender el enfoque moderno inmediatamente.

## 📚 Referencias Adicionales

- [Railway Oriented Programming (F# for Fun and Profit)](https://fsharpforfunandprofit.com/rop/)
- [Functional C# - Enterprise Craftsmanship](https://enterprisecraftsmanship.com/posts/functional-c-handling-failures-input-errors/)
- [CSharpFunctionalExtensions Documentation](https://github.com/vkhorikov/CSharpFunctionalExtensions)
- [ASP.NET Core Error Handling](https://docs.microsoft.com/en-us/aspnet/core/web-api/handle-errors)

---

**Siguiente:** [Traditional Exceptions →](./traditional-exceptions.md)
