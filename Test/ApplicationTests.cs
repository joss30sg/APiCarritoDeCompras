using Xunit;
using Moq;
using FluentAssertions;
using System.Threading.Tasks;
using System.Threading;
using ShoppingCartApi.Domain.Interfaces;
using ShoppingCartApi.Domain.Entities;
using ShoppingCartApi.Application.UseCases.GetShoppingCart;
using ShoppingCartApi.Application.UseCases.RemoveProductFromCart;
using ShoppingCartApi.Application.UseCases.UpdateProductQuantity;
using System;

namespace ShoppingCartApi.Tests
{
    public class ApplicationTests
    {
        private readonly Mock<IShoppingCartRepository> _mockShoppingCartRepository;
        private readonly Mock<IProductRepository> _mockProductRepository;

        public ApplicationTests()
        {
            _mockShoppingCartRepository = new Mock<IShoppingCartRepository>();
            _mockProductRepository = new Mock<IProductRepository>();
        }

        [Fact]
        public async Task GetShoppingCartQueryHandler_ShouldReturnExistingCart_WhenCartExists()
        {
            // Arrange
            var userId = 1;
            var existingCart = new ShoppingCart { Id = 1, UserId = userId };
            _mockShoppingCartRepository.Setup(repo => repo.GetCartByUserIdAsync(userId))
                .ReturnsAsync(existingCart);
            var handler = new GetShoppingCartQueryHandler(_mockShoppingCartRepository.Object);
            var query = new GetShoppingCartQuery { UserId = userId };

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().Be(existingCart);
            _mockShoppingCartRepository.Verify(repo => repo.GetCartByUserIdAsync(userId), Times.Once);
        }

        [Fact]
        public async Task GetShoppingCartQueryHandler_ShouldReturnEmptyCart_WhenCartDoesNotExist()
        {
            // Arrange
            var userId = 1;
            _mockShoppingCartRepository.Setup(repo => repo.GetCartByUserIdAsync(userId))
                .ReturnsAsync((ShoppingCart?)null);
            var handler = new GetShoppingCartQueryHandler(_mockShoppingCartRepository.Object);
            var query = new GetShoppingCartQuery { UserId = userId };

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.UserId.Should().Be(userId);
            result.Items.Should().BeEmpty();
            _mockShoppingCartRepository.Verify(repo => repo.GetCartByUserIdAsync(userId), Times.Once);
        }

        [Fact]
        public async Task RemoveProductFromCartCommandHandler_ShouldRemoveProduct_WhenCartAndProductExist()
        {
            // Arrange
            var userId = 1;
            var productId = 101;
            var cart = new ShoppingCart { Id = 1, UserId = userId };
            cart.AddItem(new Product { Id = productId, Name = "Test Product", Price = 10.0m }, 1);

            _mockShoppingCartRepository.Setup(repo => repo.GetCartByUserIdAsync(userId))
                .ReturnsAsync(cart);
            _mockShoppingCartRepository.Setup(repo => repo.SaveAsync(It.IsAny<ShoppingCart>()))
                .Returns(Task.CompletedTask);

            var handler = new RemoveProductFromCartCommandHandler(_mockShoppingCartRepository.Object);
            var command = new RemoveProductFromCartCommand { UserId = userId, ProductId = productId };

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            cart.Items.Should().BeEmpty();
            _mockShoppingCartRepository.Verify(repo => repo.GetCartByUserIdAsync(userId), Times.Once);
            _mockShoppingCartRepository.Verify(repo => repo.SaveAsync(cart), Times.Once);
        }

        [Fact]
        public async Task RemoveProductFromCartCommandHandler_ShouldDoNothing_WhenCartDoesNotExist()
        {
            // Arrange
            var userId = 1;
            var productId = 101;

            _mockShoppingCartRepository.Setup(repo => repo.GetCartByUserIdAsync(userId))
                .ReturnsAsync((ShoppingCart?)null);
            _mockShoppingCartRepository.Setup(repo => repo.SaveAsync(It.IsAny<ShoppingCart>()))
                .Returns(Task.CompletedTask);

            var handler = new RemoveProductFromCartCommandHandler(_mockShoppingCartRepository.Object);
            var command = new RemoveProductFromCartCommand { UserId = userId, ProductId = productId };

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            _mockShoppingCartRepository.Verify(repo => repo.GetCartByUserIdAsync(userId), Times.Once);
            _mockShoppingCartRepository.Verify(repo => repo.SaveAsync(It.IsAny<ShoppingCart>()), Times.Never);
        }

        [Fact]
        public async Task RemoveProductFromCartCommandHandler_ShouldDoNothing_WhenProductDoesNotExistInCart()
        {
            // Arrange
            var userId = 1;
            var productId = 101;
            var cart = new ShoppingCart { Id = 1, UserId = userId };
            cart.AddItem(new Product { Id = 999, Name = "Other Product", Price = 5.0m }, 1); // Add a different product

            _mockShoppingCartRepository.Setup(repo => repo.GetCartByUserIdAsync(userId))
                .ReturnsAsync(cart);
            _mockShoppingCartRepository.Setup(repo => repo.SaveAsync(It.IsAny<ShoppingCart>()))
                .Returns(Task.CompletedTask);

            var handler = new RemoveProductFromCartCommandHandler(_mockShoppingCartRepository.Object);
            var command = new RemoveProductFromCartCommand { UserId = userId, ProductId = productId };

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            cart.Items.Should().NotBeEmpty(); // Should still contain the other product
            cart.Items.Should().HaveCount(1);
            _mockShoppingCartRepository.Verify(repo => repo.GetCartByUserIdAsync(userId), Times.Once);
            _mockShoppingCartRepository.Verify(repo => repo.SaveAsync(cart), Times.Once); // Cart was modified (even if item not found, SaveAsync is called)
        }

        [Fact]
        public async Task UpdateProductQuantityCommandHandler_ShouldUpdateQuantity_WhenCartAndProductExist()
        {
            // Arrange
            var cartId = 1;
            var productId = 101;
            var newQuantity = 5;
            var cart = new ShoppingCart { Id = cartId, UserId = 1 };
            cart.AddItem(new Product { Id = productId, Name = "Test Product", Price = 10.0m }, 1);

            _mockShoppingCartRepository.Setup(repo => repo.GetByIdAsync(cartId))
                .ReturnsAsync(cart);
            _mockShoppingCartRepository.Setup(repo => repo.SaveAsync(It.IsAny<ShoppingCart>()))
                .Returns(Task.CompletedTask);

            var handler = new UpdateProductQuantityCommandHandler(_mockShoppingCartRepository.Object);
            var command = new UpdateProductQuantityCommand { CartId = cartId, ProductId = productId, Quantity = newQuantity };

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            cart.Items.Should().ContainSingle(item => item.ProductId == productId && item.Quantity == newQuantity);
            _mockShoppingCartRepository.Verify(repo => repo.GetByIdAsync(cartId), Times.Once);
            _mockShoppingCartRepository.Verify(repo => repo.SaveAsync(cart), Times.Once);
        }

        [Fact]
        public async Task UpdateProductQuantityCommandHandler_ShouldRemoveProduct_WhenQuantityIsZero()
        {
            // Arrange
            var cartId = 1;
            var productId = 101;
            var newQuantity = 0;
            var cart = new ShoppingCart { Id = cartId, UserId = 1 };
            cart.AddItem(new Product { Id = productId, Name = "Test Product", Price = 10.0m }, 1);

            _mockShoppingCartRepository.Setup(repo => repo.GetByIdAsync(cartId))
                .ReturnsAsync(cart);
            _mockShoppingCartRepository.Setup(repo => repo.SaveAsync(It.IsAny<ShoppingCart>()))
                .Returns(Task.CompletedTask);

            var handler = new UpdateProductQuantityCommandHandler(_mockShoppingCartRepository.Object);
            var command = new UpdateProductQuantityCommand { CartId = cartId, ProductId = productId, Quantity = newQuantity };

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            cart.Items.Should().BeEmpty();
            _mockShoppingCartRepository.Verify(repo => repo.GetByIdAsync(cartId), Times.Once);
            _mockShoppingCartRepository.Verify(repo => repo.SaveAsync(cart), Times.Once);
        }

        [Fact]
        public async Task UpdateProductQuantityCommandHandler_ShouldThrowException_WhenCartDoesNotExist()
        {
            // Arrange
            var cartId = 1;
            var productId = 101;
            var newQuantity = 5;

            _mockShoppingCartRepository.Setup(repo => repo.GetByIdAsync(cartId))
                .ReturnsAsync((ShoppingCart?)null);

            var handler = new UpdateProductQuantityCommandHandler(_mockShoppingCartRepository.Object);
            var command = new UpdateProductQuantityCommand { CartId = cartId, ProductId = productId, Quantity = newQuantity };

            // Act
            Func<Task> action = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await action.Should().ThrowAsync<Exception>()
                .WithMessage($"Shopping cart with ID {cartId} not found.");
            _mockShoppingCartRepository.Verify(repo => repo.GetByIdAsync(cartId), Times.Once);
            _mockShoppingCartRepository.Verify(repo => repo.SaveAsync(It.IsAny<ShoppingCart>()), Times.Never);
        }
    }
}
