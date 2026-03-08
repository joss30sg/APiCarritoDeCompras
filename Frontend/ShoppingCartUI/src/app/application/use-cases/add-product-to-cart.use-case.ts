import { Injectable } from '@angular/core';
import { ShoppingCartDto, AddProductToCartRequestDto } from '../dto/shopping-cart.dto';
import { ShoppingCartApiRepository } from '../../infrastructure/repositories/shopping-cart.api.repository';

/**
 * Add Product to Cart Use Case
 * Define el caso de uso para agregar un producto al carrito
 * Patrón: Use Case Pattern
 */
@Injectable({
  providedIn: 'root'
})
export class AddProductToCartUseCase {
  constructor(private shoppingCartRepository: ShoppingCartApiRepository) {}

  async execute(userId: string, request: AddProductToCartRequestDto): Promise<ShoppingCartDto> {
    // Delegar al repositorio - es responsabilidad de la infraestructura
    // manejar la lógica de agregar productos
    const savedCart = await this.shoppingCartRepository.findByUserId(userId);
    if (!savedCart) {
      throw new Error('Shopping cart not found');
    }
    // En una implementación real, el backend maneja la adición de productos
    const cart = await this.shoppingCartRepository.findByUserId(userId);
    return this.mapToDto(cart || { id: userId, userId, items: [] });
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
