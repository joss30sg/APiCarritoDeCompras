using Xunit;
using FluentAssertions;
using ShoppingCartApi.Domain.Entities;
using System.Linq;
using System.Collections.Generic;

namespace ShoppingCartApi.Tests
{
    public class DomainTests
    {
        [Fact]
        public void ShoppingCart_AddItem_ShouldAddNewItem_WhenProductIsNotInCart()
        {
            // Arrange
            var cart = new ShoppingCart { Id = 1, UserId = 1 };
            var product = new Product { Id = 101, Name = "Product A", Price = 10.0m };
            var quantity = 2;

            // Act
            cart.AddItem(product, quantity);

            // Assert
            cart.Items.Should().ContainSingle();
            cart.Items.First().ProductId.Should().Be(product.Id);
            cart.Items.First().Quantity.Should().Be(quantity);
            cart.Items.First().Product.Should().Be(product);
        }

        [Fact]
        public void ShoppingCart_AddItem_ShouldIncreaseQuantity_WhenProductIsInCart()
        {
            // Arrange
            var cart = new ShoppingCart { Id = 1, UserId = 1 };
            var product = new Product { Id = 101, Name = "Product A", Price = 10.0m };
            cart.AddItem(product, 2);

            // Act
            cart.AddItem(product, 3);

            // Assert
            cart.Items.Should().ContainSingle();
            cart.Items.First().ProductId.Should().Be(product.Id);
            cart.Items.First().Quantity.Should().Be(5);
        }

        [Fact]
        public void ShoppingCart_UpdateItemQuantity_ShouldUpdateQuantity_WhenProductExists()
        {
            // Arrange
            var cart = new ShoppingCart { Id = 1, UserId = 1 };
            var product = new Product { Id = 101, Name = "Product A", Price = 10.0m };
            cart.AddItem(product, 2);
            var newQuantity = 5;

            // Act
            cart.UpdateItemQuantity(product.Id, newQuantity);

            // Assert
            cart.Items.Should().ContainSingle();
            cart.Items.First().ProductId.Should().Be(product.Id);
            cart.Items.First().Quantity.Should().Be(newQuantity);
        }

        [Fact]
        public void ShoppingCart_UpdateItemQuantity_ShouldRemoveItem_WhenQuantityIsZeroOrLess()
        {
            // Arrange
            var cart = new ShoppingCart { Id = 1, UserId = 1 };
            var product = new Product { Id = 101, Name = "Product A", Price = 10.0m };
            cart.AddItem(product, 2);
            var newQuantity = 0;

            // Act
            cart.UpdateItemQuantity(product.Id, newQuantity);

            // Assert
            cart.Items.Should().BeEmpty();
        }

        [Fact]
        public void ShoppingCart_UpdateItemQuantity_ShouldDoNothing_WhenProductDoesNotExist()
        {
            // Arrange
            var cart = new ShoppingCart { Id = 1, UserId = 1 };
            var product = new Product { Id = 101, Name = "Product A", Price = 10.0m };
            cart.AddItem(product, 2);
            var newQuantity = 5;

            // Act
            cart.UpdateItemQuantity(999, newQuantity); // Update a non-existent product

            // Assert
            cart.Items.Should().ContainSingle();
            cart.Items.First().ProductId.Should().Be(product.Id);
            cart.Items.First().Quantity.Should().Be(2); // Quantity should remain unchanged
        }

        [Fact]
        public void ShoppingCart_RemoveItem_ShouldRemoveProduct_WhenProductExists()
        {
            // Arrange
            var cart = new ShoppingCart { Id = 1, UserId = 1 };
            var product1 = new Product { Id = 101, Name = "Product A", Price = 10.0m };
            var product2 = new Product { Id = 102, Name = "Product B", Price = 20.0m };
            cart.AddItem(product1, 1);
            cart.AddItem(product2, 1);

            // Act
            cart.RemoveItem(product1.Id);

            // Assert
            cart.Items.Should().ContainSingle();
            cart.Items.First().ProductId.Should().Be(product2.Id);
        }

        [Fact]
        public void ShoppingCart_RemoveItem_ShouldDoNothing_WhenProductDoesNotExist()
        {
            // Arrange
            var cart = new ShoppingCart { Id = 1, UserId = 1 };
            var product1 = new Product { Id = 101, Name = "Product A", Price = 10.0m };
            cart.AddItem(product1, 1);

            // Act
            cart.RemoveItem(999); // Remove non-existent product

            // Assert
            cart.Items.Should().ContainSingle();
            cart.Items.First().ProductId.Should().Be(product1.Id);
        }

        [Fact]
        public void ShoppingCart_GetTotalPrice_ShouldCalculateCorrectPrice()
        {
            // Arrange
            var cart = new ShoppingCart { Id = 1, UserId = 1 };
            var product1 = new Product { Id = 101, Name = "Product A", Price = 10.0m };
            var product2 = new Product { Id = 102, Name = "Product B", Price = 20.0m };
            cart.AddItem(product1, 2); // 2 * 10.0 = 20.0
            cart.AddItem(product2, 1); // 1 * 20.0 = 20.0

            // Act
            var totalPrice = cart.GetTotalPrice();

            // Assert
            totalPrice.Should().Be(40.0m);
        }

        [Fact]
        public void ShoppingCart_GetTotalPrice_ShouldReturnZero_WhenCartIsEmpty()
        {
            // Arrange
            var cart = new ShoppingCart { Id = 1, UserId = 1 };

            // Act
            var totalPrice = cart.GetTotalPrice();

            // Assert
            totalPrice.Should().Be(0.0m);
        }

        [Fact]
        public void Attribute_Properties_ShouldBeSetCorrectly()
        {
            // Arrange
            var attribute = new ShoppingCartApi.Domain.Entities.Attribute
            {
                ProductId = 1,
                AttributeId = 101,
                Name = "Color",
                DefaultQuantity = 1,
                MaxQuantity = 5,
                PriceImpactAmount = 2,
                IsRequired = true,
                NegativeAttributeId = "202",
                Order = 1,
                StatusId = "Active",
                UrlImage = "http://example.com/image.png"
            };

            // Assert
            attribute.ProductId.Should().Be(1);
            attribute.AttributeId.Should().Be(101);
            attribute.Name.Should().Be("Color");
            attribute.DefaultQuantity.Should().Be(1);
            attribute.MaxQuantity.Should().Be(5);
            attribute.PriceImpactAmount.Should().Be(2);
            attribute.IsRequired.Should().BeTrue();
            attribute.NegativeAttributeId.Should().Be("202");
            attribute.Order.Should().Be(1);
            attribute.StatusId.Should().Be("Active");
            attribute.UrlImage.Should().Be("http://example.com/image.png");
        }

        [Fact]
        public void QuantityInformation_Properties_ShouldBeSetCorrectly()
        {
            // Arrange
            var quantityInfo = new QuantityInformation
            {
                GroupAttributeQuantity = 1,
                ShowPricePerProduct = true,
                IsShown = true,
                IsEditable = true,
                IsVerified = true,
                VerifyValue = "EQUAL_THAN"
            };

            // Assert
            quantityInfo.GroupAttributeQuantity.Should().Be(1);
            quantityInfo.ShowPricePerProduct.Should().BeTrue();
            quantityInfo.IsShown.Should().BeTrue();
            quantityInfo.IsEditable.Should().BeTrue();
            quantityInfo.IsVerified.Should().BeTrue();
            quantityInfo.VerifyValue.Should().Be("EQUAL_THAN");
        }
    }
}
