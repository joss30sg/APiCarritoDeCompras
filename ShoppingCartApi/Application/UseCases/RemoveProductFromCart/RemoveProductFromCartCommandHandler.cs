using MediatR;
using ShoppingCartApi.Domain.Interfaces;
using System.Threading;
using System.Threading.Tasks;
using ShoppingCartApi.Domain.Exceptions; // Añadir para NotFoundException
using ShoppingCartApi.Domain.Entities; // Añadir para ShoppingCart

namespace ShoppingCartApi.Application.UseCases.RemoveProductFromCart
{
    public class RemoveProductFromCartCommandHandler : IRequestHandler<RemoveProductFromCartCommand>
    {
        private readonly IShoppingCartRepository _shoppingCartRepository;

        public RemoveProductFromCartCommandHandler(IShoppingCartRepository shoppingCartRepository)
        {
            _shoppingCartRepository = shoppingCartRepository;
        }

        public async Task Handle(RemoveProductFromCartCommand request, CancellationToken cancellationToken)
        {
            var cart = await _shoppingCartRepository.GetCartByUserIdAsync(request.UserId);
            if (cart == null)
            {
                return;
            }

            cart.RemoveItem(request.ProductId);
            await _shoppingCartRepository.SaveAsync(cart);
        }
    }
}
