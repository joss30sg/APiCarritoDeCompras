using ShoppingCartApi.Domain.Entities;
using ShoppingCartApi.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCartApi.Infrastructure.Repositories
{
    public class InMemoryShoppingCartRepository : IShoppingCartRepository
    {
        private readonly Dictionary<int, ShoppingCart> _shoppingCarts = new Dictionary<int, ShoppingCart>();

        public Task<ShoppingCart?> GetByIdAsync(int id)
        {
            _shoppingCarts.TryGetValue(id, out var cart);
            return Task.FromResult(cart);
        }

        public Task SaveAsync(ShoppingCart cart)
        {
            _shoppingCarts[cart.Id] = cart;
            return Task.CompletedTask;
        }

        public Task RemoveProductAsync(int cartId, int productId)
        {
            if (_shoppingCarts.TryGetValue(cartId, out var cart))
            {
                var itemToRemove = cart.Items.FirstOrDefault(item => item.ProductId == productId);
                if (itemToRemove != null)
                {
                    cart.Items.Remove(itemToRemove);
                }
            }
            return Task.CompletedTask;
        }

        public Task<ShoppingCart?> GetCartByUserIdAsync(int userId)
        {
            var cart = _shoppingCarts.Values.FirstOrDefault(c => c.UserId == userId);
            return Task.FromResult(cart);
        }
    }
}
