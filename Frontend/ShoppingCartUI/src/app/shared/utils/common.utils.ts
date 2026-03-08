/**
 * Common Utilities
 * Utilidades comunes de la aplicación
 */

export class CommonUtils {
  /**
   * Formatear número como moneda
   */
  static formatCurrency(value: number, symbol: string = '$'): string {
    return `${symbol}${value.toFixed(2).replace(/\B(?=(\d{3})+(?!\d))/g, ',')}`;
  }

  /**
   * Formatear fecha
   */
  static formatDate(date: Date, format: string = 'dd/MM/yyyy'): string {
    const d = new Date(date);
    const day = String(d.getDate()).padStart(2, '0');
    const month = String(d.getMonth() + 1).padStart(2, '0');
    const year = d.getFullYear();

    return format
      .replace('dd', day)
      .replace('MM', month)
      .replace('yyyy', String(year));
  }

  /**
   * Generar UUID
   */
  static generateUUID(): string {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
      const r = (Math.random() * 16) | 0;
      const v = c === 'x' ? r : (r & 0x3) | 0x8;
      return v.toString(16);
    });
  }

  /**
   * Delay (útil para pruebas)
   */
  static delay(ms: number): Promise<void> {
    return new Promise(resolve => setTimeout(resolve, ms));
  }

  /**
   * Deep clone de objeto
   */
  static deepClone<T>(obj: T): T {
    return JSON.parse(JSON.stringify(obj));
  }
}
