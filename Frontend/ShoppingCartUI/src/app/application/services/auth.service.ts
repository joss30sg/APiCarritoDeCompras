import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject, tap, catchError, throwError } from 'rxjs';
import { LoginRequest, RegisterRequest, AuthResponse, AuthState } from '../../shared/models/auth.model';
import { JwtTokenService } from '../../infrastructure/interceptors/jwt-token.service';

/**
 * Authentication Service - Repository Pattern (REFACTORED)
 * 
 * Responsabilidad ÚNICA:
 * - Realizar llamadas HTTP a la API de autenticación
 * - NO maneja estado global (eso es responsabilidad de AuthEffects/Store)
 * - NO guarda tokens en localStorage (eso es responsabilidad de AuthEffects)
 * 
 * Patrón: Repository Pattern + Dependency Injection
 * Principios SOLID:
 * - Single Responsibility: Solo HTTP calls
 * - Dependency Inversion: Inyecta HttpClient
 * 
 * El flujo de autenticación es:
 * 1. Componente dispatch AuthActions.login()
 * 2. AuthEffects captura la acción y llama a AuthService.login()
 * 3. AuthService hace HTTP call a la API
 * 4. AuthEffects recibe la respuesta y guarda en localStorage + Store
 * 5. Componente se suscribe al Store y obtiene estado actualizado
 * 
 * @example
 * // En un componente
 * this.store.dispatch(AuthActions.login({ request: credentials }));
 * 
 * // En effects
 * login$ = createEffect(() =>
 *   this.actions$.pipe(
 *     ofType(AuthActions.login),
 *     switchMap(({ request }) =>
 *       this.authService.login(request).pipe(...)
 *     )
 *   )
 * );
 */
@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly apiUrl = 'http://localhost:5276/api/v1/auth';
  
  // Estado de autenticación
  private authStateSubject = new BehaviorSubject<AuthState>({
    isAuthenticated: false,
    user: null,
    token: null,
    error: null
  });
  
  authState$ = this.authStateSubject.asObservable();

  constructor(
    private httpClient: HttpClient,
    private jwtTokenService: JwtTokenService
  ) {
    // Inicializar estado con token existente
    this.initializeAuthState();
  }

  /**
   * Inicializar el estado de autenticación con el token actual
   */
  private initializeAuthState(): void {
    const token = this.jwtTokenService.getToken();
    if (token && !this.jwtTokenService.isTokenExpired(token)) {
      this.authStateSubject.next({
        isAuthenticated: true,
        user: null,
        token: token,
        error: null
      });
    }
  }

  /**
   * HTTP call para registrar un nuevo usuario
   * 
   * @param request - Datos de registro (username, password)
   * @returns Observable<any> con la respuesta del servidor
   *          Estructura esperada: { data: { token, refreshToken, user } }
   */
  register(request: RegisterRequest): Observable<any> {
    return this.httpClient.post<any>(`${this.apiUrl}/register`, request).pipe(
      tap((response: any) => {
        console.log('[AuthService] Respuesta de registro recibida:', response);
        
        // Extraer token de la respuesta (estructura: { success, data: { token, ... }, ... })
        const token = response?.data?.token;
        if (token) {
          this.jwtTokenService.setToken(token);
          this.updateAuthState({
            isAuthenticated: true,
            token: token,
            error: null
          });
          console.log('[AuthService] Registro exitoso, token guardado en localStorage');
        } else {
          console.warn('[AuthService] No se encontró token en respuesta de registro');
        }
      }),
      catchError((error) => {
        console.error('[AuthService] Error durante registro:', error);
        this.updateAuthState({
          isAuthenticated: false,
          error: error?.error?.message || 'Error en registro'
        });
        return throwError(() => error);
      })
    );
  }

  /**
   * HTTP call para autenticar un usuario existente
   * 
   * @param request - Credenciales (username, password)
   * @returns Observable<any> con la respuesta del servidor
   *          Estructura esperada: { data: { token, refreshToken, user } }
   */
  login(request: LoginRequest): Observable<any> {
    return this.httpClient.post<any>(`${this.apiUrl}/login`, request).pipe(
      tap((response: any) => {
        console.log('[AuthService] Respuesta de login recibida:', response);
        
        // Extraer token de la respuesta (estructura: { success, data: { token, ... }, ... })
        const token = response?.data?.token;
        
        if (token) {
          // Guardar token en localStorage
          this.jwtTokenService.setToken(token);
          
          // Actualizar estado de autenticación
          this.updateAuthState({
            isAuthenticated: true,
            token: token,
            error: null
          });
          
          console.log('[AuthService] Login exitoso, token guardado en localStorage');
        } else {
          console.error('[AuthService] No se encontró token en la respuesta de login', {
            response,
            dataExists: !!response?.data,
            tokenExists: !!response?.data?.token
          });
          throw new Error('No token in response');
        }
      }),
      catchError((error) => {
        console.error('[AuthService] Error durante login:', error);
        this.updateAuthState({
          isAuthenticated: false,
          error: error?.error?.message || 'Error en autenticación'
        });
        return throwError(() => error);
      })
    );
  }

  /**
   * Obtener el token almacenado en localStorage
   * 
   * NOTA IMPORTANTE:
   * Los componentes deberían usar selectores del store en lugar de esto:
   * this.store.select(selectAuthToken)
   * 
   * Este método es útil solo para guards u otros servicios que necesiten
   * verificar rápidamente el token sin acceder al store.
   * 
   * @returns Token JWT o null si no existe
   */
  getToken(): string | null {
    return this.jwtTokenService.getToken();
  }

  /**
   * Obtener refresh token almacenado en localStorage
   * 
   * @returns Refresh token o null si no existe
   */
  getRefreshToken(): string | null {
    return this.jwtTokenService.getRefreshToken();
  }

  /**
   * Verificar si el usuario está autenticado
   * 
   * NOTA IMPORTANTE:
   * Los componentes deberían usar selectores del store en lugar de esto:
   * this.store.select(selectIsAuthenticated)
   * 
   * Este método verifica:
   * 1. Que exista un token en localStorage
   * 2. Que el token no esté expirado
   * 
   * @returns true si hay token válido y no expirado, false en caso contrario
   */
  isAuthenticated(): boolean {
    const token = this.jwtTokenService.getToken();
    if (!token) return false;
    return !this.jwtTokenService.isTokenExpired(token);
  }

  /**
   * Verificar si el token ha expirado
   * 
   * @param token - Token JWT (opcional, usa el actual si se omite)
   * @returns true si ha expirado, false si todavía es válido
   */
  isTokenExpired(token?: string): boolean {
    return this.jwtTokenService.isTokenExpired(token);
  }

  /**
   * Obtener fecha de expiración del token actual
   * 
   * @returns Objeto Date con la expiración, o null si no es válida
   */
  getTokenExpiration(): Date | null {
    return this.jwtTokenService.getTokenExpiration();
  }

  /**
   * Alias para isAuthenticated() - Verificar si el token es válido
   * Se mantiene para compatibilidad con componentes
   * 
   * @returns true si hay token válido y no expirado, false en caso contrario
   */
  isTokenValid(): boolean {
    return this.isAuthenticated();
  }

  /**
   * Obtener el estado actual de autenticación
   * 
   * @returns Estado actual de autenticación
   */
  getAuthState(): AuthState {
    return this.authStateSubject.getValue();
  }

  /**
   * Actualizar el estado de autenticación
   * (Uso interno del servicio)
   */
  private updateAuthState(state: Partial<AuthState>): void {
    const current = this.authStateSubject.getValue();
    this.authStateSubject.next({
      ...current,
      ...state
    });
  }

  /**
   * Limpiar toda la sesión del usuario
   * Elimina tokens del localStorage
   * 
   * NOTA: Esto es llamado por AuthEffects al hacer logout,
   * pero se incluye aquí para permitir logout de emergencia si es necesario.
   */
  logout(): void {
    this.jwtTokenService.removeToken();
    this.authStateSubject.next({
      isAuthenticated: false,
      user: null,
      token: null,
      error: null
    });
  }
}
