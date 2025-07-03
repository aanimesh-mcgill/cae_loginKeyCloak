using Microsoft.EntityFrameworkCore;
using LoginSystem.API.Data;
using LoginSystem.API.Interfaces;
using LoginSystem.API.Models;

namespace LoginSystem.API.Repositories
{
    public class LoginHistoryRepository : ILoginHistoryRepository
    {
        private readonly ApplicationDbContext _context;

        public LoginHistoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<LoginHistory>> GetAllAsync()
        {
            return await _context.LoginHistories
                .OrderByDescending(lh => lh.LoginTimeUtc)
                .ToListAsync();
        }

        public async Task<IEnumerable<LoginHistory>> GetByUsernameAsync(string username)
        {
            return await _context.LoginHistories
                .Where(lh => lh.Username == username)
                .OrderByDescending(lh => lh.LoginTimeUtc)
                .ToListAsync();
        }

        public async Task<IEnumerable<LoginHistory>> GetByUserIdAsync(Guid userId)
        {
            return await _context.LoginHistories
                .Where(lh => lh.UserId == userId)
                .OrderByDescending(lh => lh.LoginTimeUtc)
                .ToListAsync();
        }

        public async Task<LoginHistory> CreateAsync(LoginHistory loginHistory)
        {
            loginHistory.Id = Guid.NewGuid();
            loginHistory.LoginTimeUtc = DateTime.UtcNow;

            _context.LoginHistories.Add(loginHistory);
            await _context.SaveChangesAsync();
            return loginHistory;
        }

        public async Task<IEnumerable<LoginHistory>> GetRecentLoginsAsync(int count = 10)
        {
            return await _context.LoginHistories
                .OrderByDescending(lh => lh.LoginTimeUtc)
                .Take(count)
                .ToListAsync();
        }
    }
} 