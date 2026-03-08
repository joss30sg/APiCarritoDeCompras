using MediatR;
using ShoppingCartApi.Domain.Interfaces;
using ShoppingCartApi.Domain.Entities;
using ShoppingCartApi.Domain.Exceptions;
using ShoppingCartApi.Domain.Specifications;

namespace ShoppingCartApi.Application.UseCases.AddProductToCart
{
    /// <summary>
    /// Manejador del Comando de Agregar Producto al Carrito
    /// Ejecuta la logica para agregar un producto al carrito
    /// </summary>
    /// <remarks>
    /// Patron: CQRS Handler Pattern
    /// Flujo:
    /// 1. Obtiene o crea carrito
    /// 2. Valida que el producto existe
    /// 3. Valida atributos con ProductAttributeValidationSpecification
    /// 4. Agrega producto al carrito
    /// 5. Persiste cambios
    /// </remarks>
    public class AddProductToCartCommandHandler : IRequestHandler<AddProductToCartCommand>
    {
        private readonly IShoppingCartRepository _shoppingCartRepository;
        private readonly IProductRepository _productRepository;

        public AddProductToCartCommandHandler(IShoppingCartRepository shoppingCartRepository, IProductRepository productRepository)
        {
            _shoppingCartRepository = shoppingCartRepository;
            _productRepository = productRepository;
        }

        /// <summary>
        /// Ejecuta el comando de agregar producto
        /// </summary>
        public async Task Handle(AddProductToCartCommand request, CancellationToken cancellationToken)
        {
            var cart = await _shoppingCartRepository.GetByIdAsync(request.CartId);
            if (cart == null)
            {
                cart = new ShoppingCart { Id = request.CartId };
            }

            var product = await _productRepository.GetByIdAsync(request.ProductId);
            if (product == null)
            {
                throw new Exception($"Product with ID {request.ProductId} not found.");
            }

            var validationSpec = new ProductAttributeValidationSpecification(product, request.SelectedAttributeGroups);
            var errors = validationSpec.Validate();
            if (errors.Any())
            {
                throw new ValidationException(errors);
            }

            cart.AddItem(product, request.Quantity);
            await _shoppingCartRepository.SaveAsync(cart);
        }

    }
}
