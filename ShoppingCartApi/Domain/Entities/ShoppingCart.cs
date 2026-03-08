using System.Collections.Generic;
using System.Linq;

namespace ShoppingCartApi.Domain.Entities
{
    /// <summary>
    /// Entidad Carrito de Compras - Representa el carrito de un usuario autenticado
    /// </summary>
    public class ShoppingCart
    {
        /// <summary>
        /// ID unico del carrito
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// ID del usuario propietario del carrito
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Lista de items en el carrito
        /// </summary>
        public List<ShoppingCartItem> Items { get; set; } = new List<ShoppingCartItem>();

        /// <summary>
        /// Agrega un producto al carrito. Si ya existe, suma la cantidad.
        /// </summary>
        public void AddItem(Product product, int quantity)
        {
            var existingItem = Items.FirstOrDefault(item => item.ProductId == product.Id);
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                Items.Add(new ShoppingCartItem { ProductId = product.Id, Product = product, Quantity = quantity });
            }
        }

        /// <summary>
        /// Actualiza la cantidad de un producto. Si es menor o igual a 0, lo elimina.
        /// </summary>
        public void UpdateItemQuantity(int productId, int quantity)
        {
            var existingItem = Items.FirstOrDefault(item => item.ProductId == productId);
            if (existingItem != null)
            {
                existingItem.Quantity = quantity;
                if (existingItem.Quantity <= 0)
                {
                    RemoveItem(productId);
                }
            }
        }

        /// <summary>
        /// Elimina un producto del carrito
        /// </summary>
        public void RemoveItem(int productId)
        {
            Items.RemoveAll(item => item.ProductId == productId);
        }

        /// <summary>
        /// Calcula el precio total del carrito
        /// </summary>
        public decimal GetTotalPrice()
        {
            return Items.Sum(item => item.Product.Price * item.Quantity);
        }
    }
}
