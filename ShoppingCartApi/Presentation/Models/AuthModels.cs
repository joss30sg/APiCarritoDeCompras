using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace ShoppingCartApi.Presentation.Models
{
    /// <summary>Solicitud de registro: username único (3-50 caracteres) y password (6-100 caracteres)</summary>
    public class RegisterRequest
    {
        /// <summary>Nombre de usuario único (3-50 caracteres)</summary>
        /// <example>johndoe</example>
        [Required(ErrorMessage = "Username is required")]
        [MinLength(3, ErrorMessage = "Username must be at least 3 characters")]
        [MaxLength(50, ErrorMessage = "Username must not exceed 50 characters")]
        public required string Username { get; set; }

        /// <summary>Contraseña segura (mínimo 6 caracteres, máximo 100)</summary>
        /// <example>SecurePassword123!</example>
        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        [MaxLength(100, ErrorMessage = "Password must not exceed 100 characters")]
        public required string Password { get; set; }
    }

    /// <summary>Solicitud de login: valida credenciales contra base de datos</summary>
    public class LoginRequest
    {
        /// <summary>Nombre de usuario registrado</summary>
        /// <example>johndoe</example>
        [Required(ErrorMessage = "Username is required")]
        [MinLength(3, ErrorMessage = "Username must be at least 3 characters")]
        public required string Username { get; set; }

        /// <summary>Contraseña del usuario</summary>
        /// <example>SecurePassword123!</example>
        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        public required string Password { get; set; }
    }

    /// <summary>Respuesta exitosa: contiene token JWT y datos del usuario</summary>
    public class AuthResponse
    {
        /// <summary>Token JWT válido por 7 días para autenticación en futuras solicitudes</summary>
        public required string Token { get; set; }

        /// <summary>Token de refresco para obtener nuevos access tokens</summary>
        public string? RefreshToken { get; set; }

        /// <summary>Información básica del usuario autenticado</summary>
        public UserInfo? User { get; set; }
    }

    /// <summary>Información pública del usuario autenticado</summary>
    public class UserInfo
    {
        /// <summary>ID único del usuario en el sistema</summary>
        /// <example>1</example>
        public int Id { get; set; }

        /// <summary>Nombre de usuario único</summary>
        /// <example>johndoe</example>
        public required string Username { get; set; }
    }
}
