export interface LoginRequest {
  username: string;
  password: string;
}

export interface RegisterRequest {
  username: string;
  password: string;
}

export interface AuthResponse {
  token: string;
  user?: {
    id: string;
    username: string;
  };
}

export interface AuthState {
  isAuthenticated: boolean;
  user: { id: string; username: string } | null;
  token: string | null;
  error: string | null;
}
