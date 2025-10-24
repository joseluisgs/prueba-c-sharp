# 📚 Guía Completa del Proyecto - Índice de Recursos

## 🎯 Cómo Usar Este Repositorio

Este repositorio está diseñado para **estudiantes y profesores** que quieren migrar de Java/Spring Boot a C#/.NET de forma gradual y comprensible.

### 📖 Para Estudiantes Java

Si vienes de Java y quieres aprender C#/.NET, sigue esta ruta de aprendizaje:

#### **Nivel 1: Fundamentos de C#** (1-2 semanas)
1. **[Fundamentos de C#](./csharp/01-fundamentos/)** - Sintaxis básica, tipos, variables
   - Variables y tipos de datos
   - Propiedades vs getters/setters
   - Null safety con operadores `??` y `?.`
   - Estructuras de control

2. **[LINQ vs Stream API](./csharp/04-streams-linq/)** - Consultas funcionales
   - Comparación lado a lado completa
   - Operaciones de filtrado, transformación, agregación
   - Ejemplos prácticos

#### **Nivel 2: Acceso a Datos** (2-3 semanas)
3. **[ADO.NET (JDBC equivalente)](./ejemplos/01-AccesoAdoNet/)**
   - Conexiones manuales a base de datos
   - Operaciones CRUD sin ORM
   - Ideal para entender los fundamentos

4. **[Entity Framework Core (JPA/Hibernate equivalente)](./ejemplos/02-AccesoEntityFramework/)**
   - DbContext vs EntityManager
   - LINQ to Entities
   - Migraciones y configuración

#### **Nivel 3: Web APIs** (3-4 semanas)
5. **[ASP.NET Core Web API](./netcore/02-web-api/)**
   - Controllers vs @RestController
   - Dependency Injection
   - Routing y validación
   - Middleware vs Filters

### 👨‍🏫 Para Profesores

#### Materiales Didácticos Disponibles

1. **Ejemplos Progresivos**
   - Cada ejemplo tiene README completo con explicaciones
   - Código completamente comentado en español
   - Comparativas lado a lado Java vs C#

2. **Documentación de Referencia**
   - `csharp/` - Conceptos del lenguaje C#
   - `netcore/` - Framework ASP.NET Core
   - Tablas de equivalencias en cada sección

3. **Ejercicios Prácticos**
   - Proyectos ejecutables listos para usar
   - Ejemplos incrementales de complejidad

#### Sugerencias de Uso en Clase

**Semana 1-2: Sintaxis Básica**
- Empezar con `csharp/01-fundamentos`
- Comparar código Java vs C# en vivo
- Ejercicio: Convertir clases Java simples a C#

**Semana 3-4: Collections y LINQ**
- Estudiar `csharp/04-streams-linq`
- Comparar Stream API con LINQ
- Ejercicio: Convertir operaciones Stream a LINQ

**Semana 5-7: Acceso a Datos**
- Ejemplo `01-AccesoAdoNet` para fundamentos
- Ejemplo `02-AccesoEntityFramework` para ORM
- Ejercicio: Migrar repositorio JPA a EF Core

**Semana 8-12: Web APIs**
- Tutorial `netcore/02-web-api`
- Crear controllers REST
- Ejercicio: Migrar endpoint Spring Boot a ASP.NET Core

## 📂 Estructura del Repositorio

```
prueba-c-sharp/
│
├── 📁 ejemplos/                      # Ejemplos prácticos ejecutables
│   ├── 01-AccesoAdoNet/              # ADO.NET (JDBC)
│   │   ├── AccesoAdoNet.Console/     # Proyecto ejecutable
│   │   └── README.md                 # Documentación detallada
│   │
│   └── 02-AccesoEntityFramework/     # Entity Framework Core (JPA)
│       ├── AccesoEF.Console/         # Proyecto ejecutable
│       └── README.md                 # Comparación exhaustiva
│
├── 📁 csharp/                        # Documentación Java → C#
│   ├── 01-fundamentos/               # ⭐ Sintaxis básica
│   │   └── README.md                 # Variables, tipos, operadores
│   │
│   └── 04-streams-linq/              # ⭐ LINQ vs Stream API
│       └── README.md                 # Comparación completa
│
├── 📁 netcore/                       # Documentación Spring Boot → ASP.NET Core
│   └── 02-web-api/                   # ⭐ Web APIs
│       └── README.md                 # Controllers, DI, Middleware
│
└── 📁 api/                           # API completa (próximamente)
    └── TiendaApi/                    # Puerto completo de Tienda Spring Boot
```

## 🚀 Inicio Rápido

### Opción 1: Explorar Ejemplos

Si quieres **ver código funcionando inmediatamente**:

```bash
# Clonar el repositorio
git clone https://github.com/joseluisgs/prueba-c-sharp.git
cd prueba-c-sharp

# Ejemplo 1: ADO.NET
cd ejemplos/01-AccesoAdoNet/AccesoAdoNet.Console
dotnet run

# Ejemplo 2: Entity Framework Core
cd ../02-AccesoEntityFramework/AccesoEF.Console
dotnet run
```

### Opción 2: Leer Documentación

Si quieres **entender conceptos primero**:

1. **[Fundamentos de C#](./csharp/01-fundamentos/)** - Empezar aquí
2. **[LINQ vs Streams](./csharp/04-streams-linq/)** - Consultas funcionales
3. **[Web API](./netcore/02-web-api/)** - Spring Boot → ASP.NET Core

### Opción 3: Ruta Completa (Recomendado)

Combinar teoría y práctica:

```
Día 1-3:   Leer csharp/01-fundamentos → Escribir código básico
Día 4-7:   Leer csharp/04-streams-linq → Ejecutar ejemplos LINQ
Día 8-10:  Ejecutar ejemplos/01-AccesoAdoNet → Modificar código
Día 11-14: Ejecutar ejemplos/02-AccesoEntityFramework → Crear queries
Día 15-21: Leer netcore/02-web-api → Crear tu primer controller
```

## 📋 Requisitos Previos

### Conocimientos Necesarios
- ✅ Java básico e intermedio
- ✅ Spring Boot fundamentals
- ✅ Git básico
- ⭐ Opcional: Spring Data JPA, Maven/Gradle

### Software Necesario
- [.NET 8 SDK](https://dotnet.microsoft.com/download) - **Obligatorio**
- [Visual Studio Code](https://code.visualstudio.com/) - Recomendado
- [Docker Desktop](https://www.docker.com/products/docker-desktop) - Para bases de datos
- [PostgreSQL](https://www.postgresql.org/) - Opcional (puede usar Docker)

### Extensiones VS Code Recomendadas
- C# (Microsoft)
- C# Dev Kit (Microsoft)
- NuGet Package Manager
- REST Client

## 🔍 Mapa de Conceptos

### Del Lenguaje (Java → C#)

| Necesito aprender... | Ver documento... |
|---------------------|------------------|
| Sintaxis básica de C# | `csharp/01-fundamentos` |
| Clases y POO | `csharp/02-oop` (próximamente) |
| Collections en C# | `csharp/03-collections` (próximamente) |
| LINQ (Stream API) | `csharp/04-streams-linq` ⭐ |
| Genéricos en C# | `csharp/05-generics` (próximamente) |
| Manejo de excepciones | `csharp/06-exceptions` (próximamente) |
| Atributos (@Annotations) | `csharp/07-annotations-attributes` (próximamente) |

### Del Framework (Spring Boot → ASP.NET Core)

| Necesito aprender... | Ver documento/ejemplo... |
|---------------------|-------------------------|
| ADO.NET (JDBC) | `ejemplos/01-AccesoAdoNet` ⭐ |
| Entity Framework Core (JPA) | `ejemplos/02-AccesoEntityFramework` ⭐ |
| Controllers REST | `netcore/02-web-api` ⭐ |
| Dependency Injection | `netcore/02-web-api` ⭐ |
| Middleware (Filters) | `netcore/02-web-api` ⭐ |
| Validación | `netcore/02-web-api` ⭐ |
| Security (Spring Security) | `netcore/04-security` (próximamente) |
| Testing (JUnit → NUnit) | `netcore/05-testing` (próximamente) |
| Caching (Spring Cache) | `netcore/06-caching` (próximamente) |

## 💡 Consejos de Aprendizaje

### Para Desarrolladores Java

1. **No todo es diferente** - C# y Java son ~80% similares
2. **LINQ es tu amigo** - Más potente que Stream API
3. **Properties > Getters/Setters** - Menos código boilerplate
4. **async/await > CompletableFuture** - Más natural y legible
5. **Entity Framework ≈ JPA** - Conceptos muy similares

### Errores Comunes de Migración

❌ **Error**: Usar `getNombre()` en vez de `Nombre`
✅ **Correcto**: En C# se usan properties: `persona.Nombre`

❌ **Error**: Buscar `@Autowired` en C#
✅ **Correcto**: DI por constructor automática en ASP.NET Core

❌ **Error**: Intentar usar `Optional<T>`
✅ **Correcto**: Usar nullable types: `string?` o `T?`

❌ **Error**: Escribir JPQL en strings
✅ **Correcto**: Usar LINQ con type-safety

## 📊 Comparación Rápida

### Tabla de Equivalencias Principales

| Concepto | Java/Spring Boot | C#/.NET |
|----------|-----------------|---------|
| **Lenguaje** | Java | C# |
| **Framework Web** | Spring Boot | ASP.NET Core |
| **ORM** | JPA/Hibernate | Entity Framework Core |
| **DI** | Spring IoC | Built-in DI |
| **Testing** | JUnit + Mockito | NUnit/xUnit + Moq |
| **Build** | Maven/Gradle | dotnet CLI |
| **Package Manager** | Maven Central | NuGet |
| **API REST** | @RestController | [ApiController] |
| **Queries** | JPQL/Stream API | LINQ |
| **Config** | application.properties | appsettings.json |
| **Async** | CompletableFuture | async/await + Task |

## 🎓 Recursos Adicionales

### Documentación Oficial
- [C# Documentation](https://docs.microsoft.com/en-us/dotnet/csharp/)
- [ASP.NET Core Docs](https://docs.microsoft.com/en-us/aspnet/core/)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)

### Tutoriales Externos
- [Microsoft Learn - C# for Java Developers](https://docs.microsoft.com/en-us/dotnet/csharp/tour-of-csharp/)
- [.NET for Java Developers](https://www.pluralsight.com/paths/c-for-java-developers)

### Comunidad
- [Stack Overflow - C# Tag](https://stackoverflow.com/questions/tagged/c%23)
- [Reddit r/csharp](https://www.reddit.com/r/csharp/)
- [.NET Discord](https://discord.gg/dotnet)

## 🤝 Contribuciones

Este es un proyecto educativo en desarrollo. Las contribuciones son bienvenidas:

1. **Reportar errores** - Issues en GitHub
2. **Sugerir mejoras** - Pull requests
3. **Compartir experiencia** - Discusiones

## 📝 Roadmap

### ✅ Completado
- [x] Estructura base del proyecto
- [x] Ejemplo 01: ADO.NET (JDBC equivalente)
- [x] Ejemplo 02: Entity Framework Core (JPA equivalente)
- [x] Documentación: Fundamentos de C#
- [x] Documentación: LINQ vs Stream API
- [x] Documentación: ASP.NET Core Web API

### 🚧 En Progreso
- [ ] Más ejemplos (03-10)
- [ ] API completa de Tienda
- [ ] Documentación adicional (OOP, Collections, etc.)

### 📅 Planificado
- [ ] Videos tutoriales
- [ ] Ejercicios con soluciones
- [ ] Tests automatizados para ejemplos
- [ ] Docker Compose completo

## 📧 Contacto

**José Luis González Sánchez**

- 🌐 Web: [https://joseluisgs.dev](https://joseluisgs.dev)
- 🐦 Twitter: [@JoseLuisGS_](https://twitter.com/JoseLuisGS_)
- 💼 LinkedIn: [joseluisgonsan](https://www.linkedin.com/in/joseluisgonsan)
- 📧 Email: joseluis.gonzalez@profesor.com

## 📄 Licencia

Este proyecto está licenciado bajo **Creative Commons BY-NC-SA 4.0**

Puedes:
- ✅ Compartir y adaptar el material
- ✅ Usar en contextos educativos

Debes:
- 📌 Dar crédito apropiado
- 📌 Indicar si realizaste cambios
- 📌 Usar la misma licencia

No puedes:
- ❌ Usar comercialmente sin permiso

---

**¡Feliz aprendizaje! 🚀**

> "El mejor momento para aprender algo nuevo fue ayer. El segundo mejor momento es ahora."
