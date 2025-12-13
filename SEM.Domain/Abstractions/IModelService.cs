using SEM.Domain.DTOs;
using SEM.Entities;

namespace SEM.Domain.Abstractions;

public interface IModelService
{
    Task UpdateModelAsync(ModelDto? model);
    Task DeleteModelAsync(Guid modelId);
    Task<Model?> GetModelAsync(Guid model);
    Task<IEnumerable<Model>> GetModelsAsync();
    Task CreateModelAsync(Model? model, Guid userId);
}