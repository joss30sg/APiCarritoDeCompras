import { createReducer, on } from '@ngrx/store';
import * as AuthActions from './auth.actions';
import { AuthState, initialAuthState } from './auth.state';

export const authReducer = createReducer(
  initialAuthState,

  // Login
  on(AuthActions.login, (state) => ({
    ...state,
    isLoading: true,
    error: null
  })),

  on(AuthActions.loginSuccess, (state, { response }) => ({
    ...state,
    user: response.user || null,
    token: response.token,
    isLoading: false,
    isAuthenticated: true,
    error: null,
    lastAuthTime: Date.now()
  })),

  on(AuthActions.loginFailure, (state, { error }) => ({
    ...state,
    isLoading: false,
    isAuthenticated: false,
    error
  })),

  // Register
  on(AuthActions.register, (state) => ({
    ...state,
    isLoading: true,
    error: null
  })),

  on(AuthActions.registerSuccess, (state, { response }) => ({
    ...state,
    user: response.user || null,
    token: response.token,
    isLoading: false,
    isAuthenticated: true,
    error: null,
    lastAuthTime: Date.now()
  })),

  on(AuthActions.registerFailure, (state, { error }) => ({
    ...state,
    isLoading: false,
    isAuthenticated: false,
    error
  })),

  // Logout
  on(AuthActions.logout, (state) => ({
    ...state,
    isLoading: true
  })),

  on(AuthActions.logoutSuccess, (state) => ({
    ...state,
    user: null,
    token: null,
    isLoading: false,
    isAuthenticated: false,
    error: null,
    lastAuthTime: null
  })),

  // Restore Auth
  on(AuthActions.restoreAuthSuccess, (state, { user, token }) => ({
    ...state,
    user,
    token,
    isAuthenticated: true,
    error: null
  })),

  // Refresh Token
  on(AuthActions.refreshToken, (state) => ({
    ...state,
    isLoading: true
  })),

  on(AuthActions.refreshTokenSuccess, (state, { token }) => ({
    ...state,
    token,
    isLoading: false,
    lastAuthTime: Date.now()
  })),

  on(AuthActions.refreshTokenFailure, (state) => ({
    ...state,
    isLoading: false,
    isAuthenticated: false,
    user: null,
    token: null
  })),

  // Clear Error
  on(AuthActions.clearAuthError, (state) => ({
    ...state,
    error: null
  }))
);
