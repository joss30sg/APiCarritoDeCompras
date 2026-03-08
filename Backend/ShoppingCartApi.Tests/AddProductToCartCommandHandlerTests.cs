using Xunit;
using Moq;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ShoppingCartApi.Application.UseCases.AddProductToCart;
using ShoppingCartApi.Domain.Entities;
using ShoppingCartApi.Domain.Interfaces;

namespace ShoppingCartApi.Tests
{
    public class AddProductToCartCommandHandlerTests
    {
        private readonly Mock<IShoppingCartRepository> _mockShoppingCartRepository;
        private readonly Mock<IProductRepository> _mockProductRepository;
        private readonly AddProductToCartCommandHandler _handler;

        public AddProductToCartCommandHandlerTests()
        {
            _mockShoppingCartRepository = new Mock<IShoppingCartRepository>();
            _mockProductRepository = new Mock<IProductRepository>();
            _handler = new AddProductToCartCommandHandler(_mockShoppingCartRepository.Object, _mockProductRepository.Object);
        }

        private Product CreateSampleProduct(bool withAttributes = true, bool isGroupRequired = false, string? verifyValue = null, int groupAttributeQuantity = 0)
        {
            var product = new Product
            {
                Id = 1,
                Name = "Test Product",
                Price = 10.0m
            };

            if (withAttributes)
            {
                var groupAttributeType = new GroupAttributeType
                {
                    GroupAttributeTypeId = "1",
                    Name = "Sample Group Type",
                    IsRequired = isGroupRequired
                };

                var quantityInformation = new QuantityInformation
                {
                    GroupAttributeQuantity = groupAttributeQuantity,
                    VerifyValue = verifyValue
                };

                var attribute1 = new Domain.Entities.Attribute
                {
                    ProductId = 1,
                    AttributeId = 101,
                    Name = "Attribute 1",
                    DefaultQuantity = 0,
                    MaxQuantity = 5,
                    PriceImpactAmount = 1,
                    StatusId = "A" // Required member
                };

                var attribute2 = new Domain.Entities.Attribute
                {
                    ProductId = 1,
                    AttributeId = 102,
                    Name = "Attribute 2",
                    DefaultQuantity = 0,
                    MaxQuantity = 5,
                    PriceImpactAmount = 2,
                    StatusId = "A" // Required member
                };

                var productAttributeGroup = new ProductAttributeGroup
                {
                    GroupAttributeId = "G1",
                    GroupAttributeType = groupAttributeType,
                    Description = "Sample Group", // Required member
                    QuantityInformation = quantityInformation,
                    Attributes = new List<Domain.Entities.Attribute> { attribute1, attribute2 },
                    Order = 1
                };
                product.AttributeGroups = new List<ProductAttributeGroup> { productAttributeGroup };
            }
            return product;
        }

        private AddProductToCartCommand CreateAddProductToCartCommand(int productId, int quantity, List<ProductAttributeGroup>? selectedAttributeGroups = null, int cartId = 1)
        {
            return new AddProductToCartCommand
            {
                CartId = cartId,
                ProductId = productId,
                Quantity = quantity,
                SelectedAttributeGroups = selectedAttributeGroups
            };
        }

        [Fact]
        public async Task Handle_ShouldCreateNewCart_WhenCartDoesNotExist()
        {
            // Arrange
            var productId = 1;
            var cartId = 1;
            var command = CreateAddProductToCartCommand(productId, 1, cartId: cartId);
            var product = CreateSampleProduct(false);

            _mockShoppingCartRepository.Setup(r => r.GetByIdAsync(command.CartId)).ReturnsAsync((ShoppingCart?)null);
            _mockProductRepository.Setup(r => r.GetByIdAsync(productId)).ReturnsAsync(product);
            _mockShoppingCartRepository.Setup(r => r.SaveAsync(It.IsAny<ShoppingCart>())).Returns(Task.CompletedTask);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mockShoppingCartRepository.Verify(r => r.SaveAsync(It.Is<ShoppingCart>(cart => cart.Id == command.CartId && cart.Items.Any(item => item.ProductId == productId))), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldAddProductToExistingCart_WhenCartExists()
        {
            // Arrange
            var productId = 1;
            var cartId = 1;
            var existingCart = new ShoppingCart { Id = cartId };
            var command = CreateAddProductToCartCommand(productId, 1, cartId: cartId);
            var product = CreateSampleProduct(false);

            _mockShoppingCartRepository.Setup(r => r.GetByIdAsync(command.CartId)).ReturnsAsync(existingCart);
            _mockProductRepository.Setup(r => r.GetByIdAsync(productId)).ReturnsAsync(product);
            _mockShoppingCartRepository.Setup(r => r.SaveAsync(It.IsAny<ShoppingCart>())).Returns(Task.CompletedTask);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mockShoppingCartRepository.Verify(r => r.SaveAsync(It.Is<ShoppingCart>(cart => cart.Id == command.CartId && cart.Items.Any(item => item.ProductId == productId))), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenProductNotFound()
        {
            // Arrange
            var command = CreateAddProductToCartCommand(999, 1);
            _mockProductRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Product?)null);

            // Act
            Func<Task> action = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await action.Should().ThrowAsync<Exception>().WithMessage("Product with ID 999 not found.");
        }

        [Fact]
        public async Task Handle_ShouldAddProductToCartSuccessfully_WhenNoAttributesAreProvidedAndProductDoesNotAdmitThem()
        {
            // Arrange
            var productId = 1;
            var command = CreateAddProductToCartCommand(productId, 1, selectedAttributeGroups: null);
            var product = CreateSampleProduct(withAttributes: false); // Product does not admit attributes

            _mockShoppingCartRepository.Setup(r => r.GetByIdAsync(command.CartId)).ReturnsAsync((ShoppingCart?)null);
            _mockProductRepository.Setup(r => r.GetByIdAsync(productId)).ReturnsAsync(product);
            _mockShoppingCartRepository.Setup(r => r.SaveAsync(It.IsAny<ShoppingCart>())).Returns(Task.CompletedTask);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mockShoppingCartRepository.Verify(r => r.SaveAsync(It.IsAny<ShoppingCart>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenProductDoesNotAdmitAttributesButGroupsAreProvided()
        {
            // Arrange
            var productId = 1;
            var product = CreateSampleProduct(withAttributes: false);
            var selectedGroups = new List<ProductAttributeGroup>
            {
                new ProductAttributeGroup { GroupAttributeId = "G1", QuantityInformation = new QuantityInformation(), GroupAttributeType = new GroupAttributeType { GroupAttributeTypeId = "1", Name = "Test" }, Description = "Test Description" }
            };
            var command = CreateAddProductToCartCommand(productId, 1, selectedGroups);

            _mockProductRepository.Setup(r => r.GetByIdAsync(productId)).ReturnsAsync(product);

            // Act
            Func<Task> action = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await action.Should().ThrowAsync<Exception>().WithMessage("El producto no admite grupos de atributos, pero se proporcionaron grupos.");
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenRequiredGroupIsMissing()
        {
            // Arrange
            var productId = 1;
            var product = CreateSampleProduct(withAttributes: true, isGroupRequired: true); // Product has a required group
            var command = CreateAddProductToCartCommand(productId, 1, new List<ProductAttributeGroup>()); // No groups selected

            _mockProductRepository.Setup(r => r.GetByIdAsync(productId)).ReturnsAsync(product);

            // Act
            Func<Task> action = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await action.Should().ThrowAsync<Exception>().WithMessage($"El grupo de atributos obligatorio '{product.AttributeGroups!.First().GroupAttributeType.Name}' está ausente.");
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenSelectedGroupNotFoundInProduct()
        {
            // Arrange
            var productId = 1;
            var product = CreateSampleProduct(withAttributes: true);
            var selectedGroups = new List<ProductAttributeGroup>
            {
                new ProductAttributeGroup { GroupAttributeId = "G_UNKNOWN", QuantityInformation = new QuantityInformation(), GroupAttributeType = new GroupAttributeType { GroupAttributeTypeId = "2", Name = "Unknown" }, Description = "Unknown Description" }
            };
            var command = CreateAddProductToCartCommand(productId, 1, selectedGroups);

            _mockProductRepository.Setup(r => r.GetByIdAsync(productId)).ReturnsAsync(product);

            // Act
            Func<Task> action = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await action.Should().ThrowAsync<Exception>().WithMessage($"El grupo de atributos con ID 'G_UNKNOWN' no se encontró para el producto {productId}.");
        }

        [Theory]
        [InlineData("EQUAL_THAN", 2, 1, "se deben seleccionar exactamente 2 atributos. Se seleccionaron 1.")] // Less than required
        [InlineData("EQUAL_THAN", 2, 3, "se deben seleccionar exactamente 2 atributos. Se seleccionaron 3.")] // More than required
        [InlineData("LOWER_EQUAL_THAN", 2, 3, "se pueden seleccionar como máximo 2 atributos. Se seleccionaron 3.")] // More than allowed
        public async Task Handle_ShouldThrowException_WhenGroupAttributeQuantityIsInvalid(string verifyValue, int requiredQuantity, int selectedCount, string expectedMessagePart)
        {
            // Arrange
            var productId = 1;
            var product = CreateSampleProduct(withAttributes: true, verifyValue: verifyValue, groupAttributeQuantity: requiredQuantity);

            var selectedAttributes = new List<Domain.Entities.Attribute>();
            for (int i = 0; i < selectedCount; i++)
            {
                selectedAttributes.Add(new Domain.Entities.Attribute { AttributeId = 1000 + i, Name = $"Attr {i}", StatusId = "A" });
            }

            var selectedGroups = new List<ProductAttributeGroup>
            {
                new ProductAttributeGroup
                {
                    GroupAttributeId = "G1",
                    GroupAttributeType = product.AttributeGroups!.First().GroupAttributeType,
                    Description = "Sample Group",
                    QuantityInformation = new QuantityInformation { GroupAttributeQuantity = requiredQuantity, VerifyValue = verifyValue },
                    Attributes = selectedAttributes
                }
            };
            var command = CreateAddProductToCartCommand(productId, 1, selectedGroups);

            _mockProductRepository.Setup(r => r.GetByIdAsync(productId)).ReturnsAsync(product);

            // Act
            Func<Task> action = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await action.Should().ThrowAsync<Exception>().WithMessage($"*{expectedMessagePart}*");
        }

        [Fact]
        public async Task Handle_ShouldAddProductToCartSuccessfully_WhenVerifyValueIsNotEqualThanOrLowerEqualThan()
        {
            // Arrange
            var productId = 1;
            var product = CreateSampleProduct(withAttributes: true, verifyValue: "OTHER_VALUE", groupAttributeQuantity: 2); // VerifyValue is not EQUAL_THAN or LOWER_EQUAL_THAN

            var selectedAttributes = new List<Domain.Entities.Attribute>
            {
                new Domain.Entities.Attribute { AttributeId = 101, Name = "Attribute 1", StatusId = "A" }
            };

#pragma warning disable CS8601 // Posible asignación de referencia nula
            var selectedGroups = new List<ProductAttributeGroup>
            {
                new ProductAttributeGroup
                {
                    GroupAttributeId = "G1",
                    GroupAttributeType = product.AttributeGroups?.FirstOrDefault()?.GroupAttributeType,
                    Description = "Sample Group",
                    QuantityInformation = new QuantityInformation { GroupAttributeQuantity = 2, VerifyValue = "OTHER_VALUE" },
                    Attributes = selectedAttributes
                }
            };
#pragma warning restore CS8601 // Posible asignación de referencia nula
            var command = CreateAddProductToCartCommand(productId, 1, selectedGroups);

            _mockShoppingCartRepository.Setup(r => r.GetByIdAsync(command.CartId)).ReturnsAsync((ShoppingCart?)null);
            _mockProductRepository.Setup(r => r.GetByIdAsync(productId)).ReturnsAsync(product);
            _mockShoppingCartRepository.Setup(r => r.SaveAsync(It.IsAny<ShoppingCart>())).Returns(Task.CompletedTask);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mockShoppingCartRepository.Verify(r => r.SaveAsync(It.IsAny<ShoppingCart>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenSelectedAttributeNotFoundInProductGroup()
        {
            // Arrange
            var productId = 1;
            var product = CreateSampleProduct(withAttributes: true);
#pragma warning disable CS8601 // Posible asignación de referencia nula
            var selectedGroups = new List<ProductAttributeGroup>
            {
                new ProductAttributeGroup
                {
                    GroupAttributeId = "G1",
                    GroupAttributeType = product.AttributeGroups?.FirstOrDefault()?.GroupAttributeType,
                    Description = "Sample Group",
                    QuantityInformation = product.AttributeGroups?.FirstOrDefault()?.QuantityInformation,
                    Attributes = new List<Domain.Entities.Attribute> { new Domain.Entities.Attribute { AttributeId = 999, Name = "Unknown Attribute", StatusId = "A" } }
                }
            };
#pragma warning restore CS8601 // Posible asignación de referencia nula
            var command = CreateAddProductToCartCommand(productId, 1, selectedGroups);

            _mockProductRepository.Setup(r => r.GetByIdAsync(productId)).ReturnsAsync(product);

            // Act
            Func<Task> action = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await action.Should().ThrowAsync<Exception>().WithMessage($"El atributo con ID '999' no se encontró en el grupo 'G1'.");
        }

        [Theory]
        [InlineData(0, 6, "La cantidad por defecto debe ser al menos 0 y la máxima no debe exceder 5.")] // MaxQuantity too high
        [InlineData(-1, 5, "La cantidad por defecto debe ser al menos 0 y la máxima no debe exceder 5.")] // DefaultQuantity too low
        [InlineData(2, 1, "La cantidad por defecto debe ser al menos 0 y la máxima no debe exceder 5.")] // DefaultQuantity > MaxQuantity
        public async Task Handle_ShouldThrowException_WhenIndividualAttributeQuantityIsInvalid(int defaultQuantity, int maxQuantity, string expectedMessagePart)
        {
            // Arrange
            var productId = 1;
            var product = CreateSampleProduct(withAttributes: true, verifyValue: "LOWER_EQUAL_THAN", groupAttributeQuantity: 2);
#pragma warning disable CS8601 // Posible asignación de referencia nula
            var selectedGroups = new List<ProductAttributeGroup>
            {
                new ProductAttributeGroup
                {
                    GroupAttributeId = "G1",
                    GroupAttributeType = product.AttributeGroups?.FirstOrDefault()?.GroupAttributeType,
                    Description = "Sample Group",
                    QuantityInformation = product.AttributeGroups?.FirstOrDefault()?.QuantityInformation,
                    Attributes = new List<Domain.Entities.Attribute>
                    {
                        new Domain.Entities.Attribute
                        {
                            AttributeId = 101,
                            Name = "Attribute 1",
                            DefaultQuantity = defaultQuantity,
                            MaxQuantity = maxQuantity,
                            StatusId = "A"
                        }
                    }
                }
            };
#pragma warning restore CS8601 // Posible asignación de referencia nula
            var command = CreateAddProductToCartCommand(productId, 1, selectedGroups);

            _mockProductRepository.Setup(r => r.GetByIdAsync(productId)).ReturnsAsync(product);

            // Act
            Func<Task> action = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await action.Should().ThrowAsync<Exception>().WithMessage($"*{expectedMessagePart}*");
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenGroupDoesNotAdmitAttributesButAttributesAreProvided()
        {
            // Arrange
            var productId = 1;
            var product = CreateSampleProduct(withAttributes: true);
            product.AttributeGroups!.First().Attributes = null; // Simulate group not admitting attributes

#pragma warning disable CS8604 // Posible argumento de referencia nulo
            var selectedGroups = new List<ProductAttributeGroup>
            {
                new ProductAttributeGroup
                {
                    GroupAttributeId = "G1",
                    GroupAttributeType = product.AttributeGroups.First().GroupAttributeType,
                    Description = "Sample Group",
                    QuantityInformation = product.AttributeGroups.First().QuantityInformation,
                    Attributes = new List<Domain.Entities.Attribute> { new Domain.Entities.Attribute { AttributeId = 101, Name = "Attribute 1", StatusId = "A" } }
                }
            };
#pragma warning restore CS8604 // Posible argumento de referencia nulo
            var command = CreateAddProductToCartCommand(productId, 1, selectedGroups);

            _mockProductRepository.Setup(r => r.GetByIdAsync(productId)).ReturnsAsync(product);

            // Act
            Func<Task> action = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await action.Should().ThrowAsync<Exception>().WithMessage($"El grupo 'G1' no admite atributos, pero se proporcionaron atributos.");
        }

        [Fact]
        public async Task Handle_ShouldAddProductToCartSuccessfully_WhenAllValidationsPass()
        {
            // Arrange
            var productId = 1;
            var command = CreateAddProductToCartCommand(productId, 1);
            var product = CreateSampleProduct(withAttributes: true, isGroupRequired: true, verifyValue: "EQUAL_THAN", groupAttributeQuantity: 2);

            var selectedAttributes = new List<Domain.Entities.Attribute>
            {
                new Domain.Entities.Attribute { AttributeId = 101, Name = "Attribute 1", DefaultQuantity = 0, MaxQuantity = 5, StatusId = "A" },
                new Domain.Entities.Attribute { AttributeId = 102, Name = "Attribute 2", DefaultQuantity = 0, MaxQuantity = 5, StatusId = "A" }
            };

            var selectedGroups = new List<ProductAttributeGroup>
            {
                new ProductAttributeGroup
                {
                    GroupAttributeId = "G1",
                    GroupAttributeType = product.AttributeGroups!.First().GroupAttributeType,
                    Description = "Sample Group",
                    QuantityInformation = new QuantityInformation { GroupAttributeQuantity = 2, VerifyValue = "EQUAL_THAN" },
                    Attributes = selectedAttributes,
                    Order = 1
                }
            };
            command.SelectedAttributeGroups = selectedGroups;

            _mockShoppingCartRepository.Setup(r => r.GetByIdAsync(command.CartId)).ReturnsAsync((ShoppingCart?)null);
            _mockProductRepository.Setup(r => r.GetByIdAsync(productId)).ReturnsAsync(product);
            _mockShoppingCartRepository.Setup(r => r.SaveAsync(It.IsAny<ShoppingCart>())).Returns(Task.CompletedTask);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mockShoppingCartRepository.Verify(r => r.SaveAsync(It.IsAny<ShoppingCart>()), Times.Once);
            // Further assertions can be added to check the content of the saved cart
        }

        [Fact]
        public async Task Handle_ShouldAddProductToCartSuccessfully_WhenProductHasAttributesButNoSelectedGroupsAreProvidedAndNoneAreRequired()
        {
            // Arrange
            var productId = 1;
            var product = CreateSampleProduct(withAttributes: true, isGroupRequired: false); // Product has optional attributes
            var command = CreateAddProductToCartCommand(productId, 1, selectedAttributeGroups: null); // No selected groups

            _mockShoppingCartRepository.Setup(r => r.GetByIdAsync(command.CartId)).ReturnsAsync((ShoppingCart?)null);
            _mockProductRepository.Setup(r => r.GetByIdAsync(productId)).ReturnsAsync(product);
            _mockShoppingCartRepository.Setup(r => r.SaveAsync(It.IsAny<ShoppingCart>())).Returns(Task.CompletedTask);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mockShoppingCartRepository.Verify(r => r.SaveAsync(It.IsAny<ShoppingCart>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldAddProductToCartSuccessfully_WhenSelectedAttributesAreLowerEqualThanRequired()
        {
            // Arrange
            var productId = 1;
            var product = CreateSampleProduct(withAttributes: true, verifyValue: "LOWER_EQUAL_THAN", groupAttributeQuantity: 2);

            var selectedAttributes = new List<Domain.Entities.Attribute>
            {
                new Domain.Entities.Attribute { AttributeId = 101, Name = "Attribute 1", DefaultQuantity = 0, MaxQuantity = 5, StatusId = "A" }
            };

            var selectedGroups = new List<ProductAttributeGroup>
            {
                new ProductAttributeGroup
                {
                    GroupAttributeId = "G1",
                    GroupAttributeType = product.AttributeGroups!.First().GroupAttributeType,
                    Description = "Sample Group",
                    QuantityInformation = new QuantityInformation { GroupAttributeQuantity = 2, VerifyValue = "LOWER_EQUAL_THAN" },
                    Attributes = selectedAttributes,
                    Order = 1
                }
            };
            var command = CreateAddProductToCartCommand(productId, 1, selectedGroups);

            _mockShoppingCartRepository.Setup(r => r.GetByIdAsync(command.CartId)).ReturnsAsync((ShoppingCart?)null);
            _mockProductRepository.Setup(r => r.GetByIdAsync(productId)).ReturnsAsync(product);
            _mockShoppingCartRepository.Setup(r => r.SaveAsync(It.IsAny<ShoppingCart>())).Returns(Task.CompletedTask);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mockShoppingCartRepository.Verify(r => r.SaveAsync(It.IsAny<ShoppingCart>()), Times.Once);
        }
    }
}
