import { Product } from './product.entity';

/**
 * Shopping Cart Item Entity
 * Representa un artículo en el carrito de compras
 */
export class ShoppingCartItem {
  constructor(
    readonly id: string,
    readonly product: Product,
    readonly quantity: number
  ) {}

  static create(
    id: string,
    product: Product,
    quantity: number = 1
  ): ShoppingCartItem {
    return new ShoppingCartItem(id, product, quantity);
  }

  getTotalPrice(): number {
    return this.product.price * this.quantity;
  }

  updateQuantity(newQuantity: number): ShoppingCartItem {
    return new ShoppingCartItem(this.id, this.product, newQuantity);
  }
}
