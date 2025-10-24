# 📡 Messaging & Real-Time

## Introducción

WebSockets nativos, background services y email con MailKit.

## 📚 Contenido

- **websockets-native.md** - WebSockets nativos (NO SignalR) vs Spring WebSocket
- **mailkit-email.md** - MailKit vs JavaMail para envío de emails
- **background-tasks.md** - BackgroundService vs @Scheduled
- **real-time-patterns.md** - Patrones de comunicación en tiempo real

## 🔌 WebSockets Quick Example

### Spring Boot
```java
@Configuration
@EnableWebSocket
public class WebSocketConfig implements WebSocketConfigurer {
    public void registerWebSocketHandlers(WebSocketHandlerRegistry registry) {
        registry.addHandler(new ChatHandler(), "/ws/chat");
    }
}
```

### ASP.NET Core
```csharp
app.UseWebSockets();
app.Map("/ws/chat", async context =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
        await HandleWebSocketAsync(webSocket);
    }
});
```
