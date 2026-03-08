import { ShoppingCart } from '../entities/shopping-cart.entity';
import { IRepository } from './repository.interface';

/**
 * Shopping Cart Repository Interface
 * Extiende la interfaz genérica de repositorio con operaciones específicas para carrito
 */
export interface IShoppingCartRepository extends IRepository<ShoppingCart> {
  findByUserId(userId: string): Promise<ShoppingCart | null>;
  clear(id: string): Promise<void>;
}
