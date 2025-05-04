using NpuApi.Data;
using NpuApi.Models;
using Microsoft.EntityFrameworkCore;

namespace NpuApi.Repositories
{
    public interface ICreationRepository
    {
        Task<Creation> CreateAsync(Creation creation);
        Task<IEnumerable<Creation>> GetAllAsync(string? searchTerm = null);
        Task<Creation?> GetByIdAsync(Guid id);
    }

    public class CreationRepository : ICreationRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public CreationRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Creation> CreateAsync(Creation creation)
        {
            _dbContext.Creations.Add(creation);
            await _dbContext.SaveChangesAsync();
            return creation;
        }

        public async Task<IEnumerable<Creation>> GetAllAsync(string? searchTerm = null)
        {
            var query = _dbContext.Creations.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                query = query.Where(c => c.NicePartName.ToLower().Contains(searchTerm.ToLower()));
            }

            return await query.ToListAsync();
        }

        public async Task<Creation?> GetByIdAsync(Guid id)
        {
            return await _dbContext.Creations.FindAsync(id);
        }
    }
}