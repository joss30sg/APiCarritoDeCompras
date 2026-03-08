/**
 * Product Entity
 * Representa un producto en el dominio de la aplicación
 */
export class Product {
  constructor(
    readonly id: string,
    readonly name: string,
    readonly description: string,
    readonly price: number,
    readonly quantity: number,
    readonly imageUrl: string,
    readonly createdAt: Date
  ) {}

  static create(
    id: string,
    name: string,
    description: string,
    price: number,
    quantity: number,
    imageUrl: string,
    createdAt: Date = new Date()
  ): Product {
    return new Product(id, name, description, price, quantity, imageUrl, createdAt);
  }

  isAvailable(): boolean {
    return this.quantity > 0;
  }

  getDiscountedPrice(discountPercent: number): number {
    return this.price * (1 - discountPercent / 100);
  }
}
