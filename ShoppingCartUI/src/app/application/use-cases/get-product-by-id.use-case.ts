import { Injectable } from '@angular/core';
import { ProductDto } from '../dto/product.dto';
import { ProductApiRepository } from '../../infrastructure/repositories/product.api.repository';

/**
 * Get Product by ID Use Case
 * Define el caso de uso para obtener un producto por su ID
 * Patrón: Use Case Pattern
 */
@Injectable({
  providedIn: 'root'
})
export class GetProductByIdUseCase {
  constructor(private productRepository: ProductApiRepository) {}

  async execute(productId: string): Promise<ProductDto> {
    const product = await this.productRepository.findById(productId);
    if (!product) {
      throw new Error(`Product not found: ${productId}`);
    }
    return new ProductDto(
      product.id.toString(),
      product.name,
      product.description || '',
      product.price,
      product.quantity || 0,
      product.imageUrl || ''
    );
  }
}
