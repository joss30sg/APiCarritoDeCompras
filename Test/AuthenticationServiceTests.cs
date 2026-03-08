using Xunit;
using Moq;
using ShoppingCartApi.Domain.Entities;
using ShoppingCartApi.Domain.Interfaces;
using ShoppingCartApi.Domain.Exceptions;
using ShoppingCartApi.Application.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ShoppingCartApi.Tests
{
    /// <summary>
    /// Pruebas unitarias para AuthenticationService
    /// 
    /// Cubre:
    /// - Registro de usuarios (validación, duplicados, éxito)
    /// - Login (validación, usuario no existe, contraseña incorrecta)
    /// - Bloqueo de cuenta por intentos fallidos
    /// - Hasheo y validación de contraseñas
    /// - Generación de tokens
    /// 
    /// Patrón: AAA (Arrange, Act, Assert)
    /// Mocking: Moq para IUserRepository
    /// </summary>
    public class AuthenticationServiceTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly AuthenticationService _authService;

        public AuthenticationServiceTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockConfiguration = new Mock<IConfiguration>();
            
            // Configurar mocks para JWT
            _mockConfiguration
                .Setup(x => x["Jwt:Key"])
                .Returns("SuperSecretKeyThatIsLongEnoughForHS256AlgorithmTesting");
            
            _mockConfiguration
                .Setup(x => x["Jwt:Issuer"])
                .Returns("ShoppingCartApi");
            
            _mockConfiguration
                .Setup(x => x["Jwt:Audience"])
                .Returns("ShoppingCartUI");

            _authService = new AuthenticationService(_mockUserRepository.Object, _mockConfiguration.Object);
        }

        #region Pruebas de Registro

        [Fact]
        public async Task RegisterAsync_WithValidRequest_ShouldCreateUserSuccessfully()
        {
            // Arrange
            var username = "newuser";
            var password = "ValidPassword123!";
            
            _mockUserRepository
                .Setup(x => x.GetByUsernameAsync(username))
                .ReturnsAsync((User?)null);

            _mockUserRepository
                .Setup(x => x.AddUserAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask);

            _mockUserRepository
                .Setup(x => x.UpdateUserAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _authService.RegisterAsync(username, password);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Token);
            Assert.NotNull(result.RefreshToken);
            Assert.NotNull(result.User);
            
            _mockUserRepository.Verify(x => x.AddUserAsync(It.IsAny<User>()), Times.Once);
            _mockUserRepository.Verify(x => x.UpdateUserAsync(It.IsAny<User>()), Times.Once);
        }

        [Theory]
        [InlineData("ab", "ValidPassword123!")]  // Username muy corto
        [InlineData("", "ValidPassword123!")]     // Username vacío
        [InlineData("validuser", "")]             // Password vacío
        [InlineData("validuser", "short")]        // Password muy corto
        public async Task RegisterAsync_WithInvalidInput_ShouldThrowValidationException(string username, string password)
        {
            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(
                async () => await _authService.RegisterAsync(username, password)
            );
        }

        [Fact]
        public async Task RegisterAsync_WithExistingUsername_ShouldThrowValidationException()
        {
            // Arrange
            var username = "existinguser";
            var password = "ValidPassword123!";
            var existingUser = new User { Id = 1, Username = username, PasswordHash = "hash" };

            _mockUserRepository
                .Setup(x => x.GetByUsernameAsync(username))
                .ReturnsAsync(existingUser);

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(
                async () => await _authService.RegisterAsync(username, password)
            );

            _mockUserRepository.Verify(x => x.AddUserAsync(It.IsAny<User>()), Times.Never);
        }

        #endregion

        #region Pruebas de Login

        [Fact]
        public async Task LoginAsync_WithValidCredentials_ShouldReturnAuthResponse()
        {
            // Arrange
            var username = "testuser";
            var password = "TestPassword123!";
            var passwordHash = _authService.HashPassword(password);
            
            var user = new User
            {
                Id = 1,
                Username = username,
                PasswordHash = passwordHash,
                FailedLoginAttempts = 0,
                LockoutEnd = null
            };

            _mockUserRepository
                .Setup(x => x.GetByUsernameAsync(username))
                .ReturnsAsync(user);

            _mockUserRepository
                .Setup(x => x.UpdateUserAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _authService.LoginAsync(username, password);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Token);
            Assert.NotNull(result.RefreshToken);
            Assert.Equal(username, result.User?.Username);
            
            _mockUserRepository.Verify(
                x => x.UpdateUserAsync(It.Is<User>(u => u.FailedLoginAttempts == 0)),
                Times.Once
            );
        }

        [Fact]
        public async Task LoginAsync_WithNonexistentUser_ShouldThrowNotFoundException()
        {
            // Arrange
            var username = "nonexistent";
            
            _mockUserRepository
                .Setup(x => x.GetByUsernameAsync(username))
                .ReturnsAsync((User?)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(
                async () => await _authService.LoginAsync(username, "anypassword")
            );
        }

        [Fact]
        public async Task LoginAsync_WithWrongPassword_ShouldThrowValidationException()
        {
            // Arrange
            var username = "testuser";
            var correctPassword = "CorrectPassword123!";
            var wrongPassword = "WrongPassword123!";
            var passwordHash = _authService.HashPassword(correctPassword);

            var user = new User
            {
                Id = 1,
                Username = username,
                PasswordHash = passwordHash,
                FailedLoginAttempts = 0,
                LockoutEnd = null
            };

            _mockUserRepository
                .Setup(x => x.GetByUsernameAsync(username))
                .ReturnsAsync(user);

            _mockUserRepository
                .Setup(x => x.UpdateUserAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask);

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(
                async () => await _authService.LoginAsync(username, wrongPassword)
            );

            // Verify que incrementó intentos fallidos
            _mockUserRepository.Verify(
                x => x.UpdateUserAsync(It.Is<User>(u => u.FailedLoginAttempts == 1)),
                Times.Once
            );
        }

        [Fact]
        public async Task LoginAsync_WithLockedAccount_ShouldThrowValidationException()
        {
            // Arrange
            var username = "lockeduser";
            var user = new User
            {
                Id = 1,
                Username = username,
                PasswordHash = "hash",
                FailedLoginAttempts = 5,
                LockoutEnd = DateTime.UtcNow.AddMinutes(15)  // Bloqueado
            };

            _mockUserRepository
                .Setup(x => x.GetByUsernameAsync(username))
                .ReturnsAsync(user);

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(
                async () => await _authService.LoginAsync(username, "anypassword")
            );

            _mockUserRepository.Verify(x => x.UpdateUserAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task LoginAsync_MaxFailedAttempts_ShouldLockAccount()
        {
            // Arrange
            var username = "testuser";
            var correctPassword = "CorrectPassword123!";
            var wrongPassword = "WrongPassword123!";
            var passwordHash = _authService.HashPassword(correctPassword);

            var user = new User
            {
                Id = 1,
                Username = username,
                PasswordHash = passwordHash,
                FailedLoginAttempts = 4,  // Penúltimo intento
                LockoutEnd = null
            };

            _mockUserRepository
                .Setup(x => x.GetByUsernameAsync(username))
                .ReturnsAsync(user);

            _mockUserRepository
                .Setup(x => x.UpdateUserAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask);

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(
                async () => await _authService.LoginAsync(username, wrongPassword)
            );

            // Verify que bloqueó la cuenta
            _mockUserRepository.Verify(
                x => x.UpdateUserAsync(It.Is<User>(u =>
                    u.FailedLoginAttempts == 5 &&
                    u.LockoutEnd.HasValue
                )),
                Times.Once
            );
        }

        #endregion

        #region Pruebas de Utilidades

        [Fact]
        public void VerifyPassword_WithCorrectPassword_ShouldReturnTrue()
        {
            // Arrange
            var password = "TestPassword123!";
            var hash = _authService.HashPassword(password);

            // Act
            var result = _authService.VerifyPassword(password, hash);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void VerifyPassword_WithWrongPassword_ShouldReturnFalse()
        {
            // Arrange
            var correctPassword = "CorrectPassword123!";
            var wrongPassword = "WrongPassword123!";
            var hash = _authService.HashPassword(correctPassword);

            // Act
            var result = _authService.VerifyPassword(wrongPassword, hash);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void HashPassword_ShouldProduceDifferentHashesForSamePassword()
        {
            // Arrange
            var password = "TestPassword123!";

            // Act
            var hash1 = _authService.HashPassword(password);
            var hash2 = _authService.HashPassword(password);

            // Assert
            Assert.NotEqual(hash1, hash2);
            Assert.True(_authService.VerifyPassword(password, hash1));
            Assert.True(_authService.VerifyPassword(password, hash2));
        }

        [Fact]
        public void HashPassword_ShouldProduceBCryptHash()
        {
            // Arrange
            var password = "TestPassword123!";

            // Act
            var hash = _authService.HashPassword(password);

            // Assert
            Assert.NotNull(hash);
            Assert.StartsWith("$2", hash); // BCrypt hashes start with $2
        }

        #endregion
    }
}
