using SEM.Domain.Abstractions;
using SEM.Domain.DTOs;
using SEM.Entities;

namespace SEM.Domain.Services;

public class ModelService(IModelRepository modelRepository) : IModelService
{
    public async Task UpdateModelAsync(ModelDto? modelDto)
    {
        var updatedModel = new Model{ Id = modelDto.Id, Name = modelDto.Name, Properties = modelDto.Properties }; 
        await modelRepository.UpdateModelAsync(updatedModel);
    }

    public Task DeleteModelAsync(Guid modelId)
    {
        throw new NotImplementedException();
    }

    public Task<Model?> GetModelAsync(Guid modelId)
    {
        return modelRepository.GetModelAsync(modelId);
    }

    public Task<IEnumerable<Model>> GetModelsAsync()
    {
        throw new NotImplementedException();
    }


    public async Task<IEnumerable<Model>> GetModelsAsync(Model? model)
    {
        throw new NotImplementedException();
    }

    public async Task CreateModelAsync(Model? model, Guid userId)
    {
        model!.UserId = userId;
        await modelRepository.CreateModelAsync(model);
    }
}