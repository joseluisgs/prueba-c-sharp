# 🚀 Migración Java/Spring Boot a C#/.NET - Proyecto Educativo

## Tabla de Contenidos
- [Descripción del Proyecto](#descripción-del-proyecto)
- [Objetivos Educativos](#objetivos-educativos)
- [Estructura del Repositorio](#estructura-del-repositorio)
- [Ejemplos Disponibles](#ejemplos-disponibles)
- [Tecnologías Migradas](#tecnologías-migradas)
- [Instalación y Configuración](#instalación-y-configuración)
- [Orden de Aprendizaje](#orden-de-aprendizaje)

## Descripción del Proyecto

Este repositorio contiene una **migración completa y educativa** de conceptos Java/Spring Boot a C#/.NET, diseñado específicamente para estudiantes que vienen del ecosistema Java y quieren aprender .NET de manera gradual y comprensible.

### Proyecto Base
- **Fuente:** Ejemplos del curso [DesarrolloWebEntornosServidor-02-2025-2026](https://github.com/joseluisgs/DesarrolloWebEntornosServidor-02-2025-2026)
- **API Original:** [Tienda Spring Boot](https://github.com/joseluisgs/DesarrolloWebEntornosServidor-02-Proyecto-SpringBoot)
- **Enfoque:** Migración práctica manteniendo la estructura conceptual familiar

## Estructura del Repositorio

```
📦 joseluisgs/prueba-c-sharp/
│
├── 📁 ejemplos/                     # 10 ejemplos educativos basados en curso Java
│   ├── 01-AccesoAdoNet/            # ADO.NET manual (base fundamental)
│   ├── 02-AccesoEntityFramework/   # Entity Framework Core (ORM)
│   ├── 03-TenistasSync/            # Programación síncrona migrada
│   ├── 04-TenistasResult/          # Result Pattern y manejo errores
│   ├── 05-DesayunoAsincrono/       # Programación asíncrona básica
│   ├── 06-TenistasAsync/           # Programación asíncrona avanzada
│   ├── 07-ProductosReactivo/       # Programación reactiva básica
│   ├── 08-TenistasReactive/        # Programación reactiva avanzada
│   ├── 09-Retrofit/                # HTTP clients declarativos
│   └── 10-DockerAndTestContainers/ # Testing con contenedores
│
├── 📁 api/                         # API completa migrada
│   ├── TiendaApi.sln              # Solution principal
│   ├── TiendaApi/                 # Proyecto Web API
│   ├── TiendaApi.Tests/           # Tests completos
│   └── docker-compose.yml         # PostgreSQL + Redis + MongoDB
│
├── 📁 csharp/                      # Documentación Java → C#
│   ├── 01-fundamentos/            # Sintaxis y conceptos básicos
│   ├── 02-oop/                    # Programación orientada a objetos
│   ├── 03-collections/            # Colecciones y estructuras datos
│   ├── 04-streams-linq/           # Stream API vs LINQ
│   └── ...                        # Más documentación
│
└── 📁 netcore/                     # Documentación Spring Boot → ASP.NET Core
    ├── 01-fundamentos/            # Comparativa frameworks
    ├── 02-web-api/                # Spring MVC vs ASP.NET Core
    ├── 03-data-access/            # JPA → Entity Framework Core
    └── ...                        # Más documentación
```

## Ejemplos Disponibles

| # | Ejemplo | Java Original | C# Migrado | Dificultad | Descripción |
|---|---------|---------------|------------|------------|-------------|
| **01** | AccesoAdoNet | JDBC | ADO.NET | 🟡 Intermedio | Acceso manual a BD con control total |
| **02** | AccesoEntityFramework | Spring Data JPA | Entity Framework Core | 🟢 Básico | ORM completo con migraciones |
| **03** | TenistasSync | Programación síncrona | Task.Run + Sync | 🟢 Básico | Operaciones síncronas y colecciones |
| **04** | TenistasResult | Optional/Either | CSharpFunctionalExtensions | 🟡 Intermedio | Railway Oriented Programming |
| **05** | DesayunoAsincrono | CompletableFuture | Task/async-await | 🟢 Básico | Programación asíncrona conceptual |
| **06** | TenistasAsync | Async + Streams | IAsyncEnumerable | 🟡 Intermedio | Async streams y BackgroundService |
| **07** | ProductosReactivo | RxJava básico | System.Reactive | 🟡 Intermedio | Observables y patrones reactivos |
| **08** | TenistasReactive | RxJava avanzado | Reactive Extensions | 🔴 Avanzado | Hot/Cold observables, Schedulers |
| **09** | Retrofit | Retrofit | Refit | 🟢 Básico | HTTP clients declarativos |
| **10** | TestContainers | TestContainers Java | TestContainers.NET | 🟡 Intermedio | Integration testing con Docker |

## Tecnologías Migradas

### Framework Principal
| Java/Spring Boot | C#/.NET | Uso |
|------------------|---------|-----|
| Spring Boot 3.5.6 | ASP.NET Core 8 | Framework web principal |
| Spring MVC | ASP.NET Core Web API | Controllers y routing |
| Spring Validation | FluentValidation | Validación de modelos |
| Jackson | System.Text.Json | Serialización JSON |

### Acceso a Datos
| Java | C# | Escenario |
|------|-----|-----------|
| Spring Data JPA | Entity Framework Core | ORM principal (recomendado) |
| JDBC | ADO.NET | Acceso directo (control total) |
| H2 (in-memory) | InMemory EF Provider | Testing y desarrollo |
| PostgreSQL | Npgsql.EntityFrameworkCore.PostgreSQL | Base de datos principal |

### Testing
| Java | C# | Razón de Elección |
|------|-----|------------------|
| JUnit 5 | NUnit 3.14 | Sintaxis más familiar para Java developers |
| Mockito | Moq | Patrón de mocking similar |
| TestContainers | TestContainers.NET | Integration testing con Docker |

## Instalación y Configuración

### Prerrequisitos
- **.NET 8 SDK** - [Descargar](https://dotnet.microsoft.com/download/dotnet/8.0)
- **Visual Studio 2022** o **VS Code** con extensión C#
- **Docker Desktop** (para ejemplos con bases de datos)
- **Git** para clonar el repositorio

### Clonar el Repositorio
```bash
git clone https://github.com/joseluisgs/prueba-c-sharp.git
cd prueba-c-sharp
```

### Ejecutar un Ejemplo
```bash
# Ejemplo: Entity Framework
cd ejemplos/02-AccesoEntityFramework/AccesoEF.Console
dotnet restore
dotnet run
```

### Ejecutar Tests
```bash
# Tests de un ejemplo específico
cd ejemplos/02-AccesoEntityFramework/AccesoEF.Tests
dotnet test --logger "console;verbosity=detailed"
```

## Orden de Aprendizaje

### 🎯 Ruta Recomendada para Estudiantes Java

#### **Nivel 1: Acceso a Datos (Fundamental)**
1. **[02-AccesoEntityFramework](./ejemplos/02-AccesoEntityFramework/)** - Entender ORM (como JPA)
2. **[01-AccesoAdoNet](./ejemplos/01-AccesoAdoNet/)** - Entender acceso manual (como JDBC)

#### **Nivel 2: Programación Básica**
3. **[03-TenistasSync](./ejemplos/03-TenistasSync/)** - Programación síncrona y colecciones
4. **[04-TenistasResult](./ejemplos/04-TenistasResult/)** - Manejo funcional de errores
5. **[05-DesayunoAsincrono](./ejemplos/05-DesayunoAsincrono/)** - Async/await básico

#### **Nivel 3: Programación Avanzada**
6. **[06-TenistasAsync](./ejemplos/06-TenistasAsync/)** - Async streams avanzado
7. **[09-Retrofit](./ejemplos/09-Retrofit/)** - HTTP clients (Refit)
8. **[07-ProductosReactivo](./ejemplos/07-ProductosReactivo/)** - Reactive básico

#### **Nivel 4: Conceptos Expertos**
9. **[08-TenistasReactive](./ejemplos/08-TenistasReactive/)** - Reactive avanzado
10. **[10-TestContainers](./ejemplos/10-DockerAndTestContainers/)** - Testing con Docker

---

¡Este proyecto te guiará paso a paso en la migración de Java/Spring Boot a C#/.NET! 🚀

## Autor

Codificado con :sparkling_heart: por [José Luis González Sánchez](https://twitter.com/JoseLuisGS_)

[![Twitter](https://img.shields.io/twitter/follow/JoseLuisGS_?style=social)](https://twitter.com/JoseLuisGS_)
[![GitHub](https://img.shields.io/github/followers/joseluisgs?style=social)](https://github.com/joseluisgs)

### Contacto

<p>
  Cualquier cosa que necesites házmelo saber por si puedo ayudarte 💬.
</p>
<p>
 <a href="https://joseluisgs.dev" target="_blank">
        <img src="https://joseluisgs.github.io/img/favicon.png" 
    height="30">
    </a>  &nbsp;&nbsp;
    <a href="https://github.com/joseluisgs" target="_blank">
        <img src="https://distreau.com/github.svg" 
    height="30">
    </a> &nbsp;&nbsp;
        <a href="https://twitter.com/JoseLuisGS_" target="_blank">
        <img src="https://i.imgur.com/U4Uiaef.png" 
    height="30">
    </a> &nbsp;&nbsp;
    <a href="https://www.linkedin.com/in/joseluisgonsan" target="_blank">
        <img src="https://upload.wikimedia.org/wikipedia/commons/thumb/c/ca/LinkedIn_logo_initials.png/768px-LinkedIn_logo_initials.png" 
    height="30">
    </a>  &nbsp;&nbsp;
    <a href="https://g.dev/joseluisgs" target="_blank">
        <img loading="lazy" src="https://googlediscovery.com/wp-content/uploads/google-developers.png" 
    height="30">
    </a>  &nbsp;&nbsp;
<a href="https://www.youtube.com/@joseluisgs" target="_blank">
        <img loading="lazy" src="https://upload.wikimedia.org/wikipedia/commons/e/ef/Youtube_logo.png" 
    height="30">
    </a>  
</p>

## Licencia de uso

Este repositorio y todo su contenido está licenciado bajo licencia **Creative Commons**, si desea saber más, vea
la [LICENSE](https://joseluisgs.dev/docs/license/). Por favor si compartes, usas o modificas este proyecto cita a su
autor, y usa las mismas condiciones para su uso docente, formativo o educativo y no comercial.

<a rel="license" href="http://creativecommons.org/licenses/by-nc-sa/4.0/"><img alt="Licencia de Creative Commons" style="border-width:0" src="https://i.creativecommons.org/l/by-nc-sa/4.0/88x31.png" /></a><br /><span xmlns:dct="http://purl.org/dc/terms/" property="dct:title">
JoseLuisGS</span>
by <a xmlns:cc="http://creativecommons.org/ns#" href="https://joseluisgs.dev/" property="cc:attributionName" rel="cc:attributionURL">
José Luis González Sánchez</a> is licensed under
a <a rel="license" href="http://creativecommons.org/licenses/by-nc-sa/4.0/">Creative Commons
Reconocimiento-NoComercial-CompartirIgual 4.0 Internacional License</a>.<br />Creado a partir de la obra
en <a xmlns:dct="http://purl.org/dc/terms/" href="https://github.com/joseluisgs" rel="dct:source">https://github.com/joseluisgs</a>.

**¿Quieres que te siga proporcionando el resto del contenido (los ejemplos 01 y 02 completos) o prefieres subir primero este README corregido?**

Las herramientas de GitHub no están funcionando, pero puedo darte todo el código completo para que lo subas manualmente. Es más rápido y eficiente.
