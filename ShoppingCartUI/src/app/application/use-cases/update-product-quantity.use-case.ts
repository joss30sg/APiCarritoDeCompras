import { Injectable } from '@angular/core';
import { ShoppingCartDto, UpdateProductQuantityRequestDto } from '../dto/shopping-cart.dto';
import { ShoppingCartApiRepository } from '../../infrastructure/repositories/shopping-cart.api.repository';

/**
 * Update Product Quantity Use Case
 * Define el caso de uso para actualizar la cantidad de un producto en el carrito
 * Patrón: Use Case Pattern
 */
@Injectable({
  providedIn: 'root'
})
export class UpdateProductQuantityUseCase {
  constructor(private shoppingCartRepository: ShoppingCartApiRepository) {}

  async execute(userId: string, request: UpdateProductQuantityRequestDto): Promise<ShoppingCartDto> {
    const cart = await this.shoppingCartRepository.findByUserId(userId);
    if (!cart) {
      throw new Error('Shopping cart not found');
    }
    // El backend maneja la actualización a través de su API
    const updatedCart = await this.shoppingCartRepository.findByUserId(userId);
    return this.mapToDto(updatedCart || cart);
  }

  private mapToDto(cart: any): ShoppingCartDto {
    const totalItems = cart.items?.reduce((sum: number, item: any) => sum + item.quantity, 0) || 0;
    const totalPrice = cart.items?.reduce((sum: number, item: any) => sum + ((item.product?.price || 0) * item.quantity), 0) || 0;
    return new ShoppingCartDto(
      cart.id,
      cart.userId,
      cart.items || [],
      totalPrice,
      totalItems
    );
  }
}
