import { createFeatureSelector, createSelector } from '@ngrx/store';
import { AuthState } from './auth.state';

// Feature Selector
export const selectAuthState = createFeatureSelector<AuthState>('auth');

// Selectors
export const selectAuthUser = createSelector(
  selectAuthState,
  (state: AuthState) => state.user
);

export const selectAuthToken = createSelector(
  selectAuthState,
  (state: AuthState) => state.token
);

export const selectIsAuthenticated = createSelector(
  selectAuthState,
  (state: AuthState) => state.isAuthenticated
);

export const selectIsLoading = createSelector(
  selectAuthState,
  (state: AuthState) => state.isLoading
);

export const selectAuthError = createSelector(
  selectAuthState,
  (state: AuthState) => state.error
);

export const selectLastAuthTime = createSelector(
  selectAuthState,
  (state: AuthState) => state.lastAuthTime
);

// Selector para obtener todo el estado de auth
export const selectFullAuthState = createSelector(
  selectAuthState,
  (state: AuthState) => state
);

// Selector derivado: usuario nombre completo
export const selectUserDisplay = createSelector(
  selectAuthUser,
  (user) => user ? user.username : ''
);
