using ShoppingCartApi.Domain.Entities;
using ShoppingCartApi.Presentation.Models;
using System.Threading.Tasks;

namespace ShoppingCartApi.Application.Services
{
    /// <summary>
    /// Interfaz para el servicio de autenticación.
    /// Encapsula toda la lógica de autenticación, validación y generación de tokens.
    /// Principio SOLID: Dependency Inversion Principle (DIP)
    /// </summary>
    public interface IAuthenticationService
    {
        /// <summary>
        /// Registra un nuevo usuario en el sistema.
        /// </summary>
        /// <param name="username">Nombre de usuario único</param>
        /// <param name="password">Contraseña en texto plano (será hasheada)</param>
        /// <returns>Respuesta de autenticación con token JWT</returns>
        Task<AuthResponse> RegisterAsync(string username, string password);

        /// <summary>
        /// Autentica un usuario existente.
        /// </summary>
        /// <param name="username">Nombre de usuario</param>
        /// <param name="password">Contraseña en texto plano</param>
        /// <returns>Respuesta de autenticación con tokens JWT y refresh token</returns>
        Task<AuthResponse> LoginAsync(string username, string password);

        /// <summary>
        /// Genera un nuevo token de acceso usando un refresh token válido.
        /// </summary>
        /// <param name="refreshToken">Token de refresco</param>
        /// <returns>Nuevo token de acceso</returns>
        Task<string> RefreshTokenAsync(string refreshToken);

        /// <summary>
        /// Valida un token JWT y extrae sus claims.
        /// </summary>
        /// <param name="token">Token JWT a validar</param>
        /// <returns>Claims del token si es válido, null si es inválido o expiró</returns>
        System.Collections.Generic.Dictionary<string, object>? ValidateToken(string token);

        /// <summary>
        /// Verifica si una contraseña coincide con el hash almacenado.
        /// </summary>
        /// <param name="password">Contraseña en texto plano</param>
        /// <param name="hash">Hash BCrypt almacenado</param>
        /// <returns>True si coincide, false en caso contrario</returns>
        bool VerifyPassword(string password, string hash);

        /// <summary>
        /// Genera un hash seguro de una contraseña usando BCrypt.
        /// </summary>
        /// <param name="password">Contraseña en texto plano</param>
        /// <returns>Hash BCrypt de la contraseña</returns>
        string HashPassword(string password);
    }
}
