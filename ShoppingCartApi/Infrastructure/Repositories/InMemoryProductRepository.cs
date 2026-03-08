using ShoppingCartApi.Domain.Entities;
using ShoppingCartApi.Domain.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShoppingCartApi.Infrastructure.Repositories
{
    public class InMemoryProductRepository : IProductRepository
    {
        private readonly Dictionary<int, Product> _products = new Dictionary<int, Product>();

        public InMemoryProductRepository()
        {
            // Seed initial products with working images from Unsplash
            _products.Add(1, new Product 
            { 
                Id = 1, 
                Name = "Laptop Dell XPS 13", 
                Price = 4809.96m, 
                ImageUrl = "https://images.unsplash.com/photo-1517694712202-14dd9538aa97?w=400&h=300&fit=crop" 
            });
            
            _products.Add(2, new Product 
            { 
                Id = 2, 
                Name = "iPhone 15 Pro", 
                Price = 3699.96m, 
                ImageUrl = "https://images.unsplash.com/photo-1592286927505-1def25115558?w=400&h=300&fit=crop" 
            });
            
            _products.Add(3, new Product 
            { 
                Id = 3, 
                Name = "AirPods Pro", 
                Price = 924.96m, 
                ImageUrl = "https://images.unsplash.com/photo-1505740420928-5e560c06d30e?w=400&h=300&fit=crop" 
            });
            
            _products.Add(4, new Product 
            { 
                Id = 4, 
                Name = "iPad Air", 
                Price = 2219.96m, 
                ImageUrl = "https://images.unsplash.com/photo-1587372247318-f18282c81a98?w=400&h=300&fit=crop" 
            });
            
            _products.Add(5, new Product 
            { 
                Id = 5, 
                Name = "Apple Watch Series 9", 
                Price = 1479.96m, 
                ImageUrl = "https://images.unsplash.com/photo-1523275335684-37898b6baf30?w=400&h=300&fit=crop" 
            });
            
            _products.Add(6, new Product 
            { 
                Id = 6, 
                Name = "Monitor 4K Ultra", 
                Price = 1899.96m, 
                ImageUrl = "https://images.unsplash.com/photo-1527864550417-7fd91fc51a46?w=400&h=300&fit=crop" 
            });
            
            _products.Add(7, new Product 
            { 
                Id = 7, 
                Name = "Samsung Galaxy S24 Ultra", 
                Price = 4499.96m, 
                ImageUrl = "https://images.unsplash.com/photo-1511707267537-b85faf00021e?w=400&h=300&fit=crop" 
            });
            
            _products.Add(8, new Product 
            { 
                Id = 8, 
                Name = "Sony WH-1000XM5", 
                Price = 1899.96m, 
                ImageUrl = "https://images.unsplash.com/photo-1487215078519-e21cc028cb29?w=400&h=300&fit=crop" 
            });
            
            _products.Add(9, new Product 
            { 
                Id = 9, 
                Name = "Mac Studio", 
                Price = 6999.96m, 
                ImageUrl = "https://images.unsplash.com/photo-1517694712202-14dd9538aa97?w=400&h=300&fit=crop" 
            });
            
            _products.Add(10, new Product 
            { 
                Id = 10, 
                Name = "Canon EOS R5", 
                Price = 9999.96m, 
                ImageUrl = "https://images.unsplash.com/photo-1606986628025-35d57e735ae0?w=400&h=300&fit=crop" 
            });
        }

        public Task<Product?> GetByIdAsync(int id)
        {
            _products.TryGetValue(id, out var product);
            return Task.FromResult(product);
        }

        public Task<IEnumerable<Product>> GetAllAsync()
        {
            return Task.FromResult<IEnumerable<Product>>(_products.Values);
        }
    }
}
