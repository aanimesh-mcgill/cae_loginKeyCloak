using Microsoft.EntityFrameworkCore;
using LoginSystem.API.Data;
using LoginSystem.API.Interfaces;
using LoginSystem.API.Models;
using Microsoft.Extensions.Logging;

namespace LoginSystem.API.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(ApplicationDbContext context, ILogger<UserRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users
                .Where(u => u.IsActive)
                .OrderBy(u => u.DisplayName)
                .ToListAsync();
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Id == id && u.IsActive);
        }

        public async Task<User?> GetByAdUsernameAsync(string adUsername)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.AdUsername == adUsername && u.IsActive);
        }

        public async Task<User> CreateAsync(User user)
        {
            try
            {
                _logger.LogInformation("Attempting to insert user into DB: {@User}", user);
                user.Id = Guid.NewGuid();
                user.CreatedAt = DateTime.UtcNow;
                user.IsActive = true;

                _context.Users.Add(user);
                _logger.LogInformation("EF Core Insert SQL: {Sql}", _context.Users.ToQueryString());
                await _context.SaveChangesAsync();
                _logger.LogInformation("User inserted successfully: {@User}", user);
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inserting user into DB: {@User}", user);
                throw;
            }
        }

        public async Task<User> UpdateAsync(User user)
        {
            user.UpdatedAt = DateTime.UtcNow;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var user = await GetByIdAsync(id);
            if (user == null) return false;

            user.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Users
                .AnyAsync(u => u.Id == id && u.IsActive);
        }

        public async Task<bool> ExistsByAdUsernameAsync(string adUsername)
        {
            return await _context.Users
                .AnyAsync(u => u.AdUsername == adUsername && u.IsActive);
        }
    }
} 