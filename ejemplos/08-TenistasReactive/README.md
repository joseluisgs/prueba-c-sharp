# 🎾 Ejemplo 08: Tenistas Reactive - Advanced Rx.NET Patterns

## 🎯 Objetivo

Demostrar **patrones reactivos avanzados** migrando conceptos avanzados de **RxJava** a **System.Reactive (Rx.NET)**, incluyendo Hot/Cold Observables, Subjects, BackPressure y error handling.

## 🔄 Conceptos Avanzados: RxJava vs Rx.NET

### Hot vs Cold Observables

**Cold Observable:**
- Cada suscripción crea una nueva ejecución independiente
- Los subscribers reciben todos los eventos desde el inicio
- Similar a una llamada a función

**Hot Observable:**
- Todos los subscribers comparten la misma ejecución
- Los nuevos subscribers solo reciben eventos futuros
- Similar a un event broadcaster

| Característica | Cold | Hot |
|----------------|------|-----|
| Ejecución | Por subscriber | Compartida |
| Estado | Sin estado | Con estado |
| Eventos pasados | Sí | No (depende del Subject) |
| Uso típico | HTTP calls, DB queries | UI events, WebSockets |

### Subjects en Detalle

| Subject Type | Comportamiento | Caso de Uso |
|-------------|----------------|-------------|
| `Subject<T>` (PublishSubject) | Sin estado, solo eventos futuros | Event bus, notifications |
| `BehaviorSubject<T>` | Guarda último valor | Estado actual, configuración |
| `ReplaySubject<T>` | Replay últimos N eventos | Cache, history |
| `AsyncSubject<T>` | Solo emite último valor al completar | Single value computation |

### BackPressure Strategies

**BackPressure** ocurre cuando el productor emite más rápido de lo que el consumidor puede procesar.

| Estrategia | Operador Rx.NET | Comportamiento |
|-----------|-----------------|----------------|
| Buffer | `Buffer(timespan, count)` | Acumula items en lotes |
| Throttle | `Throttle(timespan)` | Toma último item en ventana |
| Sample | `Sample(timespan)` | Toma item más reciente periódicamente |
| Debounce | `Throttle(timespan)` | Espera silencio antes de emitir |

**Java (RxJava):**
```java
Flowable<Item> flowable = Flowable.create(emitter -> {
    // ... emit items
}, BackpressureStrategy.BUFFER);

// Buffer strategy
flowable.buffer(100, TimeUnit.MILLISECONDS, 10);

// Throttle
flowable.throttleFirst(500, TimeUnit.MILLISECONDS);
```

**C# (Rx.NET):**
```csharp
IObservable<Item> observable = Observable.Create<Item>(observer => {
    // ... emit items
});

// Buffer strategy
observable.Buffer(TimeSpan.FromMilliseconds(100), 10);

// Throttle
observable.Throttle(TimeSpan.FromMilliseconds(500));
```

## 🏗️ Estructura del Proyecto

```
08-TenistasReactive/
├── TenistasReactive.Console/
│   ├── Models/
│   │   └── Tenista.cs                # Reutilizado de ejemplos anteriores
│   ├── Services/
│   │   ├── TenistaReactiveService.cs # Advanced reactive patterns
│   │   └── TenistaSubjectService.cs  # Subject implementations
│   ├── Streams/
│   │   ├── TenistaHotObservable.cs   # Hot observable patterns
│   │   ├── TenistaColdObservable.cs  # Cold observable patterns
│   │   └── TenistaBackPressure.cs    # BackPressure handling
│   ├── ErrorHandling/
│   │   └── ReactiveErrorHandler.cs   # Error handling strategies
│   ├── Program.cs                    # Advanced reactive demos
│   └── TenistasReactive.Console.csproj
├── TenistasReactive.Tests/           # Advanced reactive testing
│   ├── TenistaReactiveTests.cs
│   └── TenistasReactive.Tests.csproj
└── README.md
```

## 🚀 Ejecución

```bash
# Console App
cd TenistasReactive.Console
dotnet run

# Tests
cd TenistasReactive.Tests
dotnet test
```

## 📚 Patrones Demostrados

### 1. Hot Observable con Publish/RefCount

```csharp
// Convertir Cold a Hot
IObservable<Tenista> cold = GetTenistas();
IObservable<Tenista> hot = cold.Publish().RefCount();

// Todos los subscribers comparten la misma ejecución
hot.Subscribe(t => Console.WriteLine($"Sub1: {t}"));
hot.Subscribe(t => Console.WriteLine($"Sub2: {t}"));
```

### 2. Subjects para diferentes escenarios

```csharp
// PublishSubject - eventos en tiempo real
var subject = new Subject<Tenista>();
subject.Subscribe(t => Console.WriteLine(t));
subject.OnNext(tenista);

// BehaviorSubject - estado actual
var behavior = new BehaviorSubject<Tenista>(initialValue);
Console.WriteLine($"Current: {behavior.Value}");

// ReplaySubject - buffer de eventos recientes
var replay = new ReplaySubject<Tenista>(bufferSize: 3);
// Nuevos subscribers reciben últimos 3 eventos

// AsyncSubject - resultado final
var async = new AsyncSubject<Tenista>();
async.OnNext(result1);
async.OnNext(result2);
async.OnCompleted(); // Solo ahora emite result2
```

### 3. BackPressure Handling

```csharp
// Buffer - acumular hasta procesar
observable
    .Buffer(TimeSpan.FromSeconds(1), 100)
    .Subscribe(batch => ProcessBatch(batch));

// Throttle - limitar rate
observable
    .Throttle(TimeSpan.FromMilliseconds(500))
    .Subscribe(item => Process(item));

// Sample - muestreo periódico
observable
    .Sample(TimeSpan.FromSeconds(1))
    .Subscribe(item => UpdateUI(item));
```

### 4. Error Handling Avanzado

```csharp
// Retry con exponential backoff
observable
    .Retry(3)
    .Catch(ex => {
        var delay = TimeSpan.FromSeconds(Math.Pow(2, retryCount));
        return Observable.Timer(delay).SelectMany(_ => observable);
    });

// Fallback value
observable
    .Catch(Observable.Return(defaultValue));

// Alternative stream
observable
    .Catch(alternativeObservable);

// Timeout con fallback
observable
    .Timeout(TimeSpan.FromSeconds(5))
    .Catch(Observable.Return(defaultValue));
```

## 🎓 Comparativa RxJava vs Rx.NET

### Conversión Hot/Cold

| RxJava | Rx.NET |
|--------|--------|
| `observable.share()` | `observable.Publish().RefCount()` |
| `observable.replay()` | `observable.Replay()` |
| `observable.cache()` | `observable.Replay().RefCount()` |

### Subjects

| RxJava | Rx.NET |
|--------|--------|
| `PublishSubject.create()` | `new Subject<T>()` |
| `BehaviorSubject.createDefault(value)` | `new BehaviorSubject<T>(value)` |
| `ReplaySubject.createWithSize(n)` | `new ReplaySubject<T>(n)` |
| `AsyncSubject.create()` | `new AsyncSubject<T>()` |

### BackPressure

| RxJava | Rx.NET |
|--------|--------|
| `Flowable.create(BackpressureStrategy.BUFFER)` | `Observable + Buffer()` |
| `throttleFirst()` | `Throttle()` |
| `sample()` | `Sample()` |
| `debounce()` | `Throttle()` |

## 🔗 Recursos

- [ReactiveX - Hot vs Cold](http://reactivex.io/documentation/observable.html)
- [Intro to Rx - Subjects](http://introtorx.com/Content/v1.0.10621.0/02_KeyTypes.html#Subject)
- [Backpressure Strategies](https://github.com/dotnet/reactive/blob/main/Rx.NET/Documentation/IntroToRx/09_SideEffects.md)

---

**Nota:** Este ejemplo es parte del proyecto educativo de migración Java/Spring Boot → C#/.NET
