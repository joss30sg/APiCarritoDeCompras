import { Injectable } from '@angular/core';
import { User } from '../../domain/entities/user.entity';

/**
 * User Mapper
 * Convierte entre entidades del dominio y DTOs
 */
@Injectable({
  providedIn: 'root'
})
export class UserMapper {
  toDomain(data: any): User {
    return User.create(data.id, data.email, data.name, new Date(data.createdAt));
  }

  toDto(domain: User): any {
    return {
      id: domain.id,
      email: domain.email,
      name: domain.name,
      createdAt: domain.createdAt
    };
  }
}
