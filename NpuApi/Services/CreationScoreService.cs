using NpuApi.Models;
using NpuApi.Repositories;

namespace NpuApi.Services
{
    public interface ICreationScoreService
    {
        Task<CreationScore> CreateCreationScoreAsync(Guid creationId, int creativity, int uniqueness);
        Task<IEnumerable<CreationScore>> GetCreationScoresAsync(Guid creationId);
        Task<CreationScore?> GetCreationScoreByIdAsync(Guid id);
    }

    public class CreationScoreService : ICreationScoreService
    {
        private readonly ICreationScoreRepository _creationScoreRepository;

        public CreationScoreService(ICreationScoreRepository creationScoreRepository)
        {
            _creationScoreRepository = creationScoreRepository;
        }

        public async Task<CreationScore> CreateCreationScoreAsync(Guid creationId, int creativity, int uniqueness)
        {
            var creationScore = new CreationScore
            {
                CreationId = creationId,
                UserId = new Guid("9ecbfde3-df9e-4e60-a220-558609f1fe56"), // TODO: Replace with actual user ID
                Creativity = creativity,
                Uniqueness = uniqueness
            };

            return await _creationScoreRepository.CreateAsync(creationScore);
        }
 
        public async Task<IEnumerable<CreationScore>> GetCreationScoresAsync(Guid creationId)
        {
            return await _creationScoreRepository.GetAllAsync(creationId);
        }

        public async Task<CreationScore?> GetCreationScoreByIdAsync(Guid id)
        {
            return await _creationScoreRepository.GetByIdAsync(id);
        }
    }
}