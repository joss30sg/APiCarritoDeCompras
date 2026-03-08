/**
 * Authentication Service Interface
 * Define el contrato para operaciones de autenticación
 */
export interface IAuthService {
  login(email: string, password: string): Promise<{ token: string; userId: string }>;
  register(email: string, password: string, name: string): Promise<{ token: string; userId: string }>;
  logout(): Promise<void>;
  refreshToken(token: string): Promise<{ token: string }>;
  validateToken(token: string): Promise<boolean>;
}
