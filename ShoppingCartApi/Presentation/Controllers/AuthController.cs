using Microsoft.AspNetCore.Mvc;
using ShoppingCartApi.Application.Services;
using ShoppingCartApi.Presentation.Models;
using ShoppingCartApi.Presentation.Helpers;
using System.Threading.Tasks;
using System;
using System.ComponentModel.DataAnnotations;
using ShoppingCartApi.Domain.Exceptions;

namespace ShoppingCartApi.Presentation.Controllers
{
    /// <summary>
    /// Controlador de Autenticación - API v1
    /// Maneja el registro e inicio de sesión de usuarios con tokens JWT.
    /// 
    /// Principio SOLID:
    /// - Single Responsibility: Solo maneja solicitudes HTTP
    /// - Dependency Inversion: Inyecta IAuthenticationService
    /// </summary>
    [ApiController]
    [Route("api/v1/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
        }

        /// <summary>Registra nuevo usuario y retorna token JWT</summary>
        /// <remarks>
        /// Validaciones: username único (3-50 caracteres), password hasheado BCrypt, token expira en 15 minutos
        /// </remarks>
        [HttpPost("register")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(ApiResponse<RegisterResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                // Validar datos de entrada
                var validationError = ValidateRegisterRequest(request);
                if (validationError != null)
                    return validationError;

                // Llamar al servicio de autenticación
                var authResponse = await _authenticationService.RegisterAsync(request.Username, request.Password);

                // Crear respuesta
                var response = ApiResponse<RegisterResponse>.SuccessResponse(
                    new RegisterResponse
                    {
                        Token = authResponse.Token,
                        RefreshToken = authResponse.RefreshToken,
                        User = authResponse.User
                    },
                    "User registered successfully",
                    StatusCodes.Status201Created
                );

                return StatusCode(StatusCodes.Status201Created, response);
            }
            catch (ShoppingCartApi.Domain.Exceptions.ValidationException ex)
            {
                return BadRequest(ApiResponse.ErrorResponse(
                    ex.Message,
                    StatusCodes.Status400BadRequest,
                    ex.Errors?.Keys.ToList() ?? new List<string>()
                ));
            }
            catch (Exception)
            {
                return HandleInternalError("registration");
            }
        }

        /// <summary>Autentica usuario y retorna token JWT</summary>
        /// <remarks>Valida credenciales y verifica bloqueo por intentos fallidos (5 intentos, 15 min lockout)</remarks>
        [HttpPost("login")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                // Validar datos de entrada
                var validationError = ValidateLoginRequest(request);
                if (validationError != null)
                    return validationError;

                // Llamar al servicio de autenticación
                var authResponse = await _authenticationService.LoginAsync(request.Username, request.Password);

                // Crear respuesta
                var response = ApiResponse<LoginResponse>.SuccessResponse(
                    new LoginResponse
                    {
                        Token = authResponse.Token,
                        RefreshToken = authResponse.RefreshToken,
                        User = authResponse.User,
                        ExpiresIn = 900 // 15 minutos
                    },
                    "Login successful",
                    StatusCodes.Status200OK
                );

                return Ok(response);
            }
            catch (ShoppingCartApi.Domain.Exceptions.ValidationException ex)
            {
                return BadRequest(ApiResponse.ErrorResponse(
                    ex.Message,
                    StatusCodes.Status400BadRequest,
                    ex.Errors?.Keys.ToList() ?? new List<string>()
                ));
            }
            catch (NotFoundException)
            {
                return Unauthorized(ApiResponse.ErrorResponse(
                    "Invalid credentials",
                    StatusCodes.Status401Unauthorized
                ));
            }
            catch (Exception)
            {
                return HandleInternalError("login");
            }
        }

        #region Private Helper Methods

        /// <summary>Valida datos de solicitud de registro</summary>
        private IActionResult? ValidateRegisterRequest(RegisterRequest request)
        {
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(request);

            if (!Validator.TryValidateObject(request, validationContext, validationResults, true))
            {
                var errors = validationResults
                    .Select(vr => vr.ErrorMessage ?? "Validation error")
                    .ToList();

                return BadRequest(ApiResponse.ErrorResponse(
                    "Validation failed",
                    StatusCodes.Status400BadRequest,
                    errors
                ));
            }

            return null;
        }

        /// <summary>Valida datos de solicitud de login</summary>
        private IActionResult? ValidateLoginRequest(LoginRequest request)
        {
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(request);

            if (!Validator.TryValidateObject(request, validationContext, validationResults, true))
            {
                var errors = validationResults
                    .Select(vr => vr.ErrorMessage ?? "Validation error")
                    .ToList();

                return BadRequest(ApiResponse.ErrorResponse(
                    "Validation failed",
                    StatusCodes.Status400BadRequest,
                    errors
                ));
            }

            return null;
        }

        /// <summary>
        /// Maneja errores internos del servidor de forma consistente.
        /// </summary>
        /// <param name="operationName">Nombre de la operación que falló (ej: "registration", "login")</param>
        /// <param name="ex">Excepción capturada (opcional)</param>
        /// <returns>Respuesta 500 Internal Server Error</returns>
        private IActionResult HandleInternalError(string operationName, Exception? ex = null)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                ApiResponse.ErrorResponse(
                    $"An error occurred during {operationName}",
                    StatusCodes.Status500InternalServerError,
                    ex != null ? new List<string> { ex.Message } : new List<string>()
                )
            );
        }

        #endregion
    }
}
