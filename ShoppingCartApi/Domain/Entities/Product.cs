using System.Collections.Generic;

namespace ShoppingCartApi.Domain.Entities
{
    /// <summary>
    /// Entidad Producto - Representa un producto disponible en el carrito de compras
    /// </summary>
    /// <remarks>
    /// Responsabilidades:
    /// - Almacenar información básica del producto (nombre, precio, imagen)
    /// - Gestionar grupos de atributos (talle, color, etc.)
    /// - Ser utilizado en items del carrito de compras
    /// 
    /// Patrón: Domain-Driven Design (Entity Pattern)
    /// Validación: Se valida en Application Layer con ProductAttributeValidationSpecification
    /// </remarks>
    public class Product
    {
        /// <summary>
        /// ID único del producto (clave primaria)
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nombre descriptivo del producto (ejemplo: "iPhone 15 Pro")
        /// </summary>
        /// <remarks>
        /// Requerido y visible al usuario en la UI
        /// Máximo 200 caracteres recomendado
        /// </remarks>
        public required string Name { get; set; }

        /// <summary>
        /// Precio del producto en moneda base (ejemplo: S/ o USD)
        /// </summary>
        /// <remarks>
        /// Almacenado como decimal para precisar exacta en cálculos monetarios
        /// Debe ser mayor a 0
        /// Utilizado para calcular el total del carrito
        /// 
        /// Cálculo: total = Product.Price * ShoppingCartItem.Quantity
        /// </remarks>
        public decimal Price { get; set; }

        /// <summary>
        /// URL de la imagen del producto (null si no hay imagen)
        /// </summary>
        /// <remarks>
        /// Ejemplo: "https://images.unsplash.com/photo-XXX?w=400"
        /// Opcional. Si es null, la UI mostrará una imagen de placeholder
        /// Debe ser una URL válida y accesible públicamente
        /// Recomendado usar HTTPS para seguridad
        /// </remarks>
        public string? ImageUrl { get; set; }

        /// <summary>
        /// Grupos de atributos del producto (talle, color, etc.)
        /// </summary>
        /// <remarks>
        /// Null si el producto no tiene atributos
        /// Contiene lista de ProductAttributeGroup (ejemplo: talle S-M-L)
        /// Los atributos se seleccionan cuando se agrega el producto al carrito
        /// Validación: ProductAttributeValidationSpecification verifica que los atributos obligatorios estén seleccionados
        /// </remarks>
        public List<ProductAttributeGroup>? AttributeGroups { get; set; }
    }
}
