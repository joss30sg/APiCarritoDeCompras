import { Injectable } from '@angular/core';
import { ShoppingCart } from '../../domain/entities/shopping-cart.entity';
import { ShoppingCartItem } from '../../domain/entities/shopping-cart-item.entity';
import { ShoppingCartDto, ShoppingCartItemDto } from '../dto/shopping-cart.dto';
import { ProductMapper } from './product.mapper';

/**
 * Shopping Cart Mapper
 * Convierte entre entidades del dominio y DTOs
 * Patrón: Mapper Pattern
 */
@Injectable({
  providedIn: 'root'
})
export class ShoppingCartMapper {
  constructor(private productMapper: ProductMapper) {}

  toDomain(dto: ShoppingCartDto): ShoppingCart {
    const items = dto.items.map(itemDto =>
      ShoppingCartItem.create(
        itemDto.id,
        this.productMapper.toDomain(itemDto.product),
        itemDto.quantity
      )
    );

    return new ShoppingCart(dto.id, dto.userId, items);
  }

  toDto(domain: ShoppingCart): ShoppingCartDto {
    const items = domain.items.map(
      item =>
        new ShoppingCartItemDto(
          item.id,
          this.productMapper.toDto(item.product),
          item.quantity
        )
    );

    return new ShoppingCartDto(
      domain.id,
      domain.userId,
      items,
      domain.getTotalPrice(),
      domain.getTotalItems()
    );
  }
}
