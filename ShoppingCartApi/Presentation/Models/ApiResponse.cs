namespace ShoppingCartApi.Presentation.Models
{
    /// <summary>
    /// Envoltorio estándar de respuesta API.
    /// Proporciona estructura consistente para todas las respuestas: éxito/error, datos, mensaje y timestamp.
    /// </summary>
    /// <typeparam name="T">Tipo de datos en la respuesta</typeparam>
    public class ApiResponse<T>
    {
        /// <summary>
        /// Indica si la operación fue exitosa (true) o falló (false).
        /// </summary>
        /// <example>true</example>
        public bool Success { get; set; }

        /// <summary>Código de estado HTTP (200, 201, 400, 401, 500, etc.)</summary>
        /// <example>200</example>
        public int StatusCode { get; set; }

        /// <summary>Mensaje descriptivo de la operación</summary>
        /// <example>Shopping cart retrieved successfully</example>
        public string Message { get; set; } = string.Empty;

        /// <summary>Datos retornados en operaciones exitosas. Null para errores o sin contenido.</summary>
        /// <example>{ "id": 1, "items": [...], "total": 2500.00 }</example>
        public T? Data { get; set; }

        /// <summary>Lista de errores específicos cuando la operación falla</summary>
        /// <example>["Username is required", "Password must be at least 6 characters"]</example>
        public List<string>? Errors { get; set; }

        /// <summary>Momento en UTC cuando se procesó la solicitud (ISO 8601)</summary>
        /// <example>2024-12-20T15:30:45.123Z</example>
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        /// <summary>Crea respuesta exitosa con datos</summary>
        /// <param name="data">Datos a retornar</param>
        /// <param name="message">Descripción de la operación</param>
        /// <param name="statusCode">Código HTTP (default: 200)</param>
        public static ApiResponse<T> SuccessResponse(T? data, string message = "Operation completed successfully", int statusCode = 200)
        {
            return new ApiResponse<T>
            {
                Success = true,
                StatusCode = statusCode,
                Message = message,
                Data = data,
                Timestamp = DateTime.UtcNow
            };
        }

        /// <summary>Crea respuesta de error</summary>
        /// <param name="message">Descripción del error</param>
        /// <param name="statusCode">Código HTTP (default: 400)</param>
        /// <param name="errors">Errores específicos (opcional)</param>
        public static ApiResponse<T> ErrorResponse(string message, int statusCode = 400, List<string>? errors = null)
        {
            return new ApiResponse<T>
            {
                Success = false,
                StatusCode = statusCode,
                Message = message,
                Errors = errors ?? new List<string>(),
                Timestamp = DateTime.UtcNow
            };
        }

        /// <summary>Crea respuesta sin contenido (204 No Content) para DELETE/PUT</summary>
        /// <param name="message">Descripción de la operación</param>
        public static ApiResponse<T> NoContentResponse(string message = "Operation completed successfully")
        {
            return new ApiResponse<T>
            {
                Success = true,
                StatusCode = 204,
                Message = message,
                Timestamp = DateTime.UtcNow
            };
        }
    }

    /// <summary>
    /// Respuesta de API sin tipo de dato genérico.
    /// Se usa cuando la operación no devuelve datos específicos (solo estado).
    /// </summary>
    /// <remarks>
    /// CASOS DE USO:
    /// - Operaciones que solo confirman éxito/fallo sin datos
    /// - Respuestas que no necesitan payload tipado
    /// - Estados simples sin estructura de datos compleja
    /// 
    /// EJEMPLO:
    /// {
    ///   "success": true,
    ///   "statusCode": 200,
    ///   "message": "Operation completed successfully",
    ///   "errors": [],
    ///   "timestamp": "2024-12-20T15:30:45.123Z"
    /// }
    /// </remarks>
    public class ApiResponse
    {
        /// <summary>Indica si la operación fue exitosa</summary>
        /// <example>true</example>
        public bool Success { get; set; }

        /// <summary>Código de estado HTTP</summary>
        /// <example>200</example>
        public int StatusCode { get; set; }

        /// <summary>Mensaje descriptivo de la operación</summary>
        /// <example>Operation completed successfully</example>
        public string Message { get; set; } = string.Empty;

        /// <summary>Lista de errores (si la operación falló)</summary>
        /// <example>[]</example>
        public List<string>? Errors { get; set; }

        /// <summary>Timestamp UTC de cuando se procesó la solicitud</summary>
        /// <example>2024-12-20T15:30:45.123Z</example>
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Crea una respuesta exitosa sin datos.
        /// 
        /// EJEMPLO:
        /// <code>
        /// return Ok(ApiResponse.SuccessResponse(
        ///     message: "User logged out successfully",
        ///     statusCode: 200
        /// ));
        /// </code>
        /// </summary>
        public static ApiResponse SuccessResponse(string message = "Operation completed successfully", int statusCode = 200)
        {
            return new ApiResponse
            {
                Success = true,
                StatusCode = statusCode,
                Message = message,
                Timestamp = DateTime.UtcNow
            };
        }

        /// <summary>
        /// Crea una respuesta de error sin datos.
        /// 
        /// EJEMPLO:
        /// <code>
        /// return BadRequest(ApiResponse.ErrorResponse(
        ///     message: "Invalid input",
        ///     statusCode: 400,
        ///     errors: new List&lt;string&gt; { "Field is required" }
        /// ));
        /// </code>
        /// </summary>
        public static ApiResponse ErrorResponse(string message, int statusCode = 400, List<string>? errors = null)
        {
            return new ApiResponse
            {
                Success = false,
                StatusCode = statusCode,
                Message = message,
                Errors = errors ?? new List<string>(),
                Timestamp = DateTime.UtcNow
            };
        }

        /// <summary>
        /// Crea una respuesta para operaciones exitosas sin contenido.
        /// </summary>
        public static ApiResponse NoContentResponse(string message = "Operation completed successfully")
        {
            return new ApiResponse
            {
                Success = true,
                StatusCode = 204,
                Message = message,
                Timestamp = DateTime.UtcNow
            };
        }
    }
}
