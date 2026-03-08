import { Injectable } from '@angular/core';
import { IProductRepository } from '../../domain/interfaces/product.repository.interface';
import { Product } from '../../domain/entities/product.entity';
import { ApiClient } from '../api/api.client';

/**
 * Product API Repository
 * Implementación del repositorio de productos usando API HTTP
 * Patrón: Repository Pattern, Dependency Injection
 */
@Injectable({
  providedIn: 'root'
})
export class ProductApiRepository implements IProductRepository {
  constructor(private apiClient: ApiClient) {}

  async save(entity: Product): Promise<Product> {
    return this.apiClient.post('products', entity);
  }

  async findById(id: string): Promise<Product | null> {
    try {
      return await this.apiClient.get(`products/${id}`);
    } catch (error) {
      return null;
    }
  }

  async findAll(): Promise<Product[]> {
    return this.apiClient.get('products');
  }

  async findByName(name: string): Promise<Product | null> {
    try {
      return await this.apiClient.get(`products/name/${name}`);
    } catch (error) {
      return null;
    }
  }

  async findAvailable(): Promise<Product[]> {
    return this.apiClient.get('products/available');
  }

  async search(query: string): Promise<Product[]> {
    return this.apiClient.get(`products/search?query=${query}`);
  }

  async delete(id: string): Promise<void> {
    await this.apiClient.delete(`products/${id}`);
  }

  async update(id: string, entity: Partial<Product>): Promise<Product> {
    return this.apiClient.put(`products/${id}`, entity);
  }
}
