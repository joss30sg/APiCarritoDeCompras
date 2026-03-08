namespace ShoppingCartApi.Domain.Entities
{
    /// <summary>
    /// Entidad Item del Carrito - Representa un producto agregado al carrito de un usuario
    /// </summary>
    /// <remarks>
    /// Responsabilidades:
    /// - Almacenar referencia al producto y su cantidad en el carrito
    /// - Almacenar atributos seleccionados del producto (talle, color, etc.)
    /// 
    /// Patrón: Domain-Driven Design (Value Object Pattern)
    /// Relación: Cada ShoppingCartItem pertenece a un único ShoppingCart
    /// </remarks>
    public class ShoppingCartItem
    {
        /// <summary>
        /// ID del producto en el carrito (clave foránea a Product.Id)
        /// </summary>
        /// <remarks>
        /// Utilizado para identificar y recuperar el producto asociado
        /// </remarks>
        public int ProductId { get; set; }

        /// <summary>
        /// Producto completo (con todos sus datos: nombre, precio, atributos)
        /// </summary>
        /// <remarks>
        /// Requerido. Cargado desde el repositorio de productos
        /// Contiene información necesaria para calcular subtotal
        /// </remarks>
        public required Product Product { get; set; }

        /// <summary>
        /// Cantidad de unidades de este producto en el carrito
        /// </summary>
        /// <remarks>
        /// Debe ser >= 1 (no se almacenan items con cantidad 0)
        /// Utilizado para calcular: Subtotal = Product.Price * Quantity
        /// </remarks>
        public int Quantity { get; set; }
    }
}
