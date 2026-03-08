import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { OrderService } from '../../../application/services/order.service';
import { Order } from '../../../shared/models/order.model';

@Component({
  selector: 'app-order-history',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './order-history.component.html',
  styleUrls: ['./order-history.component.scss']
})
export class OrderHistoryComponent implements OnInit, OnDestroy {
  orders: Order[] = [];
  selectedOrder: Order | null = null;
  totalOrders: number = 0;
  totalSpent: number = 0;
  private destroy$ = new Subject<void>();

  constructor(
    private orderService: OrderService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadOrders();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  /**
   * Carga las órdenes del usuario
   */
  private loadOrders(): void {
    this.orderService.orders$
      .pipe(takeUntil(this.destroy$))
      .subscribe(orders => {
        this.orders = orders.filter(o => o.status === 'completed');
        this.totalOrders = this.orderService.getTotalOrders();
        this.totalSpent = this.orderService.getTotalSpent();
      });
  }

  /**
   * Selecciona una orden para ver sus detalles
   */
  selectOrder(order: Order): void {
    this.selectedOrder = order;
  }

  /**
   * Cierra el modal de detalles de la orden
   */
  closeOrderDetails(): void {
    this.selectedOrder = null;
  }

  /**
   * Regresa al carrito
   */
  backToCart(): void {
    this.router.navigate(['/cart']);
  }

  /**
   * Obtiene el estado de una orden en formato legible
   */
  getOrderStatusLabel(status: string): string {
    const labels: { [key: string]: string } = {
      'completed': '✅ Completada',
      'pending': '⏳ Pendiente',
      'cancelled': '❌ Cancelada'
    };
    return labels[status] || status;
  }

  /**
   * Obtiene el color del estado para el badge
   */
  getStatusColor(status: string): string {
    const colors: { [key: string]: string } = {
      'completed': 'success',
      'pending': 'warning',
      'cancelled': 'danger'
    };
    return colors[status] || 'default';
  }

  /**
   * Calcula cuánto tiempo ha pasado desde la orden
   */
  getTimeAgo(createdAt: Date): string {
    const now = new Date();
    const date = new Date(createdAt);
    const diffMs = now.getTime() - date.getTime();
    const diffMins = Math.floor(diffMs / 60000);
    const diffHours = Math.floor(diffMs / 3600000);
    const diffDays = Math.floor(diffMs / 86400000);

    if (diffMins < 1) return 'hace poco';
    if (diffMins < 60) return `hace ${diffMins} min`;
    if (diffHours < 24) return `hace ${diffHours}h`;
    if (diffDays < 30) return `hace ${diffDays}d`;
    return date.toLocaleDateString('es-PE');
  }
}
