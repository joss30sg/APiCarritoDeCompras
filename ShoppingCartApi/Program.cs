using ShoppingCartApi.Domain.Interfaces;
using ShoppingCartApi.Infrastructure.Repositories;
using ShoppingCartApi.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using System.Linq;
using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using ShoppingCartApi.Middleware; // Añadir para el middleware de manejo de excepciones
using System.IO; // Añadir para Path.Combine
using Microsoft.AspNetCore.Mvc.Controllers;

public partial class Program // Make Program class public and partial
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });

}

public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        // Add services to the container.
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        
        // Configuración de Rate Limiting
        var rateLimitOptions = new RateLimitOptions
        {
            LoginAttempts = Configuration.GetValue("RateLimit:LoginAttempts", 5),
            RegisterAttempts = Configuration.GetValue("RateLimit:RegisterAttempts", 3),
            GeneralLimit = Configuration.GetValue("RateLimit:GeneralLimit", 100),
            WindowSizeMinutes = Configuration.GetValue("RateLimit:WindowSizeMinutes", 1)
        };
        services.AddSingleton(rateLimitOptions);
        
        services.AddControllers(); // Simplificar la configuración de JSON para ver si resuelve el problema de PipeWriter
        services.AddEndpointsApiExplorer(); // Required for Swagger/OpenAPI
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo 
            { 
                Title = "Shopping Cart API",
                Version = "v1.0.0",
                Description = @"
# API RESTful Profesional para Carrito de Compras

## Descripción General
Esta API proporciona funcionalidades completas para:
- **Autenticación**: Registro e inicio de sesión con JWT Bearer tokens
- **Carrito de Compras**: CRUD completo de productos con atributos personalizados
- **Seguridad**: Protección contra ataques, validación de tokens, bloqueo por intentos fallidos
- **Respuestas Estandardizadas**: Estructura uniforme en todas las respuestas

## Características Principales
✅ Autenticación con JWT (7 días de validez)
✅ Protección contra fuerza bruta (bloqueo 15 min / 5 intentos)
✅ Carrito de compras con productos y atributos
✅ Validación de datos en request/response
✅ Errores detallados y mensajes claros
✅ Endpoints ordenados por versión (v1)
✅ Documentación exhaustiva con ejemplos

## Status Codes Utilizados
- **200 OK**: Operación exitosa con datos
- **201 Created**: Recurso creado exitosamente
- **204 No Content**: Operación exitosa sin contenido
- **400 Bad Request**: Errores de validación o datos inválidos
- **401 Unauthorized**: Autenticación requerida o falló
- **403 Forbidden**: Acceso denegado (ej: cuenta bloqueada)
- **500 Internal Server Error**: Error inesperado
",
                Contact = new OpenApiContact
                {
                    Name = "Equipo de Desarrollo",
                    Email = "support@shoppingcart.com",
                    Url = new Uri("https://github.com/ShoppingCartAPI")
                },
                License = new OpenApiLicense
                {
                    Name = "MIT License",
                    Url = new Uri("https://opensource.org/licenses/MIT")
                }
            });

            // Definir esquema de seguridad Bearer para JWT
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = @"Autenticación JWT usando Bearer token. 
Incluya el token en el header Authorization:
Example: 'Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...'

OBTENER TOKEN:
1. POST /api/v1/auth/register - Registrar nuevo usuario
2. POST /api/v1/auth/login - Login usuario existente
3. Guardar token en Authorization header para futuras requests",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT"
            });

            // Requerir Bearer scheme en endpoints autenticados
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });

            // Agrupar endpoints por tags (Auth, Cart, etc.)
            c.TagActionsBy(api =>
            {
                if (api?.ActionDescriptor is ControllerActionDescriptor controllerAction)
                {
                    return new[] { controllerAction.ControllerName };
                }
                return new[] { "Undefined" };
            });

            // Incluir comentarios XML para mejorar documentación
            var xmlFile = $"{typeof(Program).Assembly.GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
            }

            // Configurar para que Swagger UI muestre los comentarios de resoluciones
            c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());

            // Ordenar endpoints por nombre de controlador
            c.OrderActionsBy(e => e.RelativePath);
        });
        
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly)); // Add MediatR
        
        // Registrar servicio de autenticación
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        
        services.AddSingleton<IShoppingCartRepository, InMemoryShoppingCartRepository>();
        services.AddSingleton<IProductRepository, InMemoryProductRepository>(); // Register InMemoryProductRepository
        services.AddSingleton<IUserRepository, InMemoryUserRepository>(); // Register InMemoryUserRepository

        // Configuración de CORS
        services.AddCors(options =>
        {
            options.AddPolicy("AllowSpecificOrigin",
                builder =>
                {
                    builder.WithOrigins(
                        "http://localhost:4200",      // Angular local dev
                        "http://localhost:3000",      // Alternative Angular port
                        "http://localhost:5276",      // API itself
                        "https://localhost:7276"      // HTTP fallback
                    )
                           .AllowAnyHeader()
                           .AllowAnyMethod()
                           .AllowCredentials();
                });
        });

        // Configuración de autenticación JWT
        var jwtSecret = Configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key not found.");
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Configuration["Jwt:Issuer"],
                    ValidAudience = Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
                };

                // Manejar errores de autenticación - retornar 401 en lugar de 500
                options.Events = new JwtBearerEvents
                {
                    OnChallenge = async context =>
                    {
                        context.HandleResponse();
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json";
                        
                        var response = new
                        {
                            statusCode = 401,
                            message = "Token no proporcionado o inválido",
                            error = context.ErrorDescription ?? "Unauthorized access"
                        };
                        
                        await context.Response.WriteAsJsonAsync(response);
                    },
                    OnAuthenticationFailed = async context =>
                    {
                        context.NoResult();
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json";
                        
                        var response = new
                        {
                            statusCode = 401,
                            message = "Autenticación fallida",
                            error = context.Exception?.Message ?? "Authentication failed"
                        };
                        
                        await context.Response.WriteAsJsonAsync(response);
                    }
                };
            });

        services.AddAuthorization();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        // Añadir el middleware de manejo de excepciones personalizado al principio del pipeline
        app.UseMiddleware<ExceptionHandlingMiddleware>();

        // Añadir Rate Limiting Middleware para proteger contra ataques de fuerza bruta
        var rateLimitOptions = app.ApplicationServices.GetRequiredService<RateLimitOptions>();
        app.UseMiddleware<RateLimitingMiddleware>(rateLimitOptions);

        // Configure the HTTP request pipeline.
        if (env.IsDevelopment() && !env.IsEnvironment("Testing"))
        {
            app.UseSwagger();
            
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Shopping Cart API v1.0.0");
                c.RoutePrefix = "swagger"; // Swagger en /swagger
                c.DocumentTitle = "Shopping Cart API - Documentación Profesional";
                c.DefaultModelsExpandDepth(1);
                c.DefaultModelExpandDepth(1);
                c.EnableFilter();
                c.DisplayRequestDuration();
                c.EnableDeepLinking();
            });
        }
        else
        {
            // El manejador de excepciones global ya está cubierto por el middleware personalizado
            // app.UseExceptionHandler("/Error");
            // Habilitar HSTS para producción
            app.UseHsts();
        }

        app.UseHttpsRedirection(); // Activar redirección HTTPS

        // Servir archivos estáticos desde wwwroot
        app.UseStaticFiles();

        // Usar la política de CORS
        app.UseCors("AllowSpecificOrigin");

        // Añadir encabezados de seguridad HTTP para OWASP
        app.Use(async (context, next) =>
        {
            context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
            context.Response.Headers.Append("X-Frame-Options", "DENY");
            context.Response.Headers.Append("Referrer-Policy", "no-referrer-when-downgrade");
            context.Response.Headers.Append("Permissions-Policy", "geolocation=(), microphone=(), camera=()");
            await next();
        });

        app.UseRouting(); // Add UseRouting
        app.UseAuthentication(); // Debe ir antes de UseAuthorization
        app.UseAuthorization();

        app.UseEndpoints(endpoints => // Add UseEndpoints
        {
            endpoints.MapControllers(); // Mapear los controladores de API
            
            // Configurar Angular app para rutas fallback
            endpoints.MapGet("/", context =>
            {
                context.Response.ContentType = "text/html";
                return context.Response.SendFileAsync(Path.Combine(env.WebRootPath, "app", "index.html"));
            });
            
            // Redirigir todas las rutas no reconocidas a index.html (para Angular routing)
            endpoints.MapFallback(async context =>
            {
                if (!context.Request.Path.StartsWithSegments("/api") && 
                    !context.Request.Path.StartsWithSegments("/swagger"))
                {
                    context.Response.ContentType = "text/html";
                    await context.Response.SendFileAsync(Path.Combine(env.WebRootPath, "app", "index.html"));
                }
            });
        });
    }
}
