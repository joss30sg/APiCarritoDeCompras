import { Injectable } from '@angular/core';
import { IShoppingCartRepository } from '../../domain/interfaces/shopping-cart.repository.interface';
import { ShoppingCart } from '../../domain/entities/shopping-cart.entity';
import { ApiClient } from '../api/api.client';

/**
 * Shopping Cart API Repository
 * Implementación del repositorio de carrito usando API HTTP
 * Patrón: Repository Pattern, Dependency Injection
 */
@Injectable({
  providedIn: 'root'
})
export class ShoppingCartApiRepository implements IShoppingCartRepository {
  constructor(private apiClient: ApiClient) {}

  async save(entity: ShoppingCart): Promise<ShoppingCart> {
    return this.apiClient.post('shopping-carts', entity);
  }

  async findById(id: string): Promise<ShoppingCart | null> {
    try {
      return await this.apiClient.get(`shopping-carts/${id}`);
    } catch (error) {
      return null;
    }
  }

  async findAll(): Promise<ShoppingCart[]> {
    return this.apiClient.get('shopping-carts');
  }

  async findByUserId(userId: string): Promise<ShoppingCart | null> {
    try {
      return await this.apiClient.get(`shopping-carts/user/${userId}`);
    } catch (error) {
      return null;
    }
  }

  async delete(id: string): Promise<void> {
    await this.apiClient.delete(`shopping-carts/${id}`);
  }

  async update(id: string, entity: Partial<ShoppingCart>): Promise<ShoppingCart> {
    return this.apiClient.put(`shopping-carts/${id}`, entity);
  }

  async clear(id: string): Promise<void> {
    await this.apiClient.delete(`shopping-carts/${id}/clear`);
  }
}
