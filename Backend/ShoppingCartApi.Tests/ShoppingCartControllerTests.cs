using Xunit;
using Moq;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ShoppingCartApi.Presentation.Controllers;
using ShoppingCartApi.Application.UseCases.AddProductToCart;
using ShoppingCartApi.Application.UseCases.UpdateProductQuantity;
using ShoppingCartApi.Application.UseCases.RemoveProductFromCart;
using ShoppingCartApi.Application.UseCases.GetShoppingCart;
using ShoppingCartApi.Domain.Entities;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using FluentAssertions;
using System;

namespace ShoppingCartApi.Tests
{
    public class ShoppingCartControllerTests
    {
        private readonly Mock<IMediator> _mockMediator;
        private readonly ShoppingCartController _controller;
        private readonly ClaimsPrincipal _authenticatedUser;
        private readonly ClaimsPrincipal _unauthenticatedUser;

        public ShoppingCartControllerTests()
        {
            _mockMediator = new Mock<IMediator>();
            _controller = new ShoppingCartController(_mockMediator.Object);

            // Configurar un usuario autenticado
            _authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1")
            }, "mock"));

            // Configurar un usuario no autenticado (sin NameIdentifier)
            _unauthenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim("some_other_claim", "value")
            }, "mock"));

            // Por defecto, usar el usuario autenticado para las pruebas
            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = _authenticatedUser }
            };
        }

        [Fact]
        public async Task AddProductToCart_ReturnsOk_WhenCommandIsValid()
        {
            // Arrange
            var command = new AddProductToCartCommand { CartId = 1, ProductId = 101, Quantity = 1 };
            _mockMediator.Setup(m => m.Send(command, default))
                .Returns(Task.FromResult(Unit.Value));

            // Act
            var result = await _controller.AddProductToCart(command);

            // Assert
            result.Should().BeOfType<OkResult>();
            _mockMediator.Verify(m => m.Send(command, default), Times.Once);
        }

        [Fact]
        public async Task AddProductToCart_ReturnsBadRequest_WhenMediatorThrowsException()
        {
            // Arrange
            var command = new AddProductToCartCommand { CartId = 1, ProductId = 101, Quantity = 1 };
            _mockMediator.Setup(m => m.Send(command, default))
                .ThrowsAsync(new Exception("Validation failed"));

            // Act
            Func<Task> action = async () => await _controller.AddProductToCart(command);

            // Assert
            await action.Should().ThrowAsync<Exception>().WithMessage("Validation failed");
            _mockMediator.Verify(m => m.Send(command, default), Times.Once);
        }

        [Fact]
        public async Task UpdateProductQuantity_ReturnsOk_WhenCommandIsValid()
        {
            // Arrange
            var command = new UpdateProductQuantityCommand { CartId = 1, ProductId = 101, Quantity = 5 };
            _mockMediator.Setup(m => m.Send(command, default))
                .Returns(Task.FromResult(Unit.Value));

            // Act
            var result = await _controller.UpdateProductQuantity(command);

            // Assert
            result.Should().BeOfType<OkResult>();
            _mockMediator.Verify(m => m.Send(command, default), Times.Once);
        }

        [Fact]
        public async Task UpdateProductQuantity_ReturnsBadRequest_WhenMediatorThrowsException()
        {
            // Arrange
            var command = new UpdateProductQuantityCommand { CartId = 1, ProductId = 101, Quantity = 5 };
            _mockMediator.Setup(m => m.Send(command, default))
                .ThrowsAsync(new Exception("Cart not found"));

            // Act
            Func<Task> action = async () => await _controller.UpdateProductQuantity(command);

            // Assert
            await action.Should().ThrowAsync<Exception>().WithMessage("Cart not found");
            _mockMediator.Verify(m => m.Send(command, default), Times.Once);
        }

        [Fact]
        public async Task RemoveProductFromCart_ReturnsOk_WhenProductIsRemoved()
        {
            // Arrange
            var productId = 1;
            _mockMediator.Setup(m => m.Send(It.IsAny<RemoveProductFromCartCommand>(), default))
                .Returns(Task.FromResult(Unit.Value));

            // Act
            var result = await _controller.RemoveProductFromCart(productId);

            // Assert
            result.Should().BeOfType<OkResult>();
            _mockMediator.Verify(m => m.Send(It.Is<RemoveProductFromCartCommand>(c => c.UserId == 1 && c.ProductId == productId), default), Times.Once);
        }

        [Fact]
        public async Task RemoveProductFromCart_ReturnsUnauthorized_WhenUserIdIsInvalid()
        {
            // Arrange
            _controller.ControllerContext.HttpContext.User = _unauthenticatedUser;
            var productId = 1;

            // Act
            var result = await _controller.RemoveProductFromCart(productId);

            // Assert
            result.Should().BeOfType<UnauthorizedObjectResult>()
                .Which.Value.Should().Be("User ID not found or invalid.");
            _mockMediator.Verify(m => m.Send(It.IsAny<RemoveProductFromCartCommand>(), default), Times.Never);
        }

        [Fact]
        public async Task GetShoppingCart_ReturnsShoppingCart_WhenCartExists()
        {
            // Arrange
            var expectedCart = new ShoppingCart { Id = 1, UserId = 1 };
            _mockMediator.Setup(m => m.Send(It.IsAny<GetShoppingCartQuery>(), default))
                .ReturnsAsync(expectedCart);

            // Act
            var result = await _controller.GetShoppingCart();

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().Be(expectedCart);
            _mockMediator.Verify(m => m.Send(It.Is<GetShoppingCartQuery>(q => q.UserId == 1), default), Times.Once);
        }

        [Fact]
        public async Task GetShoppingCart_ReturnsUnauthorized_WhenUserIdIsInvalid()
        {
            // Arrange
            _controller.ControllerContext.HttpContext.User = _unauthenticatedUser;

            // Act
            var result = await _controller.GetShoppingCart();

            // Assert
            result.Result.Should().BeOfType<UnauthorizedObjectResult>()
                .Which.Value.Should().Be("User ID not found or invalid.");
            _mockMediator.Verify(m => m.Send(It.IsAny<GetShoppingCartQuery>(), default), Times.Never);
        }
    }
}
