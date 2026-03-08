using MediatR;

namespace ShoppingCartApi.Application.UseCases.UpdateProductQuantity
{
    /// <summary>
    /// Comando para actualizar cantidad de producto en carrito
    /// </summary>
    public class UpdateProductQuantityCommand : IRequest<Unit>
    {
        /// <summary>
        /// ID del carrito a actualizar
        /// </summary>
        public int CartId { get; set; }

        /// <summary>
        /// ID del producto cuya cantidad se actualizara
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// Nueva cantidad. Si es menor o igual a 0, se elimina el producto.
        /// </summary>
        public int Quantity { get; set; }
    }
}
