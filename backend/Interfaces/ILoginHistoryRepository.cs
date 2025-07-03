using LoginSystem.API.Models;

namespace LoginSystem.API.Interfaces
{
    public interface ILoginHistoryRepository
    {
        Task<IEnumerable<LoginHistory>> GetAllAsync();
        Task<IEnumerable<LoginHistory>> GetByUsernameAsync(string username);
        Task<IEnumerable<LoginHistory>> GetByUserIdAsync(Guid userId);
        Task<LoginHistory> CreateAsync(LoginHistory loginHistory);
        Task<IEnumerable<LoginHistory>> GetRecentLoginsAsync(int count = 10);
    }
} 