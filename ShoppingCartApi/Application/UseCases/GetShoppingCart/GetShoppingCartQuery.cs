using MediatR;
using ShoppingCartApi.Domain.Entities;

namespace ShoppingCartApi.Application.UseCases.GetShoppingCart
{
    /// <summary>
    /// Query para Obtener Carrito de Compras
    /// Encapsula los datos necesarios para consultar el carrito de un usuario
    /// </summary>
    /// <remarks>
    /// Patrón: CQRS (Command Query Responsibility Segregation) con MediatR
    /// Responsabilidad: Transportar datos del request hacia el handler (solo lectura)
    /// 
    /// Flujo:
    /// 1. Controller recibe HTTP GET /api/ShoppingCart/{userId}
    /// 2. Crea esta Query con el UserId
    /// 3. MediatR envía la Query a GetShoppingCartQueryHandler
    /// 4. Handler consulta repositorio y retorna el carrito
    /// 
    /// Retorna:
    /// - ShoppingCart lleno si el usuario tiene carrito con items
    /// - ShoppingCart vacío (UserId, Items=[]) si no tiene carrito,
    ///   así el cliente puede agregar productos sin error
    /// </remarks>
    public class GetShoppingCartQuery : IRequest<ShoppingCart>
    {
        /// <summary>
        /// ID del usuario cuyo carrito se desea obtener
        /// </summary>
        /// <remarks>
        /// Identifica unívocamente al usuario
        /// Un usuario tiene exactamente un carrito
        /// El handler obtiene el carrito via IShoppingCartRepository.GetCartByUserIdAsync()
        /// </remarks>
        public int UserId { get; set; }
    }
}
