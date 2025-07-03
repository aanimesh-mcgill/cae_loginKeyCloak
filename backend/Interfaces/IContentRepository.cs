using LoginSystem.API.Models;

namespace LoginSystem.API.Interfaces
{
    public interface IContentRepository
    {
        Task<IEnumerable<Content>> GetAllAsync();
        Task<IEnumerable<Content>> GetPublishedAsync();
        Task<Content?> GetByIdAsync(Guid id);
        Task<Content> CreateAsync(Content content);
        Task<Content> UpdateAsync(Content content);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
    }
} 