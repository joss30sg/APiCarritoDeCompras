import { Injectable } from '@angular/core';
import { ShoppingCartDto } from '../dto/shopping-cart.dto';
import { ShoppingCartApiRepository } from '../../infrastructure/repositories/shopping-cart.api.repository';

/**
 * Get Shopping Cart Use Case
 * Define el caso de uso para obtener el carrito de compras
 * Patrón: Use Case Pattern
 */
@Injectable({
  providedIn: 'root'
})
export class GetShoppingCartUseCase {
  constructor(private shoppingCartRepository: ShoppingCartApiRepository) {}

  async execute(userId: string): Promise<ShoppingCartDto> {
    const cart = await this.shoppingCartRepository.findByUserId(userId);
    if (!cart) {
      // Retornar carrito vacío
      return new ShoppingCartDto(userId, userId, [], 0, 0);
    }
    return this.mapToDto(cart);
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
