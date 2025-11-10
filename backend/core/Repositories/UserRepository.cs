using GymManagement.Core.Models.UserModel;

namespace GymManagement.Core.Repositories.IntUserRepository
{
    public class UserRepository : IUserRepository
    {
        private static readonly List<User> _users = new();

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            try
            {
                return await Task.FromResult(_users);
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving users: " + ex.Message, ex);
            }
        }

        public Task<User?> GetByIdAsync(int id)
        {
            try
            {
                var user = _users.FirstOrDefault(u => u.Id == id);
                return Task.FromResult(user);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving user with ID {id}: {ex.Message}", ex);
            }
        }

        public Task<User?> GetByEmailAsync(string email)
        {
            try
            {
                var user = _users.FirstOrDefault(u => u.Email == email);
                return Task.FromResult(user);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving user with email {email}: {ex.Message}", ex);
            }
        }

        public Task<User> AddAsync(User user)
        {
            try
            {
                user.Id = _users.Count + 1;
                _users.Add(user);
                return Task.FromResult(user);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding new user {user.Email}: {ex.Message}", ex);
            }
        }

        public Task<User?> UpdateAsync(User user)
        {
            try
            {
                var existing = _users.FirstOrDefault(u => u.Id == user.Id);
                if (existing == null)
                    return Task.FromResult<User?>(null);

                existing.Name = user.Name;
                existing.Email = user.Email;
                existing.PasswordHash = user.PasswordHash;
                existing.Role = user.Role;

                return Task.FromResult<User?>(existing);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating user with ID {user.Id}: {ex.Message}", ex);
            }
        }

        public Task<bool> DeleteAsync(int id)
        {
            try
            {
                var user = _users.FirstOrDefault(u => u.Id == id);
                if (user == null)
                    return Task.FromResult(false);

                _users.Remove(user);
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting user with ID {id}: {ex.Message}", ex);
            }
        }
    }
}
