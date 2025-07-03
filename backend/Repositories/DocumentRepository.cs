using Microsoft.EntityFrameworkCore;
using LoginSystem.API.Data;
using LoginSystem.API.Interfaces;
using LoginSystem.API.Models;

namespace LoginSystem.API.Repositories
{
    public class DocumentRepository : IDocumentRepository
    {
        private readonly ApplicationDbContext _context;

        public DocumentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Document>> GetAllAsync()
        {
            return await _context.Documents
                .OrderByDescending(d => d.CreatedAt)
                .ToListAsync();
        }

        public async Task<Document?> GetByIdAsync(Guid id)
        {
            return await _context.Documents
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<Document> CreateAsync(Document document)
        {
            document.Id = Guid.NewGuid();
            document.CreatedAt = DateTime.UtcNow;
            _context.Documents.Add(document);
            await _context.SaveChangesAsync();
            return document;
        }

        public async Task<Document> UpdateAsync(Document document)
        {
            document.UpdatedAt = DateTime.UtcNow;
            _context.Documents.Update(document);
            await _context.SaveChangesAsync();
            return document;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var document = await GetByIdAsync(id);
            if (document == null) return false;
            _context.Documents.Remove(document);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Documents.AnyAsync(d => d.Id == id);
        }
    }
} 