import { ProductDto } from './product.dto';

/**
 * Shopping Cart Item DTO
 * Data Transfer Object para ítems del carrito
 */
export class ShoppingCartItemDto {
  constructor(
    readonly id: string,
    readonly product: ProductDto,
    readonly quantity: number
  ) {}
}

/**
 * Shopping Cart DTO
 * Data Transfer Object para el carrito de compras
 */
export class ShoppingCartDto {
  constructor(
    readonly id: string,
    readonly userId: string,
    readonly items: ShoppingCartItemDto[],
    readonly totalPrice: number,
    readonly totalItems: number
  ) {}
}

/**
 * Add Product to Cart Request DTO
 */
export class AddProductToCartRequestDto {
  constructor(
    readonly productId: string,
    readonly quantity: number = 1
  ) {}
}

/**
 * Update Product Quantity Request DTO
 */
export class UpdateProductQuantityRequestDto {
  constructor(
    readonly itemId: string,
    readonly quantity: number
  ) {}
}
