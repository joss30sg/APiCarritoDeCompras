namespace ShoppingCartApi.Presentation.Models
{
    /// <summary>
    /// Respuesta de registro de usuario.
    /// </summary>
    public class RegisterResponse
    {
        /// <summary>
        /// Token JWT generado para el usuario registrado.
        /// </summary>
        public string Token { get; set; } = string.Empty;

        /// <summary>
        /// Token de refresco para obtener nuevos access tokens.
        /// </summary>
        public string? RefreshToken { get; set; }

        /// <summary>
        /// Información básica del usuario registrado.
        /// </summary>
        public UserInfo? User { get; set; }
    }

    /// <summary>
    /// Respuesta de inicio de sesión de usuario.
    /// </summary>
    public class LoginResponse
    {
        /// <summary>
        /// Token JWT para autenticación en futuras solicitudes.
        /// </summary>
        public string Token { get; set; } = string.Empty;

        /// <summary>
        /// Token de refresco para obtener nuevos access tokens.
        /// </summary>
        public string? RefreshToken { get; set; }

        /// <summary>
        /// Información básica del usuario autenticado.
        /// </summary>
        public UserInfo? User { get; set; }

        /// <summary>
        /// Tiempo de expiración del token en segundos.
        /// Por defecto: 604800 (7 días).
        /// </summary>
        public int ExpiresIn { get; set; } = 604800;

        /// <summary>
        /// Tipo de token (siempre "Bearer" para JWT).
        /// </summary>
        public string TokenType { get; set; } = "Bearer";
    }
}
