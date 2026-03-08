import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ProductListComponent } from '../../components/product-list/product-list.component';
import { AuthService } from '../../../application/services/auth.service';
import { AuthState } from '../../../shared/models/auth.model';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

/**
 * Home Page Component
 * Página principal de la aplicación con mensaje de bienvenida personalizado
 */
@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, ProductListComponent],
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit, OnDestroy {
  authState: AuthState = {
    isAuthenticated: false,
    user: null,
    token: null,
    error: null
  };
  greeting: string = '';
  userName: string = '';
  private destroy$ = new Subject<void>();

  constructor(private authService: AuthService) {}

  ngOnInit(): void {
    // Obtener estado inicial
    this.authState = this.authService.getAuthState();
    this.updateGreeting();

    // Suscribirse a cambios de autenticación
    this.authService.authState$
      .pipe(takeUntil(this.destroy$))
      .subscribe((state: AuthState) => {
        this.authState = state;
        this.updateGreeting();
      });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  /**
   * Actualiza el saludo y el nombre del usuario
   */
  private updateGreeting(): void {
    this.greeting = this.getGreeting();
    this.userName = this.authState.user?.username || '';
  }

  /**
   * Obtiene el saludo según la hora del día
   * @returns Saludo personalizado (Buenos días, Buenas tardes, Buenas noches)
   */
  private getGreeting(): string {
    const hour = new Date().getHours();

    if (hour >= 6 && hour < 12) {
      return 'Buenos días';
    } else if (hour >= 12 && hour < 18) {
      return 'Buenas tardes';
    } else {
      return 'Buenas noches';
    }
  }
}

