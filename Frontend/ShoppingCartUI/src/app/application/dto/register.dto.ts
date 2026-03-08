/**
 * Register Request DTO
 * Data Transfer Object para solicitud de registro
 */
export class RegisterRequestDto {
  constructor(
    readonly email: string,
    readonly password: string,
    readonly name: string
  ) {}
}
