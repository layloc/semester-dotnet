using SEM.Domain.Entities;

namespace SEM.Domain.Abstractions;

public interface IModelRepository
{
    public Task<Model?> GetModelByIdAsync(Guid id);
    public Task<Model?> UpdateModelAsync(Model?  model);
    public Task DeleteModelAsync(Model? model);
    public Task<Guid> CreateModelAsync(Model? model);
    public Task<IEnumerable<Model?>> GetModelsOfUserAsync(Guid userId);
}