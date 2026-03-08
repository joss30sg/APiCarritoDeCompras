/**
 * User Entity
 * Representa a un usuario en el dominio de la aplicación
 */
export class User {
  constructor(
    readonly id: string,
    readonly email: string,
    readonly name: string,
    readonly createdAt: Date
  ) {}

  static create(
    id: string,
    email: string,
    name: string,
    createdAt: Date = new Date()
  ): User {
    return new User(id, email, name, createdAt);
  }
}
