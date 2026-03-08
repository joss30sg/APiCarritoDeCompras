using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ShoppingCartApi.Middleware
{
    /// <summary>
    /// Middleware para implementar Rate Limiting (limitación de velocidad) en la API.
    /// Protege contra ataques de fuerza bruta y abuso de recursos.
    /// </summary>
    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly RateLimitOptions _options;
        
        // Diccionario para almacenar información de límite de velocidad por IP
        private static readonly ConcurrentDictionary<string, RateLimitInfo> ClientRequests 
            = new ConcurrentDictionary<string, RateLimitInfo>();

        public RateLimitingMiddleware(RequestDelegate next, RateLimitOptions options)
        {
            _next = next;
            _options = options ?? new RateLimitOptions();
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var clientIp = GetClientIpAddress(context);
            var endpoint = context.Request.Path.ToString();

            // Verificar si el endpoint tiene una regla personalizada
            var rateLimit = GetRateLimitForEndpoint(endpoint, clientIp);

            if (rateLimit.IsLimited)
            {
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                context.Response.ContentType = "application/json";
                
                var retryAfter = (rateLimit.ResetTime - DateTime.UtcNow).TotalSeconds;
                context.Response.Headers["Retry-After"] = Math.Ceiling(retryAfter).ToString();

                await context.Response.WriteAsJsonAsync(new
                {
                    error = "Too Many Requests",
                    message = $"Rate limit exceeded. Try again in {Math.Ceiling(retryAfter)} seconds.",
                    retryAfter = Math.Ceiling(retryAfter)
                });

                return;
            }

            await _next(context);
        }

        private RateLimitInfo GetRateLimitForEndpoint(string endpoint, string clientIp)
        {
            var key = $"{clientIp}_{endpoint}";
            var now = DateTime.UtcNow;

            // Obtener o crear la información del cliente
            var clientInfo = ClientRequests.GetOrAdd(key, _ => new RateLimitInfo
            {
                RequestCount = 0,
                ResetTime = now.AddMinutes(1)
            });

            // Resetear si hemos pasado el tiempo de ventana
            if (now >= clientInfo.ResetTime)
            {
                clientInfo = new RateLimitInfo
                {
                    RequestCount = 0,
                    ResetTime = now.AddMinutes(1)
                };
                ClientRequests[key] = clientInfo;
            }

            // Incrementar el contador
            clientInfo.RequestCount++;

            // Obtener el límite específico del endpoint
            int rateLimit = GetRateLimitForEndpoint(endpoint);

            // Verificar si excedió el límite
            if (clientInfo.RequestCount > rateLimit)
            {
                clientInfo.IsLimited = true;
            }

            return clientInfo;
        }

        private int GetRateLimitForEndpoint(string endpoint)
        {
            // Límites específicos para endpoints sensibles
            if (endpoint.Contains("/api/v1/auth/login", StringComparison.OrdinalIgnoreCase))
                return _options.LoginAttempts; // Por defecto 5 intentos por minuto

            if (endpoint.Contains("/api/v1/auth/register", StringComparison.OrdinalIgnoreCase))
                return _options.RegisterAttempts; // Por defecto 3 registros por minuto

            // Límite general para otros endpoints
            return _options.GeneralLimit; // Por defecto 100 requests por minuto
        }

        private string GetClientIpAddress(HttpContext context)
        {
            // Intenta obtener la IP real del cliente, considerando proxies
            var forwarded = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(forwarded))
            {
                return forwarded.Split(',')[0].Trim();
            }

            return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        }
    }

    /// <summary>
    /// Opciones de configuración para Rate Limiting
    /// </summary>
    public class RateLimitOptions
    {
        /// <summary>
        /// Número máximo de intentos de login por minuto por IP
        /// </summary>
        public int LoginAttempts { get; set; } = 5;

        /// <summary>
        /// Número máximo de registros por minuto por IP
        /// </summary>
        public int RegisterAttempts { get; set; } = 3;

        /// <summary>
        /// Límite general de requests por minuto por IP
        /// </summary>
        public int GeneralLimit { get; set; } = 100;

        /// <summary>
        /// Ventana de tiempo en minutos para resetear el contador
        /// </summary>
        public int WindowSizeMinutes { get; set; } = 1;
    }

    /// <summary>
    /// Información de Rate Limiting por cliente
    /// </summary>
    public class RateLimitInfo
    {
        /// <summary>
        /// Número de requests realizados
        /// </summary>
        public int RequestCount { get; set; }

        /// <summary>
        /// Fecha y hora en que se resetea el contador
        /// </summary>
        public DateTime ResetTime { get; set; }

        /// <summary>
        /// Indicador si el cliente está limitado
        /// </summary>
        public bool IsLimited { get; set; }
    }
}
