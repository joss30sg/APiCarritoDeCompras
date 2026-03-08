import { User } from '../entities/user.entity';
import { IRepository } from './repository.interface';

/**
 * User Repository Interface
 * Extiende la interfaz genérica de repositorio con operaciones específicas para usuarios
 */
export interface IUserRepository extends IRepository<User> {
  findByEmail(email: string): Promise<User | null>;
}
