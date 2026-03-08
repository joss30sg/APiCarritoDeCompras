/**
 * Generic Repository Interface
 * Define el contrato para las operaciones CRUD básicas
 * Patrón: Repository Pattern
 */
export interface IRepository<T> {
  save(entity: T): Promise<T>;
  findById(id: string): Promise<T | null>;
  findAll(): Promise<T[]>;
  delete(id: string): Promise<void>;
  update(id: string, entity: Partial<T>): Promise<T>;
}
