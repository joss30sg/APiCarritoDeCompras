using Xunit;
using FluentAssertions;
using System.Threading.Tasks;
using ShoppingCartApi.Infrastructure.Repositories;
using ShoppingCartApi.Domain.Entities;
using System.Linq;
using System.Collections.Generic;

namespace ShoppingCartApi.Tests
{
    public class InfrastructureTests
    {
        [Fact]
        public async Task InMemoryShoppingCartRepository_GetByIdAsync_ShouldReturnCart_WhenExists()
        {
            // Arrange
            var repository = new InMemoryShoppingCartRepository();
            var cart = new ShoppingCart { Id = 1, UserId = 101 };
            await repository.SaveAsync(cart);

            // Act
            var result = await repository.GetByIdAsync(1);

            // Assert
            result.Should().Be(cart);
        }

        [Fact]
        public async Task InMemoryShoppingCartRepository_GetByIdAsync_ShouldReturnNull_WhenDoesNotExist()
        {
            // Arrange
            var repository = new InMemoryShoppingCartRepository();

            // Act
            var result = await repository.GetByIdAsync(999);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task InMemoryShoppingCartRepository_SaveAsync_ShouldAddOrUpdateCart()
        {
            // Arrange
            var repository = new InMemoryShoppingCartRepository();
            var cart = new ShoppingCart { Id = 1, UserId = 101 };

            // Act
            await repository.SaveAsync(cart);
            var retrievedCart = await repository.GetByIdAsync(1);

            // Assert
            retrievedCart.Should().Be(cart);

            // Update
            cart.AddItem(new Product { Id = 10, Name = "Product 1", Price = 10.0m }, 1);
            await repository.SaveAsync(cart);
            retrievedCart = await repository.GetByIdAsync(1);
            retrievedCart!.Items.Should().HaveCount(1);
        }

        [Fact]
        public async Task InMemoryShoppingCartRepository_RemoveProductAsync_ShouldRemoveProduct_WhenExists()
        {
            // Arrange
            var repository = new InMemoryShoppingCartRepository();
            var cart = new ShoppingCart { Id = 1, UserId = 101 };
            cart.AddItem(new Product { Id = 10, Name = "Product 1", Price = 10.0m }, 1);
            await repository.SaveAsync(cart);

            // Act
            await repository.RemoveProductAsync(1, 10);

            // Assert
            var updatedCart = await repository.GetByIdAsync(1);
            updatedCart!.Items.Should().BeEmpty();
        }

        [Fact]
        public async Task InMemoryShoppingCartRepository_RemoveProductAsync_ShouldDoNothing_WhenCartDoesNotExist()
        {
            // Arrange
            var repository = new InMemoryShoppingCartRepository();

            // Act
            await repository.RemoveProductAsync(999, 10);

            // Assert (no exception, no change)
            var result = await repository.GetByIdAsync(999);
            result.Should().BeNull();
        }

        [Fact]
        public async Task InMemoryShoppingCartRepository_RemoveProductAsync_ShouldDoNothing_WhenProductDoesNotExist()
        {
            // Arrange
            var repository = new InMemoryShoppingCartRepository();
            var cart = new ShoppingCart { Id = 1, UserId = 101 };
            cart.AddItem(new Product { Id = 10, Name = "Product 1", Price = 10.0m }, 1);
            await repository.SaveAsync(cart);

            // Act
            await repository.RemoveProductAsync(1, 999);

            // Assert
            var updatedCart = await repository.GetByIdAsync(1);
            updatedCart!.Items.Should().ContainSingle();
            updatedCart.Items.First().ProductId.Should().Be(10);
        }

        [Fact]
        public async Task InMemoryShoppingCartRepository_GetCartByUserIdAsync_ShouldReturnCart_WhenExists()
        {
            // Arrange
            var repository = new InMemoryShoppingCartRepository();
            var cart = new ShoppingCart { Id = 1, UserId = 101 };
            await repository.SaveAsync(cart);

            // Act
            var result = await repository.GetCartByUserIdAsync(101);

            // Assert
            result.Should().Be(cart);
        }

        [Fact]
        public async Task InMemoryShoppingCartRepository_GetCartByUserIdAsync_ShouldReturnNull_WhenDoesNotExist()
        {
            // Arrange
            var repository = new InMemoryShoppingCartRepository();

            // Act
            var result = await repository.GetCartByUserIdAsync(999);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task InMemoryProductRepository_GetByIdAsync_ShouldReturnProduct_WhenExists()
        {
            // Arrange
            var repository = new InMemoryProductRepository();
            // The InMemoryProductRepository is initialized with a predefined list of products.
            // We need to ensure that a product with ID 1 exists in that list for this test.
            // If the repository's internal state is not exposed, we can't directly add a product.
            // Assuming the default constructor populates it with a product having ID 1.
            // Let's verify this assumption by checking the existing products.
            var existingProduct = await repository.GetByIdAsync(1);
            existingProduct.Should().NotBeNull("Product with ID 1 should exist in the default InMemoryProductRepository.");
            // Usar el operador de perdón de nulos '!' ya que la aserción anterior garantiza que no es nulo.
            existingProduct!.Should().NotBeNull();

            // Act
            var result = await repository.GetByIdAsync(1);

            // Assert
            result.Should().Be(existingProduct);
        }

        [Fact]
        public async Task InMemoryProductRepository_GetByIdAsync_ShouldReturnNull_WhenDoesNotExist()
        {
            // Arrange
            var repository = new InMemoryProductRepository();

            // Act
            var result = await repository.GetByIdAsync(999);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task InMemoryProductRepository_GetAllAsync_ShouldReturnAllProducts()
        {
            // Arrange
            var repository = new InMemoryProductRepository();

            // Act
            var result = await repository.GetAllAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().NotBeEmpty();
            // Assuming there are at least 3 products initialized in the repository
            result.Should().HaveCount(c => c >= 3);
        }
    }
}
