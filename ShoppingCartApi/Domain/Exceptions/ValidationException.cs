using System;
using System.Collections.Generic;
using System.Linq;

namespace ShoppingCartApi.Domain.Exceptions
{
    public class ValidationException : Exception
    {
        public IDictionary<string, string[]> Errors { get; }

        public ValidationException()
            : base("One or more validation failures have occurred.")
        {
            Errors = new Dictionary<string, string[]>();
        }

        public ValidationException(IEnumerable<string>? failures)
            : base(failures?.FirstOrDefault() ?? "One or more validation failures have occurred.")
        {
            Errors = (failures ?? new List<string>())
                .GroupBy(e => "General") // Agrupar todos los errores bajo una clave "General"
                .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());
        }
    }
}
