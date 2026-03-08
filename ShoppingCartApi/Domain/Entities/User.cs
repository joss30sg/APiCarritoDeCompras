using System;

namespace ShoppingCartApi.Domain.Entities
{
    /// <summary>
    /// Entidad Usuario - Representa un usuario del sistema con autenticación JWT
    /// </summary>
    /// <remarks>
    /// Responsabilidades:
    /// - Almacenar credenciales de usuario (Username, PasswordHash)
    /// - Gestionar intentos fallidos de login y bloqueo de cuenta
    /// - Registrar fechas de creación y último login
    /// 
    /// Patrón: Domain-Driven Design (Entity Pattern)
    /// Seguridad: Contraseña almacenada con BCrypt hash, nunca en texto plano
    /// </remarks>
    public class User
    {
        /// <summary>
        /// ID único del usuario (clave primaria)
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nombre de usuario único (3-50 caracteres)
        /// </summary>
        /// <remarks>
        /// Debe ser único en la base de datos
        /// Utilizado para login junto con la contraseña
        /// </remarks>
        public required string Username { get; set; }

        /// <summary>
        /// Hash seguro de la contraseña (NO es la contraseña en texto plano)
        /// </summary>
        /// <remarks>
        /// Generado con BCrypt.Net.BCrypt.HashPassword()
        /// Nunca se almacena ni transmite la contraseña original
        /// </remarks>
        public required string PasswordHash { get; set; }

        /// <summary>
        /// Contador de intentos fallidos de login consecutivos
        /// </summary>
        /// <remarks>
        /// Se incrementa en cada intento fallido de login
        /// Se resetea a 0 en login exitoso
        /// Cuando alcanza MaxFailedAttempts (5), la cuenta se bloquea temporalmente
        /// </remarks>
        public int FailedLoginAttempts { get; set; } = 0;

        /// <summary>
        /// Fecha y hora hasta la cual la cuenta está bloqueada (UTC)
        /// </summary>
        /// <remarks>
        /// Null = cuenta no está bloqueada
        /// Cuando DateTime.UtcNow > LockoutEnd, el usuario puede intentar login de nuevo
        /// Se establece cuando FailedLoginAttempts >= 5 por 15 minutos
        /// </remarks>
        public DateTime? LockoutEnd { get; set; }

        /// <summary>
        /// Fecha y hora de creación del usuario (UTC)
        /// </summary>
        /// <remarks>
        /// Se establece automáticamente al crear el usuario
        /// Utilizado para auditoría y estadísticas
        /// </remarks>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Fecha y hora del último login exitoso (UTC)
        /// </summary>
        /// <remarks>
        /// Null si el usuario nunca ha hecho login
        /// Se actualiza automáticamente en cada login exitoso
        /// Útil para auditoría y detección de actividad sospechosa
        /// </remarks>
        public DateTime? LastLoginAt { get; set; }

        /// <summary>
        /// Token de refresco (refresh token) para obtener nuevos access tokens
        /// </summary>
        /// <remarks>
        /// Almacenado de forma segura en la base de datos
        /// Se regenera en cada login
        /// Tiene una validez más larga que el access token (7 días por defecto)
        /// </remarks>
        public string? RefreshToken { get; set; }

        /// <summary>
        /// Fecha y hora de expiración del refresh token (UTC)
        /// </summary>
        /// <remarks>
        /// Null si no hay refresh token válido
        /// Se utiliza para validar la expiración del refresh token
        /// </remarks>
        public DateTime? RefreshTokenExpiryTime { get; set; }
    }
}
