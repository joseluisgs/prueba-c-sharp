# 📦 Ejemplo 07: Productos Reactivo - RxJava vs System.Reactive

## 🎯 Objetivo

Demostrar la **programación reactiva** migrando de **RxJava** (Java) a **System.Reactive (Rx.NET)** en C#, mostrando equivalencias entre ambas bibliotecas y patrones comunes.

## 🔄 Comparativa: RxJava vs Rx.NET

### Conceptos Fundamentales

| Concepto | RxJava (Java) | Rx.NET (C#) | Notas |
|----------|---------------|-------------|-------|
| Observable | `Observable<T>` | `IObservable<T>` | Interface en .NET |
| Observer | `Observer<T>` | `IObserver<T>` | Interface en .NET |
| Subscription | `Disposable` | `IDisposable` | Patrón .NET estándar |
| Hot Observable | `Subject<T>` | `Subject<T>` | Mismo nombre |
| Cold Observable | `Observable.create()` | `Observable.Create()` | Pascal case |

### Creación de Observables

**Java (RxJava):**
```java
// Desde colección
Observable<Producto> productos = Observable.fromIterable(getProductos());

// Observable personalizado
Observable<Producto> custom = Observable.create(emitter -> {
    emitter.onNext(producto);
    emitter.onComplete();
});
```

**C# (Rx.NET):**
```csharp
// Desde colección
IObservable<Producto> productos = productos.ToObservable();

// Observable personalizado
IObservable<Producto> custom = Observable.Create<Producto>(observer =>
{
    observer.OnNext(producto);
    observer.OnCompleted();
    return Disposable.Empty;
});
```

### Operadores Comunes

| Operación | RxJava | Rx.NET | Ejemplo |
|-----------|--------|--------|---------|
| Filtrar | `filter()` | `Where()` | `Where(p => p.Precio > 100)` |
| Transformar | `map()` | `Select()` | `Select(p => p.WithDescuento(0.1m))` |
| Aplanar | `flatMap()` | `SelectMany()` | `SelectMany(p => GetDetalles(p))` |
| Agrupar | `groupBy()` | `GroupBy()` | `GroupBy(p => p.Categoria)` |
| Combinar | `merge()` | `Merge()` | `stream1.Merge(stream2)` |
| Zip | `zip()` | `Zip()` | `productos.Zip(precios)` |
| Buffer | `buffer()` | `Buffer()` | `Buffer(TimeSpan.FromSeconds(1))` |
| Distinct | `distinct()` | `Distinct()` | `Distinct(p => p.Precio)` |
| Take | `take()` | `Take()` | `Take(10)` |
| Skip | `skip()` | `Skip()` | `Skip(5)` |

### Subjects (Hot Observables)

**Java (RxJava):**
```java
// PublishSubject - no guarda estado
PublishSubject<Producto> subject = PublishSubject.create();
subject.subscribe(p -> System.out.println(p));
subject.onNext(producto);

// BehaviorSubject - guarda último valor
BehaviorSubject<Producto> behavior = BehaviorSubject.createDefault(initialValue);

// ReplaySubject - replay últimos N eventos
ReplaySubject<Producto> replay = ReplaySubject.createWithSize(3);

// AsyncSubject - solo emite último valor al completar
AsyncSubject<Producto> async = AsyncSubject.create();
```

**C# (Rx.NET):**
```csharp
// Subject - equivalente a PublishSubject
var subject = new Subject<Producto>();
subject.Subscribe(p => Console.WriteLine(p));
subject.OnNext(producto);

// BehaviorSubject - guarda último valor
var behavior = new BehaviorSubject<Producto>(initialValue);

// ReplaySubject - replay últimos N eventos
var replay = new ReplaySubject<Producto>(bufferSize: 3);

// AsyncSubject - solo emite último valor al completar
var async = new AsyncSubject<Producto>();
```

### Schedulers (Control de Threading)

**Java (RxJava):**
```java
Observable<Producto> productos = getProductos()
    .subscribeOn(Schedulers.io())           // Ejecutar en background
    .observeOn(AndroidSchedulers.mainThread()); // Observar en UI thread

// Tipos de Schedulers
Schedulers.io()           // Para I/O operations
Schedulers.computation()  // Para CPU-intensive work
Schedulers.newThread()    // Nuevo thread dedicado
Schedulers.immediate()    // Thread actual sin delay
Schedulers.trampoline()   // Queue en thread actual
```

**C# (Rx.NET):**
```csharp
IObservable<Producto> productos = GetProductos()
    .SubscribeOn(TaskPoolScheduler.Default)  // Ejecutar en background
    .ObserveOn(DispatcherScheduler.Current); // Observar en UI thread

// Tipos de Schedulers
TaskPoolScheduler.Default    // Similar a Schedulers.io()
ThreadPoolScheduler.Instance // Para trabajo paralelo
NewThreadScheduler.Default   // Nuevo thread dedicado
ImmediateScheduler.Instance  // Thread actual sin delay
Scheduler.CurrentThread      // Queue en thread actual (trampoline)
```

### Error Handling

**Java (RxJava):**
```java
Observable<Producto> productos = getProductos()
    .onErrorReturn(error -> productoDefault)  // Fallback
    .onErrorResumeNext(Observable.empty())    // Stream alternativo
    .retry(3)                                  // Reintentar N veces
    .timeout(5, TimeUnit.SECONDS);            // Timeout
```

**C# (Rx.NET):**
```csharp
IObservable<Producto> productos = GetProductos()
    .Catch(Observable.Return(productoDefault))  // Fallback
    .Catch(Observable.Empty<Producto>())        // Stream alternativo
    .Retry(3)                                    // Reintentar N veces
    .Timeout(TimeSpan.FromSeconds(5));          // Timeout
```

## 🏗️ Estructura del Proyecto

```
07-ProductosReactivo/
├── ProductosReactive.Console/
│   ├── Models/
│   │   ├── Producto.cs                # Entidad Producto
│   │   └── ProductoDto.cs            # DTO para transferencia
│   ├── Services/
│   │   ├── ProductoObservableService.cs # Observable patterns básicos
│   │   └── ProductoReactiveService.cs   # Operaciones reactivas avanzadas
│   ├── Streams/
│   │   ├── ProductoStream.cs         # Cold observables
│   │   └── ProductoHotStream.cs      # Hot observables (Subjects)
│   ├── Schedulers/
│   │   └── ProductoSchedulers.cs     # Threading y schedulers
│   ├── Program.cs                    # Demos de RxJava vs Rx.NET
│   └── ProductosReactive.Console.csproj
├── ProductosReactive.Tests/          # Tests para reactive streams
│   ├── ProductoObservableServiceTests.cs
│   ├── ProductoReactiveServiceTests.cs
│   └── ProductosReactive.Tests.csproj
└── README.md
```

## 🔧 Tecnologías Utilizadas

### NuGet Packages
```xml
<!-- Programación Reactiva -->
<PackageReference Include="System.Reactive" Version="6.0.0" />
<PackageReference Include="System.Reactive.Linq" Version="6.0.0" />

<!-- Testing -->
<PackageReference Include="Microsoft.Reactive.Testing" Version="6.0.0" />
<PackageReference Include="NUnit" Version="3.14.0" />
<PackageReference Include="FluentAssertions" Version="6.12.0" />
```

## 🚀 Ejecución

### Ejecutar Console App
```bash
cd ProductosReactive.Console
dotnet run
```

### Ejecutar Tests
```bash
cd ProductosReactive.Tests
dotnet test --logger "console;verbosity=detailed"
```

## 📚 Conceptos Demostrados

### 1. **Observable Básico**
```csharp
// Cold Observable - cada subscriber recibe todos los eventos
IObservable<Producto> cold = productos.ToObservable();
cold.Subscribe(p => Console.WriteLine(p)); // Subscriber 1
cold.Subscribe(p => Console.WriteLine(p)); // Subscriber 2 (recibe mismos eventos)
```

### 2. **Hot vs Cold Observables**
```csharp
// Cold Observable
var cold = Observable.Create<int>(observer => {
    observer.OnNext(1);
    observer.OnNext(2);
    observer.OnCompleted();
    return Disposable.Empty;
});

// Hot Observable (Subject)
var hot = new Subject<int>();
hot.Subscribe(x => Console.WriteLine($"Sub1: {x}"));
hot.OnNext(1); // Solo Sub1 recibe
hot.Subscribe(x => Console.WriteLine($"Sub2: {x}"));
hot.OnNext(2); // Ambos reciben
```

### 3. **Operadores de Transformación**
```csharp
productos
    .Where(p => p.Precio > 100)           // Filtrar
    .Select(p => p.WithDescuento(0.1m))   // Transformar
    .GroupBy(p => p.Categoria)            // Agrupar
    .Subscribe(group => {
        Console.WriteLine($"Categoría: {group.Key}");
        group.Subscribe(p => Console.WriteLine($"  - {p.Nombre}"));
    });
```

### 4. **Schedulers y Threading**
```csharp
productos
    .SubscribeOn(TaskPoolScheduler.Default)   // Background thread
    .Select(p => HeavyOperation(p))           // Operación pesada
    .ObserveOn(DispatcherScheduler.Current)   // UI thread
    .Subscribe(p => UpdateUI(p));
```

### 5. **Error Handling**
```csharp
productos
    .Catch<Producto, Exception>(ex => 
    {
        Console.WriteLine($"Error: {ex.Message}");
        return Observable.Return(productoDefault);
    })
    .Retry(3)
    .Timeout(TimeSpan.FromSeconds(5))
    .Subscribe(p => Console.WriteLine(p));
```

### 6. **Backpressure y Buffer**
```csharp
// Buffer por tiempo
productos.Buffer(TimeSpan.FromSeconds(1))
    .Subscribe(batch => Console.WriteLine($"Batch: {batch.Count}"));

// Buffer por cantidad
productos.Buffer(10)
    .Subscribe(batch => ProcessBatch(batch));

// Throttle (rate limiting)
productos.Throttle(TimeSpan.FromMilliseconds(500))
    .Subscribe(p => Console.WriteLine(p));
```

## 🎓 Diferencias Clave Java → C#

### Nomenclatura
- **Java:** `camelCase` para métodos → **C#:** `PascalCase`
- **Java:** `filter()` → **C#:** `Where()`
- **Java:** `map()` → **C#:** `Select()`
- **Java:** `flatMap()` → **C#:** `SelectMany()`

### Tipos
- **Java:** `Observable<T>` (clase) → **C#:** `IObservable<T>` (interface)
- **Java:** `Disposable` (interface) → **C#:** `IDisposable` (interface estándar)

### Schedulers
- **Java:** `Schedulers.io()` → **C#:** `TaskPoolScheduler.Default`
- **Java:** `AndroidSchedulers.mainThread()` → **C#:** `DispatcherScheduler.Current` (WPF/UWP)

### Sintaxis Lambda
- **Java:** `p -> p.getPrecio()` → **C#:** `p => p.Precio`
- **Java:** `System.out.println()` → **C#:** `Console.WriteLine()`

## 🔗 Recursos Adicionales

- [ReactiveX Documentation](http://reactivex.io/)
- [System.Reactive Documentation](https://github.com/dotnet/reactive)
- [Introduction to Rx.NET](http://introtorx.com/)
- [RxJava to Rx.NET Migration Guide](https://github.com/dotnet/reactive/blob/main/Rx.NET/Documentation/IntroToRx/00_Foreword.md)

## 📝 Ejercicios Sugeridos

1. Crear un observable que emita productos con delay variable
2. Implementar un sistema de caché usando BehaviorSubject
3. Crear una pipeline reactiva completa con múltiples transformaciones
4. Implementar retry con backoff exponencial
5. Combinar múltiples streams usando Merge, Zip y CombineLatest

---

**Nota:** Este ejemplo es parte del proyecto educativo de migración Java/Spring Boot → C#/.NET
