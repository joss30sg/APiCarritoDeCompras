import { inject } from '@angular/core';
import { Router, CanActivateFn, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { AuthService } from '../../application/services/auth.service';

/**
 * No Auth Guard (Función)
 * Guard para proteger rutas que NO deben ser accesibles si el usuario está autenticado
 * Útil para la página de login y registro
 */
export const noAuthGuard: CanActivateFn = (
  route: ActivatedRouteSnapshot,
  state: RouterStateSnapshot
) => {
  try {
    const authService = inject(AuthService);
    const router = inject(Router);

    // Permitir acceso solo si no hay token válido
    const hasToken = authService.isTokenValid();
    
    console.log('NoAuthGuard: Checking authentication...', {
      hasToken,
      path: state.url
    });
    
    // Si NO hay token, permitir acceso a login/register
    if (!hasToken) {
      console.log('NoAuthGuard: ✅ Sin autenticación - Permitiendo acceso a', state.url);
      return true;
    }

    // Si hay token, redirigir a home
    console.log('NoAuthGuard: ❌ Con autenticación - Redirigiendo a / desde', state.url);
    router.navigate(['/']);
    return false;
  } catch (error) {
    console.error('NoAuthGuard: Error en guard', error);
    // En caso de error, permitir acceso para evitar bloqueos
    return true;
  }
};

