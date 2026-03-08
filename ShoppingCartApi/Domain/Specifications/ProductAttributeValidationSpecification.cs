using ShoppingCartApi.Domain.Entities;

namespace ShoppingCartApi.Domain.Specifications
{
    /// <summary>
    /// Especificación para validar los atributos del producto en el carrito
    /// Encapsula la lógica de validación compleja de grupos y atributos
    /// Patrón: Specification Pattern, Single Responsibility Principle
    /// </summary>
    public class ProductAttributeValidationSpecification : Specification
    {
        private readonly Product _product;
        private readonly List<ProductAttributeGroup>? _selectedAttributeGroups;

        public ProductAttributeValidationSpecification(
            Product product,
            List<ProductAttributeGroup>? selectedAttributeGroups)
        {
            _product = product;
            _selectedAttributeGroups = selectedAttributeGroups;
        }

        public override List<string> Validate()
        {
            var errors = new List<string>();

            // 1. Consistencia de grupos: Validar que los grupos enviados sean consistentes
            if (_product.AttributeGroups == null && _selectedAttributeGroups != null && _selectedAttributeGroups.Any())
            {
                errors.Add("El producto no admite grupos de atributos, pero se proporcionaron grupos.");
            }

            if (_product.AttributeGroups != null)
            {
                // Validar grupos requeridos
                ValidateRequiredAttributeGroups(errors);

                // Validar grupos seleccionados
                ValidateSelectedAttributeGroups(errors);
            }

            return errors;
        }

        /// <summary>
        /// Validar que todos los grupos obligatorios estén presentes
        /// </summary>
        private void ValidateRequiredAttributeGroups(List<string> errors)
        {
            foreach (var productGroup in _product.AttributeGroups!)
            {
                if (productGroup.GroupAttributeType.IsRequired &&
                    (_selectedAttributeGroups == null || !_selectedAttributeGroups.Any(g => g.GroupAttributeId == productGroup.GroupAttributeId)))
                {
                    errors.Add($"El grupo de atributos obligatorio '{productGroup.GroupAttributeType.Name}' está ausente.");
                }
            }
        }

        /// <summary>
        /// Validar grupos y atributos seleccionados
        /// </summary>
        private void ValidateSelectedAttributeGroups(List<string> errors)
        {
            foreach (var selectedGroup in _selectedAttributeGroups ?? new List<ProductAttributeGroup>())
            {
                var productGroup = _product.AttributeGroups?.FirstOrDefault(g => g.GroupAttributeId == selectedGroup.GroupAttributeId);

                if (productGroup == null)
                {
                    errors.Add($"El grupo de atributos con ID '{selectedGroup.GroupAttributeId}' no se encontró para el producto {_product.Id}.");
                    continue;
                }

                // Validar cantidad de atributos seleccionados
                ValidateAttributeQuantity(selectedGroup, productGroup, errors);

                // Validar atributos individuales
                ValidateIndividualAttributes(selectedGroup, productGroup, errors);
            }
        }

        /// <summary>
        /// Validar la cantidad de atributos seleccionados
        /// </summary>
        private void ValidateAttributeQuantity(
            ProductAttributeGroup selectedGroup,
            ProductAttributeGroup productGroup,
            List<string> errors)
        {
            var selectedAttributesCount = selectedGroup.Attributes?.Count ?? 0;
            var requiredAttributeQuantity = productGroup.QuantityInformation.GroupAttributeQuantity;
            var verifyValue = productGroup.QuantityInformation.VerifyValue;

            if (verifyValue == "EQUAL_THAN")
            {
                if (selectedAttributesCount != requiredAttributeQuantity)
                {
                    errors.Add($"Para el grupo '{productGroup.GroupAttributeType.Name}', se deben seleccionar exactamente {requiredAttributeQuantity} atributos. Se seleccionaron {selectedAttributesCount}.");
                }
            }
            else if (verifyValue == "LOWER_EQUAL_THAN")
            {
                if (selectedAttributesCount > requiredAttributeQuantity)
                {
                    errors.Add($"Para el grupo '{productGroup.GroupAttributeType.Name}', se pueden seleccionar como máximo {requiredAttributeQuantity} atributos. Se seleccionaron {selectedAttributesCount}.");
                }
            }
        }

        /// <summary>
        /// Validar atributos individuales
        /// </summary>
        private void ValidateIndividualAttributes(
            ProductAttributeGroup selectedGroup,
            ProductAttributeGroup productGroup,
            List<string> errors)
        {
            if (productGroup.Attributes == null || !productGroup.Attributes.Any())
            {
                if (selectedGroup.Attributes != null && selectedGroup.Attributes.Any())
                {
                    errors.Add($"El grupo '{selectedGroup.GroupAttributeId}' no admite atributos, pero se proporcionaron atributos.");
                }
                return;
            }

            foreach (var selectedAttribute in selectedGroup.Attributes ?? new List<Domain.Entities.Attribute>())
            {
                var productAttribute = productGroup.Attributes.FirstOrDefault(a => a.AttributeId == selectedAttribute.AttributeId);
                if (productAttribute == null)
                {
                    errors.Add($"El atributo con ID '{selectedAttribute.AttributeId}' no se encontró en el grupo '{productGroup.GroupAttributeId}'.");
                    continue;
                }

                // Validar cantidad de atributo individual
                ValidateAttributeIndividualQuantity(selectedAttribute, productAttribute, productGroup, errors);
            }
        }

        /// <summary>
        /// Validar la cantidad de un atributo individual
        /// </summary>
        private void ValidateAttributeIndividualQuantity(
            Domain.Entities.Attribute selectedAttribute,
            Domain.Entities.Attribute productAttribute,
            ProductAttributeGroup productGroup,
            List<string> errors)
        {
            var defaultQuantity = selectedAttribute.DefaultQuantity;
            var maxQuantity = selectedAttribute.MaxQuantity;  // Usar el MaxQuantity del ATRIBUTO SELECCIONADO, no del producto

            // Validar que DefaultQuantity sea >= 0, MaxQuantity <= 5, y DefaultQuantity <= MaxQuantity
            if (defaultQuantity < 0 || maxQuantity > 5 || defaultQuantity > maxQuantity)
            {
                errors.Add("La cantidad por defecto debe ser al menos 0 y la máxima no debe exceder 5.");
            }
        }
    }
}
