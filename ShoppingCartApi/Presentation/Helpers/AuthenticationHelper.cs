using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace ShoppingCartApi.Presentation.Helpers
{
    /// <summary>
    /// Helper para extraer información del usuario autenticado desde el token JWT.
    /// </summary>
    public static class AuthenticationHelper
    {
        /// <summary>
        /// Obtiene el ID del usuario del claim del token JWT.
        /// </summary>
        /// <param name="user">ClaimsPrincipal obtenido del contexto HTTP.</param>
        /// <returns>ID del usuario como entero, o -1 si no se encuentra.</returns>
        public static int GetUserId(ClaimsPrincipal user)
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdClaim, out var userId))
            {
                return userId;
            }
            return -1;
        }

        /// <summary>
        /// Obtiene el nombre de usuario del claim del token JWT.
        /// </summary>
        /// <param name="user">ClaimsPrincipal obtenido del contexto HTTP.</param>
        /// <returns>Nombre de usuario, o null si no se encuentra.</returns>
        public static string? GetUsername(ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Name)?.Value;
        }

        /// <summary>
        /// Valida si el usuario está correctamente autenticado.
        /// </summary>
        /// <param name="user">ClaimsPrincipal obtenido del contexto HTTP.</param>
        /// <returns>true si el usuario tiene ID válido, false en caso contrario.</returns>
        public static bool IsAuthenticated(ClaimsPrincipal user)
        {
            return GetUserId(user) > 0;
        }

        /// <summary>
        /// Retorna una respuesta BadRequest si el usuario no está autenticado.
        /// </summary>
        /// <param name="controller">Referencia del controlador para retornar resultado.</param>
        /// <param name="user">ClaimsPrincipal obtenido del contexto HTTP.</param>
        /// <returns>BadRequest o null si está autenticado.</returns>
        public static IActionResult? ValidateUserAuthentication(ControllerBase controller, ClaimsPrincipal user)
        {
            if (!IsAuthenticated(user))
            {
                return controller.StatusCode(
                    StatusCodes.Status401Unauthorized,
                    Models.ApiResponse.ErrorResponse(
                        "User authentication failed or invalid token",
                        StatusCodes.Status401Unauthorized
                    )
                );
            }
            return null;
        }
    }
}
