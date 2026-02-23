using SEM.Domain.Abstractions;
using SEM.Domain.DTOs;
using SEM.Domain.Entities;

namespace SEM.Domain.Services;

public class ModelService(IModelRepository modelRepository) : IModelService
{
    public async Task UpdateModelAsync(Model? model, Guid userId)
    {
        var modelToUpdate = await modelRepository.GetModelByIdAsync(model.Id);
        if (modelToUpdate == null || modelToUpdate.UserId != userId)
        {
            throw new Exception("Model not updated");
        }
        var updatedModel = new Model{ Id = model.Id, Name = model.Name, Properties = model.Properties }; 
        await modelRepository.UpdateModelAsync(updatedModel);
    }

    public async Task DeleteModelAsync(Guid modelId, Guid userId)
    {
        var modelToDelete = await modelRepository.GetModelByIdAsync(modelId);
        if (modelToDelete == null || modelToDelete.UserId != userId)
        {
            throw new Exception("Model not updated");
        }
        await modelRepository.DeleteModelAsync(modelToDelete);
    }

    public Task<Model?> GetModelAsync(Guid modelId)
    {
        return modelRepository.GetModelByIdAsync(modelId);
    }

    public async Task<IEnumerable<Model>> GetModelsAsync()
    {
        throw new NotImplementedException();
    }
    

    public async Task CreateModelAsync(Model? model, Guid userId)
    {
        model!.UserId = userId;
        await modelRepository.CreateModelAsync(model);
    }

    public Task SaveModelAsync(ModelDto dto, Guid userId)
    {
        var model = new Model { UserId = userId, Properties = dto.Properties, Id = dto.Id };
        return modelRepository.UpdateModelAsync(model);
    }
}