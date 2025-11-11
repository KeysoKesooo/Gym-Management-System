using GymManagement.Core.Models.UserModel;
using GymManagement.Data;
using Microsoft.EntityFrameworkCore;

namespace GymManagement.Core.Repositories.IntUserRepository
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            try
            {
                return await _context.Users.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving users: " + ex.Message, ex);
            }
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Users.FindAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving user with ID {id}: {ex.Message}", ex);
            }
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            try
            {
                return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving user with email {email}: {ex.Message}", ex);
            }
        }

        public async Task<User> AddAsync(User user)
        {
            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return user;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding new user {user.Email}: {ex.Message}", ex);
            }
        }

        public async Task<User?> UpdateAsync(User user)
        {
            try
            {
                var existing = await _context.Users.FindAsync(user.Id);
                if (existing == null) return null;

                existing.Name = user.Name;
                existing.Email = user.Email;
                existing.PasswordHash = user.PasswordHash;
                existing.Role = user.Role;

                await _context.SaveChangesAsync();
                return existing;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating user with ID {user.Id}: {ex.Message}", ex);
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null) return false;

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting user with ID {id}: {ex.Message}", ex);
            }
        }
    }
}
