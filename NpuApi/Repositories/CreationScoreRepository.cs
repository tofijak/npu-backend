using NpuApi.Data;
using NpuApi.Models;
using Microsoft.EntityFrameworkCore;

namespace NpuApi.Repositories
{
    public interface ICreationScoreRepository
    {
        Task<CreationScore> CreateAsync(CreationScore creationScore);
        Task<IEnumerable<CreationScore>> GetAllAsync(Guid creationId);
        Task<CreationScore?> GetByIdAsync(Guid id);
    }

    public class CreationScoreRepository : ICreationScoreRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public CreationScoreRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<CreationScore> CreateAsync(CreationScore creationScore)
        {
            _dbContext.CreationScores.Add(creationScore);
            await _dbContext.SaveChangesAsync();
            return creationScore;
        }

        public async Task<IEnumerable<CreationScore>> GetAllAsync(Guid creationId)
        {
            return await _dbContext.CreationScores
                .Where(cs => cs.CreationId == creationId)
                .ToListAsync();
        }

        public async Task<CreationScore?> GetByIdAsync(Guid id)
        {
            return await _dbContext.CreationScores.FindAsync(id);
        }
    }
}