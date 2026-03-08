using ShoppingCartApi.Domain.Entities;
using ShoppingCartApi.Domain.Interfaces;
using ShoppingCartApi.Domain.Exceptions;
using ShoppingCartApi.Presentation.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;

namespace ShoppingCartApi.Application.Services
{
    /// <summary>
    /// Servicio de autenticación que encapsula toda la lógica de:
    /// - Registro e inicio de sesión
    /// - Generación y validación de JWT tokens
    /// - Gestión de refresh tokens
    /// - Hasheo y validación de contraseñas
    /// 
    /// Principios SOLID:
    /// - Single Responsibility (SRP): Solo maneja autenticación
    /// - Dependency Inversion (DIP): Depende de IUserRepository
    /// - Open/Closed (OCP): Extensible sin modificar
    /// </summary>
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private const int MaxFailedAttempts = 5;
        private const int LockoutMinutes = 15;
        private const int AccessTokenExpirationMinutes = 15;
        private const int RefreshTokenExpirationDays = 7;

        public AuthenticationService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        /// <summary>
        /// Registra un nuevo usuario en el sistema.
        /// </summary>
        public async Task<AuthResponse> RegisterAsync(string username, string password)
        {
            // Validar parámetros
            if (string.IsNullOrWhiteSpace(username) || username.Length < 3 || username.Length > 50)
            {
                throw new ValidationException(new[] { "Username must be between 3 and 50 characters" });
            }

            if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
            {
                throw new ValidationException(new[] { "Password must be at least 6 characters" });
            }

            // Verificar que el usuario no existe
            var existingUser = await _userRepository.GetByUsernameAsync(username);
            if (existingUser != null)
            {
                throw new ValidationException(new[] { "Username already exists" });
            }

            // Crear nuevo usuario
            var newUser = new User
            {
                Username = username,
                PasswordHash = HashPassword(password),
                FailedLoginAttempts = 0,
                LockoutEnd = null,
                CreatedAt = DateTime.UtcNow,
                LastLoginAt = null
            };

            await _userRepository.AddUserAsync(newUser);

            // Generar tokens
            var accessToken = GenerateAccessToken(newUser);
            var refreshToken = GenerateRefreshToken();

            // Guardar refresh token (normalmente se guardaría en base de datos)
            newUser.RefreshToken = refreshToken;
            newUser.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(RefreshTokenExpirationDays);
            await _userRepository.UpdateUserAsync(newUser);

            return new AuthResponse
            {
                Token = accessToken,
                RefreshToken = refreshToken,
                User = new UserInfo { Id = newUser.Id, Username = newUser.Username }
            };
        }

        /// <summary>
        /// Autentica un usuario existente y genera tokens JWT.
        /// </summary>
        public async Task<AuthResponse> LoginAsync(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                throw new ValidationException(new[] { "Username and password are required" });
            }

            //Obtener usuario
            var user = await _userRepository.GetByUsernameAsync(username);
            if (user == null)
            {
                throw new NotFoundException($"User '{username}' not found");
            }

            // Verificar si la cuenta está bloqueada
            if (user.LockoutEnd.HasValue && user.LockoutEnd > DateTime.UtcNow)
            {
                var lockedUntil = user.LockoutEnd.Value.ToString("yyyy-MM-dd HH:mm:ss");
                throw new ValidationException(new[] { $"Account is locked until {lockedUntil}" });
            }

            // Validar contraseña
            if (!VerifyPassword(password, user.PasswordHash))
            {
                // Incrementar intentos fallidos
                user.FailedLoginAttempts++;

                if (user.FailedLoginAttempts >= MaxFailedAttempts)
                {
                    user.LockoutEnd = DateTime.UtcNow.AddMinutes(LockoutMinutes);
                    await _userRepository.UpdateUserAsync(user);
                    throw new ValidationException(new[] { $"Too many failed login attempts. Account locked for {LockoutMinutes} minutes." });
                }

                await _userRepository.UpdateUserAsync(user);
                throw new ValidationException(new[] { "Invalid username or password" });
            }

            // Credenciales válidas: resetear intentos fallidos
            user.FailedLoginAttempts = 0;
            user.LockoutEnd = null;
            user.LastLoginAt = DateTime.UtcNow;
            await _userRepository.UpdateUserAsync(user);

            // Generar tokens
            var accessToken = GenerateAccessToken(user);
            var refreshToken = GenerateRefreshToken();

            // Guardar refresh token
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(RefreshTokenExpirationDays);
            await _userRepository.UpdateUserAsync(user);

            return new AuthResponse
            {
                Token = accessToken,
                RefreshToken = refreshToken,
                User = new UserInfo { Id = user.Id, Username = user.Username }
            };
        }

        /// <summary>
        /// Genera un nuevo token de acceso usando un refresh token válido.
        /// </summary>
        public async Task<string> RefreshTokenAsync(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                throw new ValidationException(new[] { "Refresh token is required" });
            }

            // Aquí deberías verificar el refresh token en la base de datos
            // Por ahora, esta es una implementación base que puede extenderse
            throw new NotImplementedException("Refresh token endpoint requires database integration");
        }

        /// <summary>
        /// Valida un token JWT y extrae sus claims.
        /// </summary>
        public Dictionary<string, object>? ValidateToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"] ?? "");

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _configuration["Jwt:Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var claims = new Dictionary<string, object>();

                foreach (var claim in jwtToken.Claims)
                {
                    claims[claim.Type] = claim.Value;
                }

                return claims;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Verifica si una contraseña coincide con el hash.
        /// </summary>
        public bool VerifyPassword(string password, string hash)
        {
            if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(hash))
            {
                return false;
            }

            try
            {
                return BCrypt.Net.BCrypt.Verify(password, hash);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Genera un hash seguro de una contraseña usando BCrypt.
        /// </summary>
        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        // ==================== Métodos privados ====================

        /// <summary>
        /// Genera un token JWT de acceso.
        /// Válido por AccessTokenExpirationMinutes (15 minutos por defecto).
        /// </summary>
        private string GenerateAccessToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"] ?? "");

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim("userId", user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddMinutes(AccessTokenExpirationMinutes),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        /// <summary>
        /// Genera un refresh token aleatorio (puede ser UUID o hash).
        /// </summary>
        private string GenerateRefreshToken()
        {
            return Convert.ToBase64String(System.Security.Cryptography.RandomNumberGenerator.GetBytes(32));
        }
    }
}
