using MediatR;
using ShoppingCartApi.Domain.Entities;
using System.Collections.Generic;

namespace ShoppingCartApi.Application.UseCases.AddProductToCart
{
    /// <summary>
    /// Comando para Agregar Producto al Carrito
    /// Encapsula los datos necesarios para agregar un producto a un carrito de compras
    /// </summary>
    /// <remarks>
    /// Patrón: CQRS (Command Query Responsibility Segregation) con MediatR
    /// Responsabilidad: Transportar datos del request hacia el handler
    /// 
    /// Flujo:
    /// 1. Controller recibe HTTP POST /api/ShoppingCart/add-product
    /// 2. Mapea DTO a este Command
    /// 3. MediatR envía el Command a AddProductToCartCommandHandler
    /// 4. Handler ejecuta la lógica y persiste cambios
    /// 
    /// Validación:
    /// - Atributos seleccionados validados en handler con ProductAttributeValidationSpecification
    /// - Producto validado que exista en repositorio
    /// - Cantidad validada sea mayor a 0
    /// </remarks>
    public class AddProductToCartCommand : IRequest
    {
        /// <summary>
        /// ID del carrito de compras donde agregar el producto
        /// </summary>
        /// <remarks>
        /// Identifica el carrito específico del usuario
        /// Si no existe, el handler lo crea
        /// </remarks>
        public int CartId { get; set; }

        /// <summary>
        /// ID del producto a agregar
        /// </summary>
        /// <remarks>
        /// Debe existir en la base de datos
        /// El handler valida que exista y lanza NotFoundException si no
        /// </remarks>
        public int ProductId { get; set; }

        /// <summary>
        /// Cantidad de unidades a agregar (debe ser > 0)
        /// </summary>
        /// <remarks>
        /// Si el producto ya existe en el carrito,
        /// la cantidad se suma a la existente (en ShoppingCart.AddItem)
        /// </remarks>
        public int Quantity { get; set; }

        /// <summary>
        /// Atributos seleccionados por el usuario (talle, color, etc.)
        /// </summary>
        /// <remarks>
        /// Null si el producto no tiene atributos
        /// Validado en handler: grupos obligatorios deben estar presentes
        /// Validado con ProductAttributeValidationSpecification
        /// </remarks>
        public List<ProductAttributeGroup>? SelectedAttributeGroups { get; set; }
    }
}
