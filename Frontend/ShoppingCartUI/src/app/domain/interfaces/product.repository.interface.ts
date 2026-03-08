import { Product } from '../entities/product.entity';
import { IRepository } from './repository.interface';

/**
 * Product Repository Interface
 * Extiende la interfaz genérica de repositorio con operaciones específicas para productos
 */
export interface IProductRepository extends IRepository<Product> {
  findByName(name: string): Promise<Product | null>;
  findAvailable(): Promise<Product[]>;
  search(query: string): Promise<Product[]>;
}
