using Microsoft.EntityFrameworkCore;
using LoginSystem.API.Data;
using LoginSystem.API.Interfaces;
using LoginSystem.API.Models;

namespace LoginSystem.API.Repositories
{
    public class ContentRepository : IContentRepository
    {
        private readonly ApplicationDbContext _context;

        public ContentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Content>> GetAllAsync()
        {
            return await _context.Contents
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Content>> GetPublishedAsync()
        {
            return await _context.Contents
                .Where(c => c.IsPublished)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<Content?> GetByIdAsync(Guid id)
        {
            return await _context.Contents
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Content> CreateAsync(Content content)
        {
            content.Id = Guid.NewGuid();
            content.CreatedAt = DateTime.UtcNow;

            _context.Contents.Add(content);
            await _context.SaveChangesAsync();
            return content;
        }

        public async Task<Content> UpdateAsync(Content content)
        {
            content.UpdatedAt = DateTime.UtcNow;
            _context.Contents.Update(content);
            await _context.SaveChangesAsync();
            return content;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var content = await GetByIdAsync(id);
            if (content == null) return false;

            _context.Contents.Remove(content);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Contents
                .AnyAsync(c => c.Id == id);
        }
    }
} 