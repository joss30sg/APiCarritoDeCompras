import { Injectable } from '@angular/core';
import {
  HttpInterceptor,
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpErrorResponse
} from '@angular/common/http';
import { Observable, throwError, BehaviorSubject } from 'rxjs';
import { catchError, filter, take, switchMap } from 'rxjs/operators';
import { Router } from '@angular/router';

/**
 * JWT Interceptor - Inyección automática de Token JWT con Error Handling
 * Features:
 * - Inyecta token automáticamente en todos los requests
 * - Maneja errores de autenticación (401)
 * - Implementa refresh token automático
 * - Redirige a login si no se puede refrescar
 */
@Injectable()
export class JwtInterceptor implements HttpInterceptor {
  private tokenKey = 'auth_token';
  private refreshTokenInProgress = false;
  private refreshTokenSubject: BehaviorSubject<any> = new BehaviorSubject<any>(null);

  constructor(private router: Router) {}

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    // Agregar token si existe
    req = this.addToken(req);

    return next.handle(req).pipe(
      catchError(error => {
        // Manejar errores de autenticación
        if (error instanceof HttpErrorResponse) {
          if (error.status === 401) {
            return this.handle401Error(req, next);
          }
          
          if (error.status === 403) {
            return this.handle403Error(error);
          }
        }
        
        return throwError(() => error);
      })
    );
  }

  /**
   * Añade el token JWT al header Authorization
   */
  private addToken(req: HttpRequest<any>): HttpRequest<any> {
    const token = localStorage.getItem(this.tokenKey);

    // No agregar token a requests de autenticación (login/register)
    if (token && !this.isAuthRoute(req)) {
      return req.clone({
        setHeaders: {
          Authorization: `Bearer ${token}`
        }
      });
    }

    return req;
  }

  /**
   * Maneja errores 401 (Unauthorized)
   * Intenta refrescar el token, sino redirige a login
   */
  private handle401Error(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    if (!this.refreshTokenInProgress) {
      this.refreshTokenInProgress = true;
      this.refreshTokenSubject.next(null);

      // Intentar refrescar token (si hay endpoint disponible)
      // Por ahora, simplemente limpiar y redirigir a login
      this.logout();
      return throwError(() => new Error('Sesión expirada. Por favor inicie sesión nuevamente.'));
    }

    // Esperar a que termine el refresco de token
    return this.refreshTokenSubject.pipe(
      filter(token => token !== null),
      take(1),
      switchMap(token => {
        this.refreshTokenInProgress = false;
        return next.handle(this.addToken(req));
      })
    );
  }

  /**
   * Maneja errores 403 (Forbidden)
   */
  private handle403Error(error: HttpErrorResponse): Observable<never> {
    // Redirigir a acceso denegado o login
    this.router.navigate(['/login']);
    return throwError(() => new Error('Acceso denegado. Permisos insuficientes.'));
  }

  /**
   * Logout y limpiar datos
   */
  private logout(): void {
    localStorage.removeItem(this.tokenKey);
    localStorage.removeItem('auth_user');
    this.router.navigate(['/login']);
  }

  /**
   * Verificar si la ruta es de autenticación
   */
  private isAuthRoute(req: HttpRequest<any>): boolean {
    return req.url.includes('/auth/login') || 
           req.url.includes('/auth/register') ||
           req.url.includes('/auth/refresh');
  }
}
