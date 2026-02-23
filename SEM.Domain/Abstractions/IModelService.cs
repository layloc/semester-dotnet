using SEM.Domain.DTOs;
using SEM.Domain.Entities;

namespace SEM.Domain.Abstractions;

public interface IModelService
{
    Task UpdateModelAsync(Model? model, Guid userId);
    Task DeleteModelAsync(Guid modelId, Guid userId);
    Task<Model?> GetModelAsync(Guid model);
    Task<IEnumerable<Model>> GetModelsAsync();
    Task CreateModelAsync(Model? model, Guid userId);
    Task SaveModelAsync(ModelDto dto, Guid userId);
    
}