/**
 * HTTP Status Codes
 * Constantes de códigos de estado HTTP
 */
export const HTTP_STATUS = {
  OK: 200,
  CREATED: 201,
  BAD_REQUEST: 400,
  UNAUTHORIZED: 401,
  FORBIDDEN: 403,
  NOT_FOUND: 404,
  INTERNAL_SERVER_ERROR: 500
};

/**
 * Error Messages
 * Mensajes de error comunes
 */
export const ERROR_MESSAGES = {
  INVALID_CREDENTIALS: 'Email o contraseña incorrectos',
  USER_NOT_FOUND: 'Usuario no encontrado',
  EMAIL_ALREADY_EXISTS: 'El email ya está registrado',
  INVALID_TOKEN: 'Token inválido o expirado',
  UNAUTHORIZED: 'No autorizado',
  SERVER_ERROR: 'Error del servidor. Intenta más tarde',
  NETWORK_ERROR: 'Error de conexión. Verifica tu conexión'
};

/**
 * Success Messages
 * Mensajes de éxito comunes
 */
export const SUCCESS_MESSAGES = {
  LOGIN_SUCCESS: 'Sesión iniciada correctamente',
  REGISTER_SUCCESS: 'Registro completado correctamente',
  LOGOUT_SUCCESS: 'Sesión cerrada correctamente',
  PRODUCT_ADDED: 'Producto agregado al carrito',
  PRODUCT_REMOVED: 'Producto removido del carrito',
  QUANTITY_UPDATED: 'Cantidad actualizada'
};
