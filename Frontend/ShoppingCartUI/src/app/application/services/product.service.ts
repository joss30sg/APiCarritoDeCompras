import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { Product, CartItem } from '../../shared/models/product.model';

@Injectable({
  providedIn: 'root'
})
export class ProductService {
  private cartSubject = new BehaviorSubject<CartItem[]>([]);
  public cart$: Observable<CartItem[]> = this.cartSubject.asObservable();

  private products: Product[] = [
    {
      id: '1',
      name: 'Laptop Dell XPS 13',
      description: 'Computadora portátil ultradelgada de alto rendimiento',
      price: 4809.96,
      quantity: 10,
      imageUrl: 'https://images.unsplash.com/photo-1517694712202-14dd9538aa97?w=400&h=300&fit=crop'
    },
    {
      id: '2',
      name: 'iPhone 15 Pro',
      description: 'Smartphone de última generación con cámara avanzada',
      price: 3699.96,
      quantity: 15,
      imageUrl: 'https://images.unsplash.com/photo-1592286927505-1def25115558?w=400&h=300&fit=crop'
    },
    {
      id: '3',
      name: 'AirPods Pro',
      description: 'Auriculares inalámbricos con cancelación de ruido',
      price: 924.96,
      quantity: 20,
      imageUrl: 'https://images.unsplash.com/photo-1505740420928-5e560c06d30e?w=400&h=300&fit=crop'
    },
    {
      id: '4',
      name: 'iPad Air',
      description: 'Tablet versátil para trabajo y entretenimiento',
      price: 2219.96,
      quantity: 12,
      imageUrl: 'https://images.unsplash.com/photo-1544244015-0df4b3ffc6b0?w=400&h=300&fit=crop'
    },
    {
      id: '5',
      name: 'Apple Watch Series 9',
      description: 'Reloj inteligente con seguimiento de salud completo',
      price: 1479.96,
      quantity: 18,
      imageUrl: 'https://images.unsplash.com/photo-1523275335684-37898b6baf30?w=400&h=300&fit=crop'
    },
    {
      id: '6',
      name: 'Magic Keyboard',
      description: 'Teclado inalámbrico compacto y elegante',
      price: 369.96,
      quantity: 25,
      imageUrl: 'https://images.unsplash.com/photo-1595225476276-606e3e2ac7d1?w=400&h=300&fit=crop'
    }
  ];

  constructor() {}

  getProducts(): Product[] {
    return this.products;
  }

  getProductById(id: string): Product | undefined {
    return this.products.find(p => p.id === id);
  }

  getCart(): CartItem[] {
    return this.cartSubject.value;
  }

  addToCart(product: Product, quantity: number = 1): void {
    const currentCart = this.cartSubject.value;
    const existingItem = currentCart.find(item => item.product.id === product.id);

    if (existingItem) {
      existingItem.quantity += quantity;
    } else {
      currentCart.push({ product, quantity });
    }

    this.cartSubject.next([...currentCart]);
  }

  removeFromCart(productId: string): void {
    const updatedCart = this.cartSubject.value.filter(item => item.product.id !== productId);
    this.cartSubject.next(updatedCart);
  }

  updateQuantity(productId: string, quantity: number): void {
    const cart = this.cartSubject.value;
    const item = cart.find(item => item.product.id === productId);

    if (item) {
      if (quantity <= 0) {
        this.removeFromCart(productId);
      } else {
        item.quantity = quantity;
        this.cartSubject.next([...cart]);
      }
    }
  }

  clearCart(): void {
    this.cartSubject.next([]);
  }

  getTotalPrice(): number {
    return this.cartSubject.value.reduce((total, item) => {
      return total + (item.product.price * item.quantity);
    }, 0);
  }
}
