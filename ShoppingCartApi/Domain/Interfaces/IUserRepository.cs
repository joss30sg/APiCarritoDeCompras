using ShoppingCartApi.Domain.Entities;

namespace ShoppingCartApi.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByUsernameAsync(string username);
        Task AddUserAsync(User user);
        Task UpdateUserAsync(User user); // Añadir método para actualizar usuario
    }
}
