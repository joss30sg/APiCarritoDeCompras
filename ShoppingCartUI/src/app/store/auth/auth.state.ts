/**
 * AuthState - Estado global de autenticación (NgRx)
 * Maneja:
 * - Información del usuario autenticado
 * - Token JWT
 * - Estado de carga
 * - Errores de autenticación
 */

export interface AuthUser {
  id: string;
  username: string;
  email?: string;
}

export interface AuthState {
  user: AuthUser | null;
  token: string | null;
  isLoading: boolean;
  isAuthenticated: boolean;
  error: string | null;
  lastAuthTime: number | null;
}

export const initialAuthState: AuthState = {
  user: null,
  token: null,
  isLoading: false,
  isAuthenticated: false,
  error: null,
  lastAuthTime: null
};
