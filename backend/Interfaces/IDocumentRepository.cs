using LoginSystem.API.Models;

namespace LoginSystem.API.Interfaces
{
    public interface IDocumentRepository
    {
        Task<IEnumerable<Document>> GetAllAsync();
        Task<Document?> GetByIdAsync(Guid id);
        Task<Document> CreateAsync(Document document);
        Task<Document> UpdateAsync(Document document);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
    }
} 