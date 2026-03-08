/**
 * API Response Wrapper DTOs
 * DTOs para envolver respuestas de la API
 */

export class ApiResponse<T> {
  constructor(
    readonly success: boolean,
    readonly data: T,
    readonly message: string = ''
  ) {}
}

export class ApiError {
  constructor(
    readonly statusCode: number,
    readonly message: string,
    readonly errors?: { [key: string]: string[] }
  ) {}
}
