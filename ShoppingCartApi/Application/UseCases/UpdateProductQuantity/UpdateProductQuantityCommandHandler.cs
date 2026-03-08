using MediatR;
using ShoppingCartApi.Domain.Interfaces;
using System.Threading;
using System.Threading.Tasks;
using ShoppingCartApi.Domain.Exceptions; // Añadir para NotFoundException
using ShoppingCartApi.Domain.Entities; // Añadir para ShoppingCart

namespace ShoppingCartApi.Application.UseCases.UpdateProductQuantity
{
    public class UpdateProductQuantityCommandHandler : IRequestHandler<UpdateProductQuantityCommand, Unit>
    {
        private readonly IShoppingCartRepository _shoppingCartRepository;

        public UpdateProductQuantityCommandHandler(IShoppingCartRepository shoppingCartRepository)
        {
            _shoppingCartRepository = shoppingCartRepository;
        }

        public async Task<Unit> Handle(UpdateProductQuantityCommand request, CancellationToken cancellationToken)
        {
            var shoppingCart = await _shoppingCartRepository.GetByIdAsync(request.CartId);

            if (shoppingCart == null)
            {
                throw new Exception($"Shopping cart with ID {request.CartId} not found.");
            }

            shoppingCart.UpdateItemQuantity(request.ProductId, request.Quantity);

            await _shoppingCartRepository.SaveAsync(shoppingCart);

            return Unit.Value;
        }
    }
}
