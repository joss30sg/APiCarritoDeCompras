import { Injectable } from '@angular/core';
import { IAuthService } from '../interfaces/auth.service.interface';

/**
 * Authentication Service
 * Implementación del servicio de autenticación
 * Patrón: Dependency Injection, Observer Pattern (con RxJS)
 */
@Injectable({
  providedIn: 'root'
})
export class AuthService implements IAuthService {
  async login(email: string, password: string): Promise<{ token: string; userId: string }> {
    // Será implementado en el layer de aplicación/infraestructura
    throw new Error('Method not implemented.');
  }

  async register(email: string, password: string, name: string): Promise<{ token: string; userId: string }> {
    // Será implementado en el layer de aplicación/infraestructura
    throw new Error('Method not implemented.');
  }

  async logout(): Promise<void> {
    // Será implementado en el layer de aplicación/infraestructura
    throw new Error('Method not implemented.');
  }

  async refreshToken(token: string): Promise<{ token: string }> {
    // Será implementado en el layer de aplicación/infraestructura
    throw new Error('Method not implemented.');
  }

  async validateToken(token: string): Promise<boolean> {
    // Será implementado en el layer de aplicación/infraestructura
    throw new Error('Method not implemented.');
  }
}
