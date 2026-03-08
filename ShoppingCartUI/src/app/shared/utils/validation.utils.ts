/**
 * Validation Utilities
 * Utilidades para validación de datos
 */

export class ValidationUtils {
  /**
   * Validar email
   */
  static isValidEmail(email: string): boolean {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailRegex.test(email);
  }

  /**
   * Validar contraseña (mínimo 8 caracteres, al menos 1 mayúscula, 1 minúscula y 1 número)
   */
  static isValidPassword(password: string): boolean {
    const passwordRegex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$/;
    return passwordRegex.test(password);
  }

  /**
   * Validar que no esté vacío
   */
  static isNotEmpty(value: string): boolean {
    return !!value && value.trim().length > 0;
  }

  /**
   * Validar número positivo
   */
  static isPositiveNumber(value: number): boolean {
    return !isNaN(value) && value > 0;
  }
}
