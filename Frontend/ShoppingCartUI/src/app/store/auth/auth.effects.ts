import { Injectable, inject } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { of } from 'rxjs';
import { map, switchMap, catchError, tap } from 'rxjs/operators';
import { Router } from '@angular/router';
import { JwtTokenService } from '../../infrastructure/interceptors/jwt-token.service';

import * as AuthActions from './auth.actions';
import { AuthService } from '../../application/services/auth.service';

/**
 * Auth Effects - Manejan efectos secundarios (side effects) de autenticación
 * 
 * Responsabilidades:
 * - Escuchar acciones de autenticación (login, register, logout)
 * - Realizar llamadas HTTP a través de AuthService
 * - Guardar/eliminar tokens en localStorage
 * - Navegar el usuario a diferentes rutas
 * - Mapear respuestas HTTP a acciones Redux
 * 
 * Principio: Effects SOLO manejan side effects
 * La lógica de estado debe estar en el Reducer
 * 
 * Flujo de datos:
 * 1. Componente dispatch(AuthActions.login({ request }))
 * 2. Effects escucha login$ y llama AuthService.login()
 * 3. API responde, Effects mapea a loginSuccess() o loginFailure()
 * 4. Reducer actualiza el Store
 * 5. Componente suscrito al Store ve el nuevo estado
 * 
 * ¡IMPORTANTE! Los métodos que manejan localStorage SOLO se ejecutan
 * cuando sucede la acción SUCCESS (no en cada HTTP call).
 */
@Injectable({ providedIn: 'root' })
export class AuthEffects {
  private readonly actions$ = inject(Actions);
  private readonly authService = inject(AuthService);
  private readonly jwtTokenService = inject(JwtTokenService);
  private readonly router = inject(Router);

  /**
   * Effect: Login
   * 
   * 1. Escucha AuthActions.login
   * 2. Llama AuthService.login() (HTTP call)
   * 3. Si éxito: mapea a loginSuccess + guarda token
   * 4. Si error: mapea a loginFailure
   */
  login$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AuthActions.login),
      switchMap(({ request }) =>
        this.authService.login(request).pipe(
          // ✅ Solo cuando ÉXITO del HTTP call
          map(response => {
            // Guardar token ANTES de dispatch la acción
            if (response?.data?.token) {
              this.jwtTokenService.setToken(response.data.token);
            }
            if (response?.data?.refreshToken) {
              this.jwtTokenService.setRefreshToken(response.data.refreshToken);
            }
            return AuthActions.loginSuccess({ 
              response: response?.data 
            });
          }),
          // ❌ Si falla el HTTP call
          catchError(error =>
            of(AuthActions.loginFailure({ 
              error: error?.error?.message || 'Error al iniciar sesión' 
            }))
          )
        )
      )
    )
  );

  /**
   * Effect: Login Success
   * 
   * Cuando login fue exitoso:
   * - Navega a la página de inicio
   * - NO dispatch otras acciones (dispatch: false)
   */
  loginSuccess$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(AuthActions.loginSuccess),
        tap(() => this.router.navigate(['/']))
      ),
    { dispatch: false }
  );

  /**
   * Effect: Register
   * 
   * 1. Escucha AuthActions.register
   * 2. Llama AuthService.register() (HTTP call)
   * 3. Si éxito: mapea a registerSuccess + guarda token
   * 4. Si error: mapea a registerFailure
   */
  register$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AuthActions.register),
      switchMap(({ request }) =>
        this.authService.register(request).pipe(
          // ✅ Solo cuando ÉXITO del HTTP call
          map(response => {
            // Guardar token ANTES de dispatch la acción
            if (response?.data?.token) {
              this.jwtTokenService.setToken(response.data.token);
            }
            if (response?.data?.refreshToken) {
              this.jwtTokenService.setRefreshToken(response.data.refreshToken);
            }
            return AuthActions.registerSuccess({ 
              response: response?.data 
            });
          }),
          // ❌ Si falla el HTTP call
          catchError(error =>
            of(AuthActions.registerFailure({ 
              error: error?.error?.message || 'Error al registrarse' 
            }))
          )
        )
      )
    )
  );

  /**
   * Effect: Register Success
   * 
   * Cuando register fue exitoso:
   * - Navega a la página de inicio
   * - NO dispatch otras acciones (dispatch: false)
   */
  registerSuccess$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(AuthActions.registerSuccess),
        tap(() => this.router.navigate(['/']))
      ),
    { dispatch: false }
  );

  /**
   * Effect: Logout
   * 
   * 1. Escucha AuthActions.logout
   * 2. Limpia localStorage
   * 3. Mapea a logoutSuccess para que reducer limpie el Store
   */
  logout$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AuthActions.logout),
      tap(() => {
        // Limpiar localStorage ANTES de dispatch logoutSuccess
        this.jwtTokenService.removeToken();
      }),
      map(() => AuthActions.logoutSuccess())
    )
  );

  /**
   * Effect: Logout Success
   * 
   * Cuando logout fue completado:
   * - Navega a la página de login
   * - NO dispatch otras acciones (dispatch: false)
   */
  logoutSuccess$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(AuthActions.logoutSuccess),
        tap(() => this.router.navigate(['/login']))
      ),
    { dispatch: false }
  );

  /**
   * Effect: Restore Auth From Storage
   * 
   * Se ejecuta cuando la app se recarga para recuperar
   * la sesión del usuario desde localStorage
   * 
   * 1. Lee token y user de localStorage
   * 2. Si existen y son válidos: mapea a restoreAuthSuccess
   * 3. Si no existen o son inválidos: mapea a logoutSuccess
   */
  restoreAuthFromStorage$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AuthActions.restoreAuthFromStorage),
      switchMap(() => {
        const token = this.jwtTokenService.getToken();
        const userStr = localStorage.getItem('auth_user');
        
        // Verificar que existen y que el token no está expirado
        if (token && userStr && !this.jwtTokenService.isTokenExpired(token)) {
          try {
            const user = JSON.parse(userStr);
            return of(AuthActions.restoreAuthSuccess({ user, token }));
          } catch (error) {
            // JSON inválido, limpiar sesión
            this.jwtTokenService.removeToken();
            return of(AuthActions.logoutSuccess());
          }
        }
        
        // Token expirado o no existe, limpiar sesión
        this.jwtTokenService.removeToken();
        return of(AuthActions.logoutSuccess());
      })
    )
  );
}

