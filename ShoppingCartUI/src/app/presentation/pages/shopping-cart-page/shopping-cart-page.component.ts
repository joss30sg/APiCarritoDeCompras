import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ShoppingCartComponent } from '../../components/shopping-cart/shopping-cart.component';

/**
 * Shopping Cart Page Component
 * Página dedicada para visualizar el carrito de compras
 */
@Component({
  selector: 'app-shopping-cart-page',
  standalone: true,
  imports: [CommonModule, ShoppingCartComponent],
  templateUrl: './shopping-cart-page.component.html',
  styleUrls: ['./shopping-cart-page.component.scss']
})
export class ShoppingCartPageComponent {}
