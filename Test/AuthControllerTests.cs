using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using FluentAssertions;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Net.Http.Json; // Add this using directive
using ShoppingCartApi.Presentation.Models;
using Microsoft.Extensions.DependencyInjection;
using ShoppingCartApi.Domain.Interfaces;
using ShoppingCartApi.Infrastructure.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.DependencyInjection.Extensions; // Add this using directive
using Microsoft.Extensions.Options; // Add this using directive

namespace ShoppingCartApi.Tests
{
    public class AuthControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly IConfiguration _configuration;

        public AuthControllerTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Reemplazar los repositorios con implementaciones en memoria para las pruebas
                    services.AddSingleton<IUserRepository, InMemoryUserRepository>();
                    services.AddSingleton<IProductRepository, InMemoryProductRepository>();
                    services.AddSingleton<IShoppingCartRepository, InMemoryShoppingCartRepository>();
                });
            });

            // Configuración para las pruebas, incluyendo la clave JWT
            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    {"Jwt:Key", "supersecretkeyforjwttestingthatislongenough"},
                    {"Jwt:Issuer", "test-issuer"},
                    {"Jwt:Audience", "test-audience"},
                    {"Jwt:ExpireDays", "1"}
                })
                .Build();
        }

        private HttpClient CreateClientWithJwtConfig()
        {
            return _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    // Eliminar servicios de autenticación existentes para evitar el error "Scheme already exists"
                    services.RemoveAll<IAuthenticationService>();
                    services.RemoveAll<IAuthenticationHandlerProvider>();
                    services.RemoveAll<IAuthenticationSchemeProvider>();
                    services.RemoveAll<IPostConfigureOptions<AuthenticationOptions>>();
                    services.RemoveAll<IConfigureOptions<AuthenticationOptions>>();

                    services.AddSingleton<IConfiguration>(_configuration);
                    services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                        .AddJwtBearer(options =>
                        {
                            options.TokenValidationParameters = new TokenValidationParameters
                            {
                                ValidateIssuer = true,
                                ValidateAudience = true,
                                ValidateLifetime = true,
                                ValidateIssuerSigningKey = true,
                                ValidIssuer = _configuration["Jwt:Issuer"],
                                ValidAudience = _configuration["Jwt:Audience"],
                                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!))
                            };
                        });
                });
            }).CreateClient();
        }

        [Fact]
        public async Task Register_ReturnsOk_WhenUserIsRegisteredSuccessfully()
        {
            // Arrange
            var client = CreateClientWithJwtConfig();
            var request = new RegisterRequest { Username = "testuser", Password = "password123" };
            var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

            // Act
            var response = await client.PostAsync("/api/Auth/register", content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            (await response.Content.ReadAsStringAsync()).Should().Be("User registered successfully.");
        }

        [Fact]
        public async Task Register_ReturnsBadRequest_WhenUsernameAlreadyExists()
        {
            // Arrange
            var client = CreateClientWithJwtConfig();
            var request = new RegisterRequest { Username = "existinguser", Password = "password123" };
            var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

            // Registrar el usuario por primera vez
            await client.PostAsync("/api/Auth/register", content);

            // Act: Intentar registrar el mismo usuario de nuevo
            var response = await client.PostAsync("/api/Auth/register", content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            (await response.Content.ReadAsStringAsync()).Should().Be("Username already exists.");
        }

        [Fact]
        public async Task Login_ReturnsAuthResponse_WhenCredentialsAreValid()
        {
            // Arrange
            var client = CreateClientWithJwtConfig();
            var registerRequest = new RegisterRequest { Username = "loginuser", Password = "password123" };
            var registerContent = new StringContent(JsonSerializer.Serialize(registerRequest), Encoding.UTF8, "application/json");
            await client.PostAsync("/api/Auth/register", registerContent); // Registrar el usuario primero

            var loginRequest = new LoginRequest { Username = "loginuser", Password = "password123" };
            var loginContent = new StringContent(JsonSerializer.Serialize(loginRequest), Encoding.UTF8, "application/json");

            // Act
            var response = await client.PostAsync("/api/Auth/login", loginContent);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>(new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            authResponse.Should().NotBeNull();
            authResponse!.Token.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task Login_ReturnsUnauthorized_WhenCredentialsAreInvalid()
        {
            // Arrange
            var client = CreateClientWithJwtConfig();
            var loginRequest = new LoginRequest { Username = "nonexistentuser", Password = "wrongpassword" };
            var loginContent = new StringContent(JsonSerializer.Serialize(loginRequest), Encoding.UTF8, "application/json");

            // Act
            var response = await client.PostAsync("/api/Auth/login", loginContent);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
            (await response.Content.ReadAsStringAsync()).Should().Be("Invalid credentials.");
        }

        [Fact]
        public void GenerateJwtToken_ReturnsValidToken()
        {
            // Arrange
            var user = new ShoppingCartApi.Domain.Entities.User { Id = 1, Username = "tokenuser", PasswordHash = "dummyhash" };
            var authController = new ShoppingCartApi.Presentation.Controllers.AuthController(
                new InMemoryUserRepository(), // Se usa un repositorio en memoria para esta prueba
                _configuration
            );

            // Act
            var token = authController.GetType().GetMethod("GenerateJwtToken", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                                      ?.Invoke(authController, new object[] { user }) as string ?? throw new InvalidOperationException("Token generation failed.");

            // Assert
            token.Should().NotBeNullOrEmpty();

            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)),
                ValidateIssuer = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = _configuration["Jwt:Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero // No tolerancia de tiempo para la expiración
            };

            ClaimsPrincipal principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
            principal.Identity.Should().BeAssignableTo<ClaimsIdentity>();
            principal.Identity!.IsAuthenticated.Should().BeTrue();
            principal.Identity.Name.Should().Be(user.Username);
        }

        [Fact]
        public void GenerateJwtToken_ThrowsException_WhenJwtKeyIsMissing()
        {
            // Arrange
            var user = new ShoppingCartApi.Domain.Entities.User { Id = 1, Username = "testuser", PasswordHash = "dummyhash" };
            
            // Configuración con Jwt:Key faltante
            var missingKeyConfiguration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    {"Jwt:Issuer", "test-issuer"},
                    {"Jwt:Audience", "test-audience"},
                    {"Jwt:ExpireDays", "1"}
                })
                .Build();

            var authController = new ShoppingCartApi.Presentation.Controllers.AuthController(
                new InMemoryUserRepository(),
                missingKeyConfiguration
            );

            // Act
            Func<string> action = () => authController.GetType().GetMethod("GenerateJwtToken", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                                      ?.Invoke(authController, new object[] { user }) as string ?? throw new InvalidOperationException("GenerateJwtToken did not return a string.");

            // Assert
            action.Should().Throw<System.Reflection.TargetInvocationException>()
                  .WithInnerException<InvalidOperationException>()
                  .WithMessage("Jwt:Key not found.");
        }
    }
}
