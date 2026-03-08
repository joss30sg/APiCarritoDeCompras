import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { ProductService } from '../../../application/services/product.service';
import { OrderService } from '../../../application/services/order.service';
import { CartItem } from '../../../shared/models/product.model';
import { Order } from '../../../shared/models/order.model';

@Component({
  selector: 'app-shopping-cart',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './shopping-cart.component.html',
  styleUrls: ['./shopping-cart.component.scss']
})
export class ShoppingCartComponent implements OnInit, OnDestroy {
  cartItems: CartItem[] = [];
  subtotal: number = 0;
  taxAmount: number = 0;
  totalPrice: number = 0;
  showOrderConfirmation: boolean = false;
  lastOrder: Order | null = null;
  isProcessing: boolean = false;
  showDuplicatePurchaseModal: boolean = false;
  duplicateProducts: { name: string; productId: string }[] = [];

  private destroy$ = new Subject<void>();

  constructor(
    private productService: ProductService,
    private orderService: OrderService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.productService.cart$
      .pipe(takeUntil(this.destroy$))
      .subscribe((items: CartItem[]) => {
        this.cartItems = items;
        this.updatePrices();
      });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  /**
   * Actualiza los cálculos de precios
   */
  private updatePrices(): void {
    this.subtotal = this.productService.getTotalPrice();
    this.taxAmount = this.subtotal * 0.10; // 10% de impuesto
    this.totalPrice = this.subtotal + this.taxAmount;
  }

  /**
   * Obtiene el subtotal de un item específico
   */
  getItemSubtotal(item: CartItem): number {
    return item.product.price * item.quantity;
  }

  /**
   * Elimina un producto del carrito
   */
  removeFromCart(productId: string): void {
    this.productService.removeFromCart(productId);
  }

  /**
   * Limpia todo el carrito
   */
  clearCart(): void {
    if (confirm('⚠️ ¿Estás seguro de que deseas vaciar el carrito?\n\nEsta acción no se puede deshacer.')) {
      this.productService.clearCart();
    }
  }

  /**
   * Procesa el checkout y crea una orden
   */
  checkout(): void {
    if (this.cartItems.length === 0) {
      alert('❌ El carrito está vacío.\n\nAgrega productos antes de proceder al pago.');
      return;
    }

    // Validar que no se intente comprar productos que ya fueron comprados
    const duplicates = this.cartItems.filter(item =>
      this.orderService.isProductPurchased(item.product.id)
    );

    if (duplicates.length > 0) {
      this.duplicateProducts = duplicates.map(item => ({
        name: item.product.name,
        productId: item.product.id
      }));
      this.showDuplicatePurchaseModal = true;
      return;
    }

    this.isProcessing = true;

    // Simular procesamiento de pago (en una app real, usarías Stripe, PayPal, etc.)
    setTimeout(() => {
      try {
        // Crear y guardar la orden
        const order = this.orderService.createOrder(this.cartItems);
        this.orderService.saveOrder(order);
        this.lastOrder = order;

        // Mostrar confirmación
        this.showOrderConfirmation = true;
        this.isProcessing = false;

        // Limpiar carrito después de 2 segundos
        setTimeout(() => {
          this.productService.clearCart();
        }, 500);

      } catch (error) {
        console.error('Error durante el checkout:', error);
        alert('❌ Ocurrió un error durante el pago. Por favor, intenta nuevamente.');
        this.isProcessing = false;
      }
    }, 1500);
  }

  /**
   * Cierra el modal de compra duplicada
   */
  closeDuplicatePurchaseModal(): void {
    this.showDuplicatePurchaseModal = false;
    this.duplicateProducts = [];
  }

  /**
   * Elimina los productos duplicados del carrito
   */
  removeDuplicateProducts(): void {
    this.duplicateProducts.forEach(product => {
      this.productService.removeFromCart(product.productId);
    });
    this.closeDuplicatePurchaseModal();
  }

  /**
   * Cierra la confirmación de orden y regresa a productos
   */
  closeOrderConfirmation(): void {
    this.showOrderConfirmation = false;
    this.lastOrder = null;
    this.continueShopping();
  }

  /**
   * Abre el historial de órdenes
   */
  viewOrderHistory(): void {
    this.router.navigate(['/order-history']);
  }

  /**
   * Regresa a la página de productos
   */
  continueShopping(): void {
    this.router.navigate(['/']);
  }
}
