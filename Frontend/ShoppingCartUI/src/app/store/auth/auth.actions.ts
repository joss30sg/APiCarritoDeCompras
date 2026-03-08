import { createAction, props } from '@ngrx/store';
import { LoginRequest, RegisterRequest, AuthResponse } from '../../shared/models/auth.model';

// Login Actions
export const login = createAction(
  '[Auth] Login',
  props<{ request: LoginRequest }>()
);

export const loginSuccess = createAction(
  '[Auth] Login Success',
  props<{ response: AuthResponse }>()
);

export const loginFailure = createAction(
  '[Auth] Login Failure',
  props<{ error: string }>()
);

// Register Actions
export const register = createAction(
  '[Auth] Register',
  props<{ request: RegisterRequest }>()
);

export const registerSuccess = createAction(
  '[Auth] Register Success',
  props<{ response: AuthResponse }>()
);

export const registerFailure = createAction(
  '[Auth] Register Failure',
  props<{ error: string }>()
);

// Logout Actions
export const logout = createAction(
  '[Auth] Logout'
);

export const logoutSuccess = createAction(
  '[Auth] Logout Success'
);

// Restore Auth Actions (cuando se recarga la página)
export const restoreAuthFromStorage = createAction(
  '[Auth] Restore Auth From Storage'
);

export const restoreAuthSuccess = createAction(
  '[Auth] Restore Auth Success',
  props<{ user: any; token: string }>()
);

// Refresh Token Actions
export const refreshToken = createAction(
  '[Auth] Refresh Token'
);

export const refreshTokenSuccess = createAction(
  '[Auth] Refresh Token Success',
  props<{ token: string }>()
);

export const refreshTokenFailure = createAction(
  '[Auth] Refresh Token Failure'
);

// Clear Error
export const clearAuthError = createAction(
  '[Auth] Clear Error'
);
