import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { Order, OrderItem } from '../../shared/models/order.model';
import { CartItem } from '../../shared/models/product.model';

@Injectable({
  providedIn: 'root'
})
export class OrderService {
  private readonly STORAGE_KEY = 'shopping_cart_orders';
  private ordersSubject = new BehaviorSubject<Order[]>([]);
  public orders$: Observable<Order[]> = this.ordersSubject.asObservable();

  constructor() {
    this.loadOrdersFromStorage();
  }

  /**
   * Crea una nueva orden a partir de los items del carrito
   */
  createOrder(cartItems: CartItem[]): Order {
    const subtotal = this.calculateSubtotal(cartItems);
    const tax = subtotal * 0.10; // 10% de impuesto
    const total = subtotal + tax;

    const order: Order = {
      id: this.generateOrderId(),
      orderNumber: this.generateOrderNumber(),
      userId: this.getCurrentUserId(),
      items: cartItems.map(item => ({
        id: item.product.id,
        name: item.product.name,
        price: item.product.price,
        quantity: item.quantity,
        imageUrl: item.product.imageUrl || '',
        subtotal: item.product.price * item.quantity
      })),
      subtotal: subtotal,
      tax: tax,
      total: total,
      status: 'completed',
      createdAt: new Date(),
      orderDate: new Date().toLocaleDateString('es-PE', {
        year: 'numeric',
        month: 'long',
        day: 'numeric',
        hour: '2-digit',
        minute: '2-digit'
      })
    };

    return order;
  }

  /**
   * Guarda una orden y la persiste en localStorage
   */
  saveOrder(order: Order): void {
    const existingOrders = this.ordersSubject.value;
    const updatedOrders = [order, ...existingOrders];
    this.ordersSubject.next(updatedOrders);
    this.saveOrdersToStorage(updatedOrders);
  }

  /**
   * Obtiene todas las órdenes del usuario actual
   */
  getUserOrders(): Observable<Order[]> {
    return this.orders$;
  }

  /**
   * Obtiene una orden por su ID
   */
  getOrderById(orderId: string): Order | undefined {
    return this.ordersSubject.value.find(order => order.id === orderId);
  }

  /**
   * Obtiene el total de órdenes completadas
   */
  getTotalOrders(): number {
    return this.ordersSubject.value.filter(order => order.status === 'completed').length;
  }

  /**
   * Obtiene el total gastado en todas las órdenes
   */
  getTotalSpent(): number {
    return this.ordersSubject.value
      .filter(order => order.status === 'completed')
      .reduce((sum, order) => sum + order.total, 0);
  }

  /**
   * Verifica si un producto está en alguna orden completada
   */
  isProductPurchased(productId: string): boolean {
    return this.ordersSubject.value.some(order =>
      order.status === 'completed' &&
      order.items.some(item => item.id === productId)
    );
  }

  /**
   * Obtiene todas las órdenes en las que aparece un producto específico
   */
  getOrdersWithProduct(productId: string): Order[] {
    return this.ordersSubject.value.filter(order =>
      order.items.some(item => item.id === productId)
    );
  }

  /**
   * Cancela una orden
   */
  cancelOrder(orderId: string): void {
    const orders = this.ordersSubject.value.map(order =>
      order.id === orderId ? { ...order, status: 'cancelled' as const } : order
    );
    this.ordersSubject.next(orders);
    this.saveOrdersToStorage(orders);
  }

  /**
   * Calcula el subtotal del carrito
   */
  private calculateSubtotal(cartItems: CartItem[]): number {
    return cartItems.reduce((sum, item) => sum + (item.product.price * item.quantity), 0);
  }

  /**
   * Obtiene el ID del usuario actual
   */
  private getCurrentUserId(): string {
    // Intenta obtener el usuario del token o del localStorage
    const user = localStorage.getItem('currentUser');
    if (user) {
      try {
        return JSON.parse(user).id || 'anonymous';
      } catch {
        return 'anonymous';
      }
    }
    return 'anonymous';
  }

  /**
   * Genera un ID único para la orden
   */
  private generateOrderId(): string {
    return `order_${Date.now()}_${Math.random().toString(36).substr(2, 9)}`;
  }

  /**
   * Genera un número de orden legible para el usuario
   */
  private generateOrderNumber(): string {
    const timestamp = Date.now();
    const random = Math.floor(Math.random() * 10000);
    return `ORD-${timestamp}-${random}`.slice(-12);
  }

  /**
   * Guarda las órdenes en localStorage
   */
  private saveOrdersToStorage(orders: Order[]): void {
    try {
      localStorage.setItem(this.STORAGE_KEY, JSON.stringify(orders));
    } catch (error) {
      console.error('Error saving orders to storage:', error);
    }
  }

  /**
   * Carga las órdenes desde localStorage
   */
  private loadOrdersFromStorage(): void {
    try {
      const storedOrders = localStorage.getItem(this.STORAGE_KEY);
      if (storedOrders) {
        const orders = JSON.parse(storedOrders).map((order: any) => ({
          ...order,
          createdAt: new Date(order.createdAt)
        }));
        this.ordersSubject.next(orders);
      }
    } catch (error) {
      console.error('Error loading orders from storage:', error);
      this.ordersSubject.next([]);
    }
  }

  /**
   * Limpia todas las órdenes (solo para demo/debug)
   */
  clearAllOrders(): void {
    this.ordersSubject.next([]);
    localStorage.removeItem(this.STORAGE_KEY);
  }
}
