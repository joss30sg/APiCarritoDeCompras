/**
 * Login Request DTO
 * Data Transfer Object para solicitud de login
 */
export class LoginRequestDto {
  constructor(
    readonly email: string,
    readonly password: string
  ) {}
}

/**
 * Login Response DTO
 * Data Transfer Object para respuesta de login
 */
export class LoginResponseDto {
  constructor(
    readonly token: string,
    readonly userId: string,
    readonly email: string,
    readonly name: string
  ) {}
}
