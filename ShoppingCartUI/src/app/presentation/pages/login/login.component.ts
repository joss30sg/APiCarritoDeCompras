import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { AuthService } from '../../../application/services/auth.service';
import { LoginRequest } from '../../../shared/models/auth.model';

/**
 * Login Page Component
 * Página de autenticación del usuario con buenas prácticas
 */
@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {
  loginForm!: FormGroup;
  loading = false;
  submitted = false;
  returnUrl: string = '';
  showPassword = false;

  constructor(
    private formBuilder: FormBuilder,
    private authService: AuthService,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    console.log('LoginComponent: ngOnInit called');
    console.log('LoginComponent: Token valid?', this.authService.isTokenValid());
    console.log('LoginComponent: Auth state:', this.authService.getAuthState());
    
    // Si ya está autenticado, redirigir a home
    if (this.authService.isTokenValid()) {
      console.log('LoginComponent: Usuario ya autenticado, redirigiendo a home');
      this.router.navigate(['/']);
      return;
    }

    console.log('LoginComponent: Usuario no autenticado, mostrando formulario de login');
    
    // Obtener URL de retorno
    this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/';

    // Inicializar formulario reactivo
    this.initializeForm();
  }

  initializeForm(): void {
    this.loginForm = this.formBuilder.group({
      username: ['', [Validators.required, Validators.minLength(3)]],
      password: ['', [Validators.required, Validators.minLength(6)]]
    });
  }

  get getErrorMessage(): { [key: string]: string } {
    return {
      usernameRequired: 'El usuario es requerido',
      usernameMinLength: 'El usuario debe tener al menos 3 caracteres',
      passwordRequired: 'La contraseña es requerida',
      passwordMinLength: 'La contraseña debe tener al menos 6 caracteres'
    };
  }

  get f() {
    return this.loginForm.controls;
  }

  togglePasswordVisibility(): void {
    this.showPassword = !this.showPassword;
  }

  onSubmit(): void {
    this.submitted = true;

    if (this.loginForm.invalid) {
      return;
    }

    this.loading = true;
    const loginRequest: LoginRequest = {
      username: this.f['username'].value,
      password: this.f['password'].value
    };

    console.log('[LoginComponent] Enviando login request:', { 
      username: loginRequest.username,
      // NO mostrar password en console por seguridad
    });

    this.authService.login(loginRequest).subscribe({
      next: (response) => {
        console.log('[LoginComponent] Login exitoso, respuesta:', response);
        console.log('[LoginComponent] Token guardado en localStorage:', this.authService.getToken());
        this.loading = false;
        this.router.navigate([this.returnUrl]);
      },
      error: (error) => {
        console.error('[LoginComponent] Error de login:', error);
        console.error('[LoginComponent] Estado del token:', {
          tokenExists: !!this.authService.getToken(),
          isAuthenticated: this.authService.isAuthenticated()
        });
        this.loading = false;
      }
    });
  }

  goToRegister(): void {
    this.router.navigate(['/register']);
  }

  // Demo login para testing
  demoLogin(): void {
    this.loginForm.patchValue({
      username: 'testuser',
      password: 'Password123!'
    });
  }
}
