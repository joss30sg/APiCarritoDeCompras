/**
 * Base Exception Class
 * Clase base para todas las excepciones del dominio
 */
export class DomainException extends Error {
  constructor(message: string) {
    super(message);
    this.name = 'DomainException';
  }
}

/**
 * Entidad no encontrada
 */
export class EntityNotFoundException extends DomainException {
  constructor(entityName: string, id: string) {
    super(`${entityName} with id: ${id} not found`);
    this.name = 'EntityNotFoundException';
  }
}

/**
 * Operación inválida
 */
export class InvalidOperationException extends DomainException {
  constructor(message: string) {
    super(message);
    this.name = 'InvalidOperationException';
  }
}

/**
 * Carrito vacío
 */
export class EmptyShoppingCartException extends DomainException {
  constructor() {
    super('Shopping cart is empty');
    this.name = 'EmptyShoppingCartException';
  }
}

/**
 * Cantidad insuficiente
 */
export class InsufficientQuantityException extends DomainException {
  constructor(productName: string, requested: number, available: number) {
    super(
      `Insufficient quantity for product: ${productName}. Requested: ${requested}, Available: ${available}`
    );
    this.name = 'InsufficientQuantityException';
  }
}
