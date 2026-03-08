import { Injectable } from '@angular/core';
import { LoginResponseDto } from '../dto/auth.dto';
import { AuthService } from '../services/auth.service';
import { firstValueFrom } from 'rxjs';

/**
 * Login Use Case
 * Define el caso de uso para autenticar un usuario
 * Patrón: Use Case Pattern
 */
@Injectable({
  providedIn: 'root'
})
export class LoginUseCase {
  constructor(private authService: AuthService) {}

  async execute(email: string, password: string): Promise<LoginResponseDto> {
    const response = await firstValueFrom(this.authService.login({ username: email, password }));
    return new LoginResponseDto(
      response.token,
      response.user?.id || '',
      email,
      response.user?.username || ''
    );
  }
}
