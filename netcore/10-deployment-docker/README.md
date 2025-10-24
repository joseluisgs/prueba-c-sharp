# 🐳 Deployment & Docker

## Introducción

Docker, Docker Compose y deployment de aplicaciones .NET.

## 📚 Contenido

- **docker-compose.md** - Multi-service deployment
- **dockerfile.md** - .NET Dockerfile vs Spring Boot
- **environment-config.md** - Environment-specific configuration
- **ci-cd-patterns.md** - CI/CD patterns con GitHub Actions

## 🐋 Dockerfile Multi-Stage

### Spring Boot
```dockerfile
FROM maven:3.9-eclipse-temurin-17 AS build
WORKDIR /app
COPY pom.xml .
COPY src ./src
RUN mvn clean package -DskipTests

FROM eclipse-temurin:17-jre
COPY --from=build /app/target/*.jar app.jar
ENTRYPOINT ["java", "-jar", "app.jar"]
```

### ASP.NET Core
```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
COPY *.csproj .
RUN dotnet restore
COPY . .
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "TiendaApi.dll"]
```

## 📦 Docker Compose

```yaml
version: '3.8'
services:
  api:
    build: .
    ports:
      - "5000:8080"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ConnectionStrings__DefaultConnection=Host=postgres;Database=tienda
    depends_on:
      - postgres
      - mongodb
      - redis
  
  postgres:
    image: postgres:15
    environment:
      POSTGRES_DB: tienda
      POSTGRES_USER: admin
      POSTGRES_PASSWORD: password
  
  mongodb:
    image: mongo:7
  
  redis:
    image: redis:7
```
