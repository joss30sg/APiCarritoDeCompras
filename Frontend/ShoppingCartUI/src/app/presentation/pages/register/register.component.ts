import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthApiService } from '../../../infrastructure/api/auth.api.service';
import { JwtTokenService } from '../../../infrastructure/interceptors/jwt-token.service';

/**
 * Register Page Component
 * Página de registro de nuevos usuarios
 */
@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent implements OnInit {
  email: string = '';
  name: string = '';
  password: string = '';
  confirmPassword: string = '';
  loading: boolean = false;
  error: string = '';

  constructor(
    private authService: AuthApiService,
    private jwtTokenService: JwtTokenService,
    private router: Router
  ) {}

  ngOnInit(): void {
    // Si ya está autenticado, redirigir a home
    if (this.jwtTokenService.hasToken() && !this.jwtTokenService.isTokenExpired()) {
      this.router.navigate(['/home']);
    }
  }

  async register(): Promise<void> {
    if (!this.email || !this.password || !this.name || !this.confirmPassword) {
      this.error = 'Por favor completa todos los campos';
      return;
    }

    if (this.password !== this.confirmPassword) {
      this.error = 'Las contraseñas no coinciden';
      return;
    }

    this.loading = true;
    this.error = '';

    try {
      const response = await this.authService.register(this.email, this.password, this.name);
      this.jwtTokenService.setToken(response.token);
      this.router.navigate(['/']);
    } catch (error: any) {
      this.error = error.message || 'Error al registrar';
    } finally {
      this.loading = false;
    }
  }

  goToLogin(): void {
    this.router.navigate(['/auth/login']);
  }
}
