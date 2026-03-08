import { Injectable } from '@angular/core';
import { ApiClient } from '../api/api.client';
import { LoginResponseDto } from '../../application/dto/auth.dto';

/**
 * Authentication API Service
 * Servicio para operaciones de autenticación
 * Patrón: Service Pattern
 */
@Injectable({
  providedIn: 'root'
})
export class AuthApiService {
  constructor(private apiClient: ApiClient) {}

  async login(email: string, password: string): Promise<LoginResponseDto> {
    return this.apiClient.post('auth/login', { email, password });
  }

  async register(email: string, password: string, name: string): Promise<LoginResponseDto> {
    return this.apiClient.post('auth/register', { email, password, name });
  }

  async refreshToken(token: string): Promise<{ token: string }> {
    return this.apiClient.post('auth/refresh', { token });
  }
}
