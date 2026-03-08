/**
 * Product DTO
 * Data Transfer Object para productos
 */
export class ProductDto {
  constructor(
    readonly id: string,
    readonly name: string,
    readonly description: string,
    readonly price: number,
    readonly quantity: number,
    readonly imageUrl: string
  ) {}
}
