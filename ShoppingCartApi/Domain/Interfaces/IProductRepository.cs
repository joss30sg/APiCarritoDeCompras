using ShoppingCartApi.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShoppingCartApi.Domain.Interfaces
{
    public interface IProductRepository
    {
        Task<Product?> GetByIdAsync(int id);
        Task<IEnumerable<Product>> GetAllAsync();
    }
}
