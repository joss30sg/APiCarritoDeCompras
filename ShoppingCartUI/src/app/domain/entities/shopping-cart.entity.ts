import { ShoppingCartItem } from './shopping-cart-item.entity';

/**
 * Shopping Cart Entity
 * Representa el carrito de compras del usuario
 */
export class ShoppingCart {
  constructor(
    readonly id: string,
    readonly userId: string,
    readonly items: ShoppingCartItem[] = [],
    readonly createdAt: Date = new Date(),
    readonly updatedAt: Date = new Date()
  ) {}

  static create(userId: string, id: string = ''): ShoppingCart {
    return new ShoppingCart(id, userId, [], new Date(), new Date());
  }

  addItem(item: ShoppingCartItem): ShoppingCart {
    const existingItem = this.items.find(i => i.product.id === item.product.id);
    const newItems = existingItem
      ? this.items.map(i =>
          i.product.id === item.product.id
            ? i.updateQuantity(i.quantity + item.quantity)
            : i
        )
      : [...this.items, item];

    return new ShoppingCart(this.id, this.userId, newItems, this.createdAt, new Date());
  }

  removeItem(itemId: string): ShoppingCart {
    const newItems = this.items.filter(i => i.id !== itemId);
    return new ShoppingCart(this.id, this.userId, newItems, this.createdAt, new Date());
  }

  updateItemQuantity(itemId: string, quantity: number): ShoppingCart {
    const newItems = this.items.map(i =>
      i.id === itemId ? i.updateQuantity(quantity) : i
    );
    return new ShoppingCart(this.id, this.userId, newItems, this.createdAt, new Date());
  }

  getTotalPrice(): number {
    return this.items.reduce((total, item) => total + item.getTotalPrice(), 0);
  }

  getTotalItems(): number {
    return this.items.reduce((total, item) => total + item.quantity, 0);
  }

  isEmpty(): boolean {
    return this.items.length === 0;
  }

  clear(): ShoppingCart {
    return new ShoppingCart(this.id, this.userId, [], this.createdAt, new Date());
  }
}
