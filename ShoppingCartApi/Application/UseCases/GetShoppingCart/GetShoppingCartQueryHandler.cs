using MediatR;
using ShoppingCartApi.Domain.Entities;
using ShoppingCartApi.Domain.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace ShoppingCartApi.Application.UseCases.GetShoppingCart
{
    /// <summary>
    /// Manejador de Query para Obtener Carrito de Compras
    /// Consulta el carrito de un usuario autenticado
    /// </summary>
    /// <remarks>
    /// Patron: CQRS Query Handler Pattern
    /// Responsabilidad: Solo lectura, obtener carrito del usuario
    /// 
    /// Flujo:
    /// 1. Recibe GetShoppingCartQuery con UserId
    /// 2. Consulta repositorio para obtener carrito
    /// 3. Si no existe, retorna carrito vacio (no error)
    /// 4. UI puede agregar productos sin necesidad de crear primero
    /// </remarks>
    public class GetShoppingCartQueryHandler : IRequestHandler<GetShoppingCartQuery, ShoppingCart>
    {
        private readonly IShoppingCartRepository _shoppingCartRepository;

        /// <summary>
        /// Constructor con inyeccion de dependencias
        /// </summary>
        public GetShoppingCartQueryHandler(IShoppingCartRepository shoppingCartRepository)
        {
            _shoppingCartRepository = shoppingCartRepository;
        }

        /// <summary>
        /// Obtiene el carrito del usuario especificado
        /// </summary>
        /// <param name="request">Query con el UserId</param>
        /// <param name="cancellationToken">Token para cancelacion</param>
        /// <returns>ShoppingCart lleno o vacio (nunca null)</returns>
        /// <remarks>
        /// Si el carrito no existe:
        /// - Retorna new ShoppingCart { UserId = request.UserId }
        /// - Items estara vacio
        /// - Cliente puede agregar productos sin error 404
        /// </remarks>
        public async Task<ShoppingCart> Handle(GetShoppingCartQuery request, CancellationToken cancellationToken)
        {
            var cart = await _shoppingCartRepository.GetCartByUserIdAsync(request.UserId);
            return cart ?? new ShoppingCart { UserId = request.UserId };
        }
    }
}
