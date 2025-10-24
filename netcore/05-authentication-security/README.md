# 🔐 Authentication & Security

## Introducción

Implementación completa de autenticación JWT y autorización en ASP.NET Core, comparando con Spring Security.

## 📚 Contenido

- **jwt-authentication.md** - JWT setup completo vs Spring Security
- **aspnet-identity.md** - ASP.NET Identity vs Spring Security UserDetailsService  
- **authorization.md** - [Authorize] vs @PreAuthorize, políticas y claims
- **middleware.md** - Security middleware pipeline

## 🔑 JWT Authentication Quick Start

### Spring Boot Security
```java
@Configuration
@EnableWebSecurity
public class SecurityConfig {
    @Bean
    public SecurityFilterChain filterChain(HttpSecurity http) {
        return http.csrf(csrf -> csrf.disable())
            .authorizeHttpRequests(auth -> auth
                .requestMatchers("/api/auth/**").permitAll()
                .anyRequest().authenticated())
            .addFilterBefore(jwtAuthFilter, UsernamePasswordAuthenticationFilter.class)
            .build();
    }
}
```

### ASP.NET Core
```csharp
// Program.cs
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        options.TokenValidationParameters = new TokenValidationParameters {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

app.UseAuthentication();
app.UseAuthorization();
```

## 🎯 Temas Cubiertos

- JWT token generation y validation
- Password hashing (BCrypt)
- Role-based authorization
- Claims-based authorization
- Security best practices
