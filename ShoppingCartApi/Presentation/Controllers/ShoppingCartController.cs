using MediatR;
using Microsoft.AspNetCore.Mvc;
using ShoppingCartApi.Application.UseCases.AddProductToCart;
using ShoppingCartApi.Application.UseCases.UpdateProductQuantity;
using ShoppingCartApi.Application.UseCases.RemoveProductFromCart;
using ShoppingCartApi.Application.UseCases.GetShoppingCart;
using ShoppingCartApi.Presentation.Models;
using ShoppingCartApi.Presentation.Helpers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace ShoppingCartApi.Presentation.Controllers
{
    /// <summary>API de Carrito - todos los endpoints requieren autenticación JWT</summary>
    [ApiController]
    [Route("api/v1/shopping-cart")]
    [Authorize]
    [Produces("application/json")]
    public class ShoppingCartController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ShoppingCartController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>Obtiene el carrito completo del usuario autenticado</summary>
        /// <returns>Carrito con items y totales</returns>
        /// <response code="200">Carrito obtenido exitosamente</response>
        /// <response code="401">Usuario no autenticado</response>
        /// <response code="500">Error del servidor</response>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetShoppingCart()
        {
            try
            {
                var validationResult = AuthenticationHelper.ValidateUserAuthentication(this, User);
                if (validationResult != null) return validationResult;

                var userId = AuthenticationHelper.GetUserId(User);
                var query = new GetShoppingCartQuery { UserId = userId };
                var cart = await _mediator.Send(query);

                return Ok(ApiResponse<object>.SuccessResponse(
                    cart,
                    "Shopping cart retrieved successfully",
                    StatusCodes.Status200OK
                ));
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse.ErrorResponse(
                        "An error occurred while retrieving the shopping cart",
                        StatusCodes.Status500InternalServerError,
                        new List<string> { ex.Message }
                    )
                );
            }
        }

        /// <summary>Agrega producto al carrito del usuario autenticado</summary>
        /// <remarks>Si el producto existe, incrementa la cantidad. Valida que exista y cantidad sea válida.</remarks>
        /// <param name="command">Comando con productId, cantidad y atributos seleccionados</param>
        /// <returns>Confirmación de agregación</returns>
        /// <response code="201">Producto agregado exitosamente</response>
        /// <response code="400">Producto no existe o atributos inválidos</response>
        /// <response code="401">Usuario no autenticado</response>
        /// <response code="500">Error del servidor</response>
        [HttpPost("items")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddProductToCart([FromBody] AddProductToCartCommand command)
        {
            try
            {
                var validationResult = AuthenticationHelper.ValidateUserAuthentication(this, User);
                if (validationResult != null) return validationResult;

                // TODO: Obtener CartId del usuario autenticado
                // Por ahora, el comando ya viene con CartId
                
                await _mediator.Send(command);

                return StatusCode(
                    StatusCodes.Status201Created,
                    ApiResponse.SuccessResponse(
                        "Product added to shopping cart successfully",
                        StatusCodes.Status201Created
                    )
                );
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse.ErrorResponse(
                        "An error occurred while adding the product to the shopping cart",
                        StatusCodes.Status500InternalServerError,
                        new List<string> { ex.Message }
                    )
                );
            }
        }

        /// <summary>Actualiza la cantidad de un producto en el carrito</summary>
        /// <remarks>Si cantidad es 0 o menor, se elimina el producto automáticamente</remarks>
        /// <param name="productId">ID del producto a actualizar</param>
        /// <param name="command">Comando con la nueva cantidad</param>
        /// <returns>204 No Content</returns>
        /// <response code="204">Cantidad actualizada exitosamente</response>
        /// <response code="400">Producto no encontrado o cantidad inválida</response>
        /// <response code="401">Usuario no autenticado</response>
        /// <response code="500">Error del servidor</response>
        [HttpPut("items/{productId}")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateProductQuantity(int productId, [FromBody] UpdateProductQuantityCommand command)
        {
            try
            {
                var validationResult = AuthenticationHelper.ValidateUserAuthentication(this, User);
                if (validationResult != null) return validationResult;

                // TODO: Obtener CartId del usuario autenticado
                // Por ahora, el comando ya viene con CartId y ProductId
                command.ProductId = productId;

                await _mediator.Send(command);

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse.ErrorResponse(
                        "An error occurred while updating the product quantity",
                        StatusCodes.Status500InternalServerError,
                        new List<string> { ex.Message }
                    )
                );
            }
        }

        /// <summary>Elimina un producto del carrito del usuario</summary>
        /// <param name="productId">ID del producto a eliminar</param>
        /// <returns>204 No Content</returns>
        /// <response code="204">Producto eliminado exitosamente</response>
        /// <response code="400">Producto no encontrado en carrito</response>
        /// <response code="401">Usuario no autenticado</response>
        /// <response code="500">Error del servidor</response>
        [HttpDelete("items/{productId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RemoveProductFromCart(int productId)
        {
            try
            {
                var validationResult = AuthenticationHelper.ValidateUserAuthentication(this, User);
                if (validationResult != null) return validationResult;

                var userId = AuthenticationHelper.GetUserId(User);
                var command = new RemoveProductFromCartCommand { UserId = userId, ProductId = productId };

                await _mediator.Send(command);

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse.ErrorResponse(
                        "An error occurred while removing the product from the shopping cart",
                        StatusCodes.Status500InternalServerError,
                        new List<string> { ex.Message }
                    )
                );
            }
        }
    }
}
