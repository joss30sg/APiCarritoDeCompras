using ShoppingCartApi.Domain.Entities;
using ShoppingCartApi.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCartApi.Infrastructure.Repositories
{
    public class InMemoryUserRepository : IUserRepository
    {
        private readonly Dictionary<int, User> _users = new Dictionary<int, User>();
        private int _nextId = 1;

        public InMemoryUserRepository()
        {
            // Seed some initial users for testing
            var adminUser = new User
            {
                Id = _nextId++,
                Username = "admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!")
            };
            _users.Add(adminUser.Id, adminUser);

            // Test user
            var testUser = new User
            {
                Id = _nextId++,
                Username = "testuser",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!")
            };
            _users.Add(testUser.Id, testUser);
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            var user = _users.Values.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
            return await Task.FromResult(user);
        }

        public async Task AddUserAsync(User user)
        {
            user.Id = _nextId++;
            _users.Add(user.Id, user);
            await Task.CompletedTask;
        }

        public async Task UpdateUserAsync(User user)
        {
            if (_users.ContainsKey(user.Id))
            {
                _users[user.Id] = user;
            }
            await Task.CompletedTask;
        }
    }
}
