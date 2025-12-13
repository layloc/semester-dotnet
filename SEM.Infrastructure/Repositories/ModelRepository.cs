using Microsoft.EntityFrameworkCore;
using SEM.Domain.Abstractions;
using SEM.Entities;

namespace SEM.Infrastructure.Repositories;

public class ModelRepository(AppDbContext dbContext) : IModelRepository
{
    public async Task<Model?> GetModelAsync(Guid id)
    {
        return await dbContext.Models.Where(m => m.Id == id).FirstOrDefaultAsync();
    }

    public async Task<Model?> UpdateModelAsync(Model? model)
    {
        var exist = await dbContext.FindAsync<Model>(model.Id);
        exist.Name = model.Name;
        exist.Properties = model.Properties;
        await dbContext.SaveChangesAsync();
        return await Task.FromResult(model);
    }

    public async Task DeleteModelAsync(Model? model)
    {
        var exist = await dbContext.Models.Where(m => m.Id == model.Id).FirstOrDefaultAsync();
        dbContext.Models.Remove(exist);
        await dbContext.SaveChangesAsync();
    }

    public async Task<Guid> CreateModelAsync(Model? model)
    {
        await dbContext.Models.AddAsync(model);
        await dbContext.SaveChangesAsync();
        return model.Id;
    }
}