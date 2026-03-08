import { Injectable } from '@angular/core';
import { ProductDto } from '../dto/product.dto';
import { ProductApiRepository } from '../../infrastructure/repositories/product.api.repository';

/**
 * Get All Products Use Case
 * Define el caso de uso para obtener todos los productos
 * Patrón: Use Case Pattern
 */
@Injectable({
  providedIn: 'root'
})
export class GetAllProductsUseCase {
  constructor(private productRepository: ProductApiRepository) {}

  async execute(): Promise<ProductDto[]> {
    const products = await this.productRepository.findAll();
    return products.map(p => new ProductDto(
      p.id.toString(),
      p.name,
      p.description || '',
      p.price,
      p.quantity || 0,
      p.imageUrl || ''
    ));
  }
}
