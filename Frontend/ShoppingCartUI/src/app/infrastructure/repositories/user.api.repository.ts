import { Injectable } from '@angular/core';
import { IUserRepository } from '../../domain/interfaces/user.repository.interface';
import { User } from '../../domain/entities/user.entity';
import { ApiClient } from '../api/api.client';

/**
 * User API Repository
 * Implementación del repositorio de usuarios usando API HTTP
 * Patrón: Repository Pattern, Dependency Injection
 */
@Injectable({
  providedIn: 'root'
})
export class UserApiRepository implements IUserRepository {
  constructor(private apiClient: ApiClient) {}

  async save(entity: User): Promise<User> {
    return this.apiClient.post('users', entity);
  }

  async findById(id: string): Promise<User | null> {
    try {
      return await this.apiClient.get(`users/${id}`);
    } catch (error) {
      return null;
    }
  }

  async findAll(): Promise<User[]> {
    return this.apiClient.get('users');
  }

  async findByEmail(email: string): Promise<User | null> {
    try {
      return await this.apiClient.get(`users/email/${email}`);
    } catch (error) {
      return null;
    }
  }

  async delete(id: string): Promise<void> {
    await this.apiClient.delete(`users/${id}`);
  }

  async update(id: string, entity: Partial<User>): Promise<User> {
    return this.apiClient.put(`users/${id}`, entity);
  }
}
