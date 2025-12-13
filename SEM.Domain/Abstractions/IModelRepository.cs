using SEM.Entities;

namespace SEM.Domain.Abstractions;

public interface IModelRepository
{
    public Task<Model?> GetModelAsync(Guid id);
    public Task<Model?> UpdateModelAsync(Model?  model);
    public Task DeleteModelAsync(Model? model);
    public Task<Guid> CreateModelAsync(Model? model);
}