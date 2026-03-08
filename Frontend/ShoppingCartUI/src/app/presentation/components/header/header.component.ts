import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { AuthService } from '../../../application/services/auth.service';
import { ProductService } from '../../../application/services/product.service';
import { AuthState } from '../../../shared/models/auth.model';
import { CartItem } from '../../../shared/models/product.model';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss']
})
export class HeaderComponent implements OnInit, OnDestroy {
  authState: AuthState = {
    isAuthenticated: false,
    user: null,
    token: null,
    error: null
  };
  cartItems: CartItem[] = [];
  cartItemCount: number = 0;
  private destroy$ = new Subject<void>();
  showMenu = false;

  constructor(
    private authService: AuthService,
    private productService: ProductService,
    private router: Router
  ) {}

  ngOnInit(): void {
    // Obtener estado inicial
    this.authState = this.authService.getAuthState();

    // Suscribirse a cambios de autenticación
    this.authService.authState$
      .pipe(takeUntil(this.destroy$))
      .subscribe((state: AuthState) => {
        this.authState = state;
      });

    // Suscribirse a cambios del carrito
    this.productService.cart$
      .pipe(takeUntil(this.destroy$))
      .subscribe(items => {
        this.cartItems = items;
        this.cartItemCount = items.reduce((total, item) => total + item.quantity, 0);
      });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  toggleMenu(): void {
    this.showMenu = !this.showMenu;
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
    this.showMenu = false;
  }

  goHome(): void {
    this.router.navigate(['/']);
    this.showMenu = false;
  }

  goLogin(): void {
    this.router.navigate(['/login']);
    this.showMenu = false;
  }

  goToCart(): void {
    this.router.navigate(['/cart']);
    this.showMenu = false;
  }
}
