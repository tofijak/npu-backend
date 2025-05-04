using NpuApi.Models;
using NpuApi.Repositories;

namespace NpuApi.Services
{
    public interface ICreationService
    {
        Task<(Guid Id, string ImageUrl)> CreateCreationAsync(string title, string description, string nicePartName, Stream imageStream, string fileName);
        Task<IEnumerable<Creation>> GetCreationsAsync(string? searchTerm = null);
        Task<Creation?> GetCreationByIdAsync(Guid id);
    }

    public class CreationService : ICreationService
    {
        private readonly ICreationRepository _creationRepository;
        private readonly ICreationImageRepository _creationImageRepository;

        public CreationService(ICreationRepository creationRepository, ICreationImageRepository creationImageRepository)
        {
            _creationRepository = creationRepository;
            _creationImageRepository = creationImageRepository;
        }

        public async Task<(Guid Id, string ImageUrl)> CreateCreationAsync(string title, string description, string nicePartName, Stream imageStream, string fileName)
        {
            // TODO: Dynamically determine the content type
            var imageUrl = await _creationImageRepository.UploadFileAsync(imageStream, "image/jpeg");

            var creation = new Creation
            {
                Title = title,
                Description = description,
                ImageUrl = imageUrl,
                UserId = new Guid("9ecbfde3-df9e-4e60-a220-558609f1fe56"), // Using seed user ID for now
                CreatedAt = DateTime.UtcNow,
                NicePartName = nicePartName
            };

            try
            {
                var createdEntity = await _creationRepository.CreateAsync(creation);
                return (createdEntity.Id, createdEntity.ImageUrl);
            }
            catch (Exception)
            {
                // TODO: Handle deletion of images if the creation fails to save
                throw;
            }
        }

        public async Task<IEnumerable<Creation>> GetCreationsAsync(string? searchTerm = null)
        {
            return await _creationRepository.GetAllAsync(searchTerm);
        }

        public async Task<Creation?> GetCreationByIdAsync(Guid id)
        {
            return await _creationRepository.GetByIdAsync(id);
        }
    }
}