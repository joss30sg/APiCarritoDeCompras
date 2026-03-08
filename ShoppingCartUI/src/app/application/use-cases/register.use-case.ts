import { Injectable } from '@angular/core';
import { LoginResponseDto } from '../dto/auth.dto';
import { AuthService } from '../services/auth.service';
import { firstValueFrom } from 'rxjs';

/**
 * Register Use Case
 * Define el caso de uso para registrar un nuevo usuario
 * Patrón: Use Case Pattern
 */
@Injectable({
  providedIn: 'root'
})
export class RegisterUseCase {
  constructor(private authService: AuthService) {}

  async execute(email: string, password: string, name: string): Promise<LoginResponseDto> {
    const response = await firstValueFrom(this.authService.register({ username: name, password }));
    return new LoginResponseDto(
      response.token,
      response.user?.id || '',
      email,
      name
    );
  }
}
