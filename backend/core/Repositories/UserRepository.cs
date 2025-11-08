using GymManagement.Core.Models.UserModel;

namespace GymManagement.Core.Repositories.IntUserRepository
{
    public class UserRepository : IUserRepository
    {
        private static readonly List<User> _users = new();


        public async Task<IEnumerable<User>> GetAllAsync() =>
            await Task.FromResult(_users);

        public async Task<User?> GetByIdAsync(int id)
        {
            var user = _users.FirstOrDefault(u => u.Id == id);
            return await Task.FromResult(user);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            var user = _users.FirstOrDefault(u => u.Email == email);
            return await Task.FromResult(user);
        }

        public async Task<User> AddAsync(User user)
        {
            user.Id = _users.Count + 1;
            _users.Add(user);
            return await Task.FromResult(user);
        }

        public async Task<User?> UpdateAsync(User user)
        {
            var existing = _users.FirstOrDefault(u => u.Id == user.Id);
            if (existing == null)
                return null;

            existing.Name = user.Name;
            existing.Email = user.Email;
            existing.PasswordHash = user.PasswordHash;
            existing.Role = user.Role;

            return await Task.FromResult(existing);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var user = _users.FirstOrDefault(u => u.Id == id);
            if (user == null)
                return await Task.FromResult(false);

            _users.Remove(user);
            return await Task.FromResult(true);
        }
    }
}