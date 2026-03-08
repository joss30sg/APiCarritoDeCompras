using System.Collections.Generic;

namespace ShoppingCartApi.Domain.Entities
{
    /// <summary>
    /// Tipo de Grupo de Atributos - Define un grupo de características disponibles para un producto
    /// </summary>
    /// <remarks>
    /// Ejemplos: "Talle" (S, M, L, XL), "Color" (Rojo, Azul, Negro), "Capacidad" (64GB, 128GB, 256GB)
    /// Responsabilidades:
    /// - Definir categorías de atributos (P.ej: Talle, Color)
    /// - Especificar si el grupo es obligatorio al comprar
    /// 
    /// Patrón: Domain-Driven Design (Value Object Pattern)
    /// </remarks>
    public class GroupAttributeType
    {
        /// <summary>
        /// ID único del tipo de grupo (P.ej: "SIZE", "COLOR")
        /// </summary>
        public required string GroupAttributeTypeId { get; set; }

        /// <summary>
        /// Nombre descriptivo del grupo en idioma local (P.ej: "Talle", "Color")
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// Indica si este grupo es obligatorio al agregar el producto al carrito
        /// </summary>
        /// <remarks>
        /// true = Usuario DEBE seleccionar un valor de este grupo
        /// false = Es opcional, el usuario puede saltarlo
        /// Ejemplo:
        /// - Talle: true (obligatorio para ropa)
        /// - Color: true (obligatorio)
        /// - Garantía extendida: false (opcional)
        /// </remarks>
        public bool IsRequired { get; set; }
    }

    /// <summary>
    /// Información de Cantidad - Controla cómo se muestra y edita la cantidad de un atributo
    /// </summary>
    /// <remarks>
    /// Responsabilidades:
    /// - Definir límites de cantidad (min/máx)
    /// - Controlar visibilidad y editabilidad en la UI
    /// - Almacenar información de verificación
    /// 
    /// Patrón: Domain-Driven Design (Value Object Pattern)
    /// </remarks>
    public class QuantityInformation
    {
        /// <summary>
        /// Cantidad de unidades de este atributo (P.ej: cantidad de complementos)
        /// </summary>
        public int GroupAttributeQuantity { get; set; }

        /// <summary>
        /// Si true, muestra el precio por unidad individual
        /// </summary>
        public bool ShowPricePerProduct { get; set; }

        /// <summary>
        /// Si true, el atributo es visible en la UI
        /// </summary>
        public bool IsShown { get; set; }

        /// <summary>
        /// Si true, el usuario puede cambiar la cantidad de este atributo
        /// </summary>
        public bool IsEditable { get; set; }

        /// <summary>
        /// Si true, la cantidad ha sido validada correctamente
        /// </summary>
        public bool IsVerified { get; set; }

        /// <summary>
        /// Valor de verificación personalizado (P.ej: validación personalizada data)
        /// </summary>
        public string? VerifyValue { get; set; }
    }

    /// <summary>
    /// Atributo Individual - Representa un valor específico dentro de un grupo de atributos
    /// </summary>
    /// <remarks>
    /// Ejemplos:
    /// - Grupo "Talle" -> Atributos ["S", "M", "L", "XL"]
    /// - Grupo "Color" -> Atributos ["Rojo", "Azul", "Negro"]
    /// 
    /// Responsabilidades:
    /// - Almacenar nombre del atributo
    /// - Controlar cantidad máxima permitida
    /// - Definir impacto en precio
    /// - Indicar si es requerido
    /// 
    /// Patrón: Domain-Driven Design (Value Object Pattern)
    /// </remarks>
    public class Attribute
    {
        /// <summary>
        /// ID del producto propietario de este atributo (clave foránea)
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// ID único del atributo dentro del producto
        /// </summary>
        public int AttributeId { get; set; }

        /// <summary>
        /// Nombre del atributo visible al usuario (P.ej: "Pequeño", "Rojo", "128GB")
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// Cantidad por defecto de este atributo (P.ej: 1 para prendas, 2 para accesorios)
        /// </summary>
        public int DefaultQuantity { get; set; }

        /// <summary>
        /// Cantidad máxima permitida (P.ej: no puedes seleccionar más de 5 unidades)
        /// </summary>
        public int MaxQuantity { get; set; }

        /// <summary>
        /// Impacto en precio (P.ej: +$50 si seleccionas "Garantía extendida")
        /// </summary>
        public int PriceImpactAmount { get; set; }

        /// <summary>
        /// Si true, este atributo DEBE ser seleccionado al comprar
        /// </summary>
        public bool IsRequired { get; set; }

        /// <summary>
        /// ID de atributo negativo (P.ej: no puedes seleccionar X e Y juntos)
        /// </summary>
        public string? NegativeAttributeId { get; set; }

        /// <summary>
        /// Orden de visualización en la UI (mayor = más arriba)
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// ID de estado (P.ej: "ACTIVE", "DISCONTINUED")
        /// </summary>
        public required string StatusId { get; set; }

        /// <summary>
        /// URL de imagen para mostrar el atributo visualmente
        /// </summary>
        public string? UrlImage { get; set; }
    }

    /// <summary>
    /// Grupo de Atributos de Producto - Colección de atributos relacionados de un producto
    /// </summary>
    /// <remarks>
    /// Ejemplo: Producto "Camiseta Nike"
    /// - Grupo 1: "Talle" con atributos [S, M, L, XL] (obligatorio)
    /// - Grupo 2: "Color" con atributos [Rojo, Azul, Negro] (obligatorio)
    /// - Grupo 3: "Diseño" con atributos [Rayas, Sin rayas] (opcional)
    /// 
    /// Responsabilidades:
    /// - Agrupar atributos relacionados
    /// - Almacenar información de cantidad y visualización
    /// - Indicar si la selección es obligatoria
    /// 
    /// Patrón: Domain-Driven Design (Aggregate Pattern)
    /// Validación: ProductAttributeValidationSpecification valida que los atributos requeridos estén seleccionados
    /// </remarks>
    public class ProductAttributeGroup
    {
        /// <summary>
        /// ID único del grupo (P.ej: "GROUP_SIZE_001")
        /// </summary>
        public required string GroupAttributeId { get; set; }

        /// <summary>
        /// Tipo/categoría de este grupo (P.ej: "Talle", "Color")
        /// </summary>
        public required GroupAttributeType GroupAttributeType { get; set; }

        /// <summary>
        /// Descripción del grupo visible en la UI (P.ej: "Selecciona tu talle")
        /// </summary>
        public required string Description { get; set; }

        /// <summary>
        /// Información de cantidad y visualización del grupo
        /// </summary>
        public required QuantityInformation QuantityInformation { get; set; }

        /// <summary>
        /// Lista de atributos disponibles en este grupo (puede ser null si no hay atributos)
        /// </summary>
        /// <remarks>
        /// Ejemplo para Talle: [Atributo("S"), Atributo("M"), Atributo("L"), Atributo("XL")]
        /// </remarks>
        public List<Attribute>? Attributes { get; set; }

        /// <summary>
        /// Orden de visualización en la UI (mayor = más arriba)
        /// </summary>
        public int Order { get; set; }
    }
}
