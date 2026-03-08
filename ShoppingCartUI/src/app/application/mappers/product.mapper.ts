import { Injectable } from '@angular/core';
import { Product } from '../../domain/entities/product.entity';
import { ProductDto } from '../dto/product.dto';

/**
 * Product Mapper
 * Convierte entre entidades del dominio y DTOs
 * Patrón: Mapper Pattern
 */
@Injectable({
  providedIn: 'root'
})
export class ProductMapper {
  toDomain(dto: ProductDto): Product {
    return Product.create(
      dto.id,
      dto.name,
      dto.description,
      dto.price,
      dto.quantity,
      dto.imageUrl
    );
  }

  toDto(domain: Product): ProductDto {
    return new ProductDto(
      domain.id,
      domain.name,
      domain.description,
      domain.price,
      domain.quantity,
      domain.imageUrl
    );
  }

  toDomainArray(dtos: ProductDto[]): Product[] {
    return dtos.map(dto => this.toDomain(dto));
  }

  toDtoArray(domains: Product[]): ProductDto[] {
    return domains.map(domain => this.toDto(domain));
  }
}
