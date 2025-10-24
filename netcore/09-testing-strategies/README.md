# 🧪 Testing Strategies

## Introducción

Testing comprehensivo: unit tests, integration tests, y API testing con .NET.

## 📚 Contenido

- **nunit-vs-junit.md** - NUnit vs JUnit comparativa completa
- **mocking-moq.md** - Moq vs Mockito para mocking
- **testcontainers.md** - TestContainers.NET vs TestContainers Java
- **integration-testing.md** - Integration test patterns
- **api-testing.md** - Testing de APIs con WebApplicationFactory

## ✅ Unit Testing Quick Example

### JUnit 5
```java
@Test
void findById_NotFound_ThrowsException() {
    when(repository.findById(999L)).thenReturn(Optional.empty());
    
    assertThrows(NotFoundException.class, () -> {
        service.findById(999L);
    });
}
```

### NUnit
```csharp
[Test]
public void FindByIdAsync_NotFound_ThrowsException()
{
    _repositoryMock.Setup(r => r.FindByIdAsync(999))
        .ReturnsAsync((Producto)null);
    
    Assert.ThrowsAsync<NotFoundException>(async () => 
        await _service.FindByIdAsync(999));
}
```

## 🐳 TestContainers Example

```csharp
[SetUp]
public async Task Setup()
{
    _container = new PostgreSqlBuilder()
        .WithImage("postgres:15")
        .WithDatabase("testdb")
        .WithUsername("test")
        .WithPassword("test")
        .Build();
    
    await _container.StartAsync();
}
```
