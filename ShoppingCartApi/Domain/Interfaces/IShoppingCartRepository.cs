using ShoppingCartApi.Domain.Entities;
using System.Threading.Tasks;

namespace ShoppingCartApi.Domain.Interfaces
{
    public interface IShoppingCartRepository
    {
        Task<ShoppingCart?> GetByIdAsync(int id);
        Task SaveAsync(ShoppingCart cart);
        Task RemoveProductAsync(int cartId, int productId);
        Task<ShoppingCart?> GetCartByUserIdAsync(int userId);
    }
}
