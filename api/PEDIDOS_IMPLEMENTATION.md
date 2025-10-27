# Pedidos Implementation - Complete Documentation

## Overview

This implementation adds full Pedidos (orders) functionality to TiendaApi, demonstrating a hybrid database architecture where:
- **PostgreSQL** stores product inventory (Productos) with stock management
- **MongoDB** stores order documents (Pedidos) with embedded items
- **Redis** provides caching layer for improved performance
- **WebSockets** enable real-time notifications
- **Email service** handles asynchronous notifications

## Architecture

### Hybrid Database Pattern
```
User creates order → 
  1. Verify products exist (PostgreSQL)
  2. Check stock availability (PostgreSQL)
  3. Reserve stock by decrementing (PostgreSQL)
  4. Save order document (MongoDB)
  5. On failure: Compensate by restoring stock (PostgreSQL)
  6. Cache order (Redis)
  7. Send notifications (WebSocket + Email)
```

### Result Pattern Usage
Following the project's functional programming approach, the Pedidos implementation uses Result Pattern for explicit error handling:
- No exceptions for business logic errors
- Type-safe error handling
- Explicit error types: NotFound, Validation, BusinessRule, etc.

## Files Added

### Repositories
- **`api/TiendaApi/Repositories/IPedidosRepository.cs`**
  - Interface defining MongoDB operations for Pedidos
  - Methods: FindAllAsync, FindByUserIdAsync, FindByIdAsync, SaveAsync, UpdateAsync

- **`api/TiendaApi/Repositories/PedidosRepository.cs`**
  - MongoDB implementation using MongoDB.Driver
  - Connects using ConnectionStrings:MongoDB or MongoDbSettings from appsettings.json
  - Collection name: "pedidos" (configurable via MongoDbSettings:PedidosCollection)

### Services
- **`api/TiendaApi/Services/Pedidos/IPedidosService.cs`**
  - Service interface with Result<T, AppError> return types
  - Methods: FindAllAsync, FindByUserIdAsync, FindByIdAsync, CreateAsync, UpdateEstadoAsync

- **`api/TiendaApi/Services/Pedidos/PedidosService.cs`**
  - Business logic implementation
  - **Stock Management**: Verifies product existence and availability
  - **Stock Reservation**: Decrements stock in PostgreSQL
  - **Compensation Logic**: Restores stock if MongoDB save fails
  - **Caching**: Uses Redis for pedidos with 5-minute TTL
  - **Notifications**: Fire-and-forget WebSocket and Email notifications
  - **Validation**: Validates items, quantities, and estado values

### Controllers
- **`api/TiendaApi/Controllers/PedidosController.cs`**
  - REST API endpoints for Pedidos
  - Uses JWT authentication with Claims-based authorization
  - Endpoints:
    - `POST /api/pedidos` - Create order (authenticated)
    - `GET /api/pedidos/me` - Get user's orders (authenticated)
    - `GET /api/pedidos/{id}` - Get order by ID (authenticated, user can only see own orders)
    - `PUT /api/pedidos/{id}/estado` - Update order status (admin only)

### WebSocket Handler
- **`api/TiendaApi/WebSockets/PedidoWebSocketHandler.cs`**
  - Manages WebSocket connections for real-time pedido notifications
  - Broadcasts two types of events:
    - `PEDIDO_CREATED` - When a new order is created
    - `PEDIDO_ESTADO_UPDATED` - When order status changes
  - Uses ConcurrentDictionary for thread-safe connection management

### Documentation
- **`api/TiendaApi/Pedidos.http`**
  - HTTP test file with example requests for all endpoints
  - Includes error case testing scenarios
  - WebSocket connection instructions

## Files Modified

### Program.cs
Added dependency injection registrations:
```csharp
// Repositories
builder.Services.AddScoped<IPedidosRepository, PedidosRepository>();

// Services
builder.Services.AddScoped<IPedidosService, PedidosService>();

// WebSocket Handler
builder.Services.AddSingleton<PedidoWebSocketHandler>();

// WebSocket endpoint mapping
app.Map("/ws/v1/pedidos", async context => { ... });
```

## Environment Configuration

### Required Environment Variables / appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=tienda;Username=admin;Password=admin123",
    "MongoDB": "mongodb://admin:admin123@localhost:27017/tienda?authSource=admin",
    "Redis": "localhost:6379"
  },
  "MongoDbSettings": {
    "ConnectionString": "mongodb://admin:admin123@localhost:27017/tienda?authSource=admin",
    "DatabaseName": "tienda",
    "PedidosCollection": "pedidos"
  },
  "Jwt": {
    "Key": "TiendaApi-Super-Secret-Key-For-JWT-Token-Generation-MinLength32Characters!",
    "Issuer": "TiendaApi",
    "Audience": "TiendaApi",
    "ExpireMinutes": "60"
  },
  "Smtp": {
    "Host": "smtp.example.com",
    "Port": "587",
    "Username": "noreply@tienda.com",
    "Password": "your-smtp-password",
    "FromEmail": "noreply@tienda.com",
    "FromName": "TiendaApi Notifications",
    "AdminEmail": "admin@tienda.com"
  }
}
```

### Docker Compose Setup
Use the provided docker-compose.yml to start required services:
```bash
docker-compose up -d
```

This starts:
- PostgreSQL (port 5432)
- MongoDB (port 27017)
- Redis (port 6379)

## Testing the Implementation

### 1. Start Services
```bash
cd api
docker-compose up -d
dotnet run --project TiendaApi
```

### 2. Register and Login
```bash
# Register
curl -X POST http://localhost:5031/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "username": "testuser",
    "email": "test@example.com",
    "password": "Test123!",
    "fullName": "Test User"
  }'

# Login to get JWT token
curl -X POST http://localhost:5031/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "username": "testuser",
    "password": "Test123!"
  }'
```

Save the token from login response.

### 3. Get Available Products
```bash
curl http://localhost:5031/api/productos
```

Note the product IDs and stock levels.

### 4. Create a Pedido
```bash
curl -X POST http://localhost:5031/api/pedidos \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -d '{
    "items": [
      {
        "productoId": 1,
        "cantidad": 2
      },
      {
        "productoId": 2,
        "cantidad": 1
      }
    ]
  }'
```

### 5. Get My Pedidos
```bash
curl http://localhost:5031/api/pedidos/me \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

### 6. Get Pedido by ID
```bash
curl http://localhost:5031/api/pedidos/YOUR_PEDIDO_ID \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

### 7. Update Pedido Estado (Admin Only)
First, login as admin or create an admin user. Then:
```bash
curl -X PUT http://localhost:5031/api/pedidos/YOUR_PEDIDO_ID/estado \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer ADMIN_JWT_TOKEN" \
  -d '{
    "estado": "PROCESANDO"
  }'
```

Valid estados: `PENDIENTE`, `PROCESANDO`, `ENVIADO`, `ENTREGADO`, `CANCELADO`

### 8. WebSocket Testing
Connect to WebSocket endpoint: `ws://localhost:5031/ws/v1/pedidos`

You can use tools like:
- Browser JavaScript: `new WebSocket('ws://localhost:5031/ws/v1/pedidos')`
- wscat: `wscat -c ws://localhost:5031/ws/v1/pedidos`
- Postman WebSocket feature

When events occur, you'll receive notifications like:
```json
{
  "type": "PEDIDO_CREATED",
  "pedidoId": "507f1f77bcf86cd799439011",
  "userId": 1,
  "estado": "PENDIENTE",
  "data": { ... },
  "timestamp": "2024-10-27T19:30:00.000Z"
}
```

## Business Logic Features

### Stock Management
1. **Verification**: Checks if products exist and have sufficient stock
2. **Reservation**: Atomically decrements stock in PostgreSQL
3. **Compensation**: If MongoDB save fails, stock is automatically restored

### Caching Strategy
- **Read-through**: Checks Redis cache before querying MongoDB
- **Write-through**: Caches newly created/updated pedidos
- **Invalidation**: Removes stale cache entries on updates
- **TTL**: 5-minute cache expiration

### Notifications (Fire-and-Forget)
All side effects are non-blocking:
- **WebSocket**: Real-time notifications to connected clients
- **Email**: Queued via background service (EmailBackgroundService)
- **Failures**: Logged but don't affect API response

Email notifications sent:
1. **On Create**: Admin receives new order notification with item details
2. **On Estado Update**: Admin receives status change notification

### Authorization Rules
- **Create Pedido**: Any authenticated user
- **Get My Pedidos**: User sees only their own orders
- **Get Pedido by ID**: User can only see own orders, admins can see all
- **Update Estado**: Admin role required

## Error Handling

### Validation Errors (400)
- Empty items list
- Invalid quantities (≤ 0)
- Invalid estado values

### Not Found Errors (404)
- Product doesn't exist
- Pedido doesn't exist

### Business Rule Errors (400)
- Insufficient stock

### Authorization Errors
- 401: Missing or invalid JWT token
- 403: User attempting to access another user's pedido or non-admin updating estado

### Internal Errors (500)
- Database connection failures
- Stock reservation failures
- MongoDB save failures (after compensation attempt)

## Design Patterns Used

### Result Pattern
Explicit error handling without exceptions:
```csharp
var resultado = await _service.CreateAsync(userId, dto);
if (resultado.IsSuccess)
{
    return CreatedAtAction(...);
}
return error.Type switch { ... };
```

### Repository Pattern
Separation of data access from business logic:
- `IPedidosRepository` - Interface
- `PedidosRepository` - MongoDB implementation

### Dependency Injection
All dependencies injected via constructor:
- Repositories, Services, Mappers, Logger, Cache, Email, Configuration

### Fire-and-Forget Pattern
Non-blocking side effects:
```csharp
_ = Task.Run(async () => {
    try { await SendNotification(); }
    catch (Exception ex) { _logger.LogWarning(...); }
});
```

### Saga Pattern (Compensation)
Distributed transaction with compensation:
1. Reserve stock (PostgreSQL)
2. Save pedido (MongoDB)
3. On failure: Restore stock (compensation)

## Performance Considerations

### Caching
- Redis caching reduces MongoDB load
- Cache-aside pattern for read operations
- Cache invalidation on writes

### Async/Await
All I/O operations are asynchronous

### Fire-and-Forget
Side effects don't block main request flow

### Connection Pooling
MongoDB.Driver and EF Core handle connection pooling automatically

## Security Considerations

### Authentication
- JWT token required for all operations
- Token contains user ID (ClaimTypes.NameIdentifier)
- Token contains role (ClaimTypes.Role)

### Authorization
- Role-based access control (RBAC)
- Resource ownership validation (users can only see own orders)

### Input Validation
- Quantities validated (> 0)
- Product IDs validated (must exist)
- Estado values validated (only allowed values)

## Monitoring and Logging

All operations are logged with appropriate levels:
- **Information**: Successful operations
- **Warning**: Non-critical failures (cache, notifications)
- **Error**: Critical failures (database operations)
- **Debug**: Detailed operation flow

Log examples:
```
[Information] Creating pedido for user: 1 with 2 items
[Information] Stock reserved for producto: 1, new stock: 48
[Information] Pedido created: 507f1f77bcf86cd799439011 for user: 1, total: 150.00
[Warning] Failed to send WebSocket notification for pedido creation
```

## Future Enhancements (Not in this PR)

- [ ] GraphQL resolver for Pedidos queries
- [ ] Integration tests with Testcontainers
- [ ] Pedido cancellation with stock restoration
- [ ] Pagination for listing pedidos
- [ ] Search and filtering by estado, date range
- [ ] Metrics and health checks
- [ ] Rate limiting for order creation

## Related Documentation

- See `api/TiendaApi/Pedidos.http` for HTTP request examples
- See `api/README.md` for general API documentation
- See `docker-compose.yml` for infrastructure setup

## Comparison with Spring Boot

This implementation demonstrates C# equivalents of Spring Boot patterns:

| Spring Boot | C# / ASP.NET Core |
|-------------|-------------------|
| `@Repository` | `IPedidosRepository` + DI registration |
| `@Service` | `IPedidosService` + DI registration |
| `@RestController` | `[ApiController]` + `[Route]` |
| `@Autowired` | Constructor injection |
| `@PreAuthorize("hasRole('ADMIN')")` | `[Authorize(Roles = "ADMIN")]` |
| Spring Data MongoDB | MongoDB.Driver |
| Spring Data JPA | Entity Framework Core |
| Spring Cache | Redis with `ICacheService` |
| `@Async` | Fire-and-forget with `Task.Run` |
| `Either<L,R>` (Vavr) | `Result<TValue, TError>` |

## Summary

This implementation provides a complete, production-ready Pedidos system with:
- ✅ Hybrid database architecture (PostgreSQL + MongoDB)
- ✅ Stock management with compensation
- ✅ Result Pattern for type-safe error handling
- ✅ Redis caching for performance
- ✅ Real-time WebSocket notifications
- ✅ Asynchronous email notifications
- ✅ JWT authentication and role-based authorization
- ✅ Comprehensive logging and error handling
- ✅ Following existing project patterns and conventions
