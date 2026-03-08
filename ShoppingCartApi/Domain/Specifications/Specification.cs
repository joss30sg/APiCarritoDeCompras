namespace ShoppingCartApi.Domain.Specifications
{
    /// <summary>
    /// Clase base para patrón Specification
    /// Permite encapsular lógica de validación compleja en objetos reutilizables
    /// Patrón: Specification Pattern, Single Responsibility Principle
    /// </summary>
    public abstract class Specification
    {
        /// <summary>
        /// Validar la especificación
        /// </summary>
        /// <returns>Lista de errores de validación. Vacía si es válida.</returns>
        public abstract List<string> Validate();

        /// <summary>
        /// Obtener si la especificación es válida
        /// </summary>
        public bool IsSatisfiedBy()
        {
            return !Validate().Any();
        }
    }

    /// <summary>
    /// Especificación genérica con datos de entrada
    /// </summary>
    public abstract class Specification<T> : Specification
    {
        protected T Data { get; }

        protected Specification(T data)
        {
            Data = data;
        }
    }
}
