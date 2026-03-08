using MediatR;

namespace ShoppingCartApi.Application.UseCases.RemoveProductFromCart
{
    /// <summary>
    /// Comando para Eliminar Producto del Carrito
    /// Encapsula los datos necesarios para remover un producto del carrito de un usuario
    /// </summary>
    /// <remarks>
    /// Patron: CQRS - MediatR Command
    /// Responsabilidad: Transportar UserId y ProductId al handler
    /// 
    /// Casos de uso:
    /// - Usuario presiona boton "Eliminar" en el carrito
    /// - Se elimina completamente el producto (todas sus unidades)
    /// 
    /// Nota: Si el producto no existe en el carrito, no genera error
    /// </remarks>
    public class RemoveProductFromCartCommand : IRequest
    {
        /// <summary>
        /// ID del usuario propietario del carrito
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// ID del producto a eliminar del carrito
        /// </summary>
        /// <remarks>
        /// Si este producto no existe en el carrito, la operacion es no-op (sin error)
        /// </remarks>
        public int ProductId { get; set; }
    }
}
