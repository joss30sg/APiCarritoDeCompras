import { inject } from '@angular/core';
import { Router, CanActivateFn, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { AuthService } from '../../application/services/auth.service';

/**
 * Authentication Guard (Función)
 * Guard para proteger rutas que requieren autenticación
 */
export const authGuard: CanActivateFn = (
  route: ActivatedRouteSnapshot,
  state: RouterStateSnapshot
) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  // Verificar si existe token válido
  if (authService.isTokenValid()) {
    return true;
  }

  // Redirigir a login
  router.navigate(['/login'], { queryParams: { returnUrl: state.url } });
  return false;
};

