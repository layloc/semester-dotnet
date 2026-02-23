using Microsoft.EntityFrameworkCore;
using SEM.Domain.Abstractions;
using SEM.Domain.Entities;

namespace SEM.Infrastructure.Repositories;

public class ModelRepository(AppDbContext dbContext) : IModelRepository
{
    public async Task<Model?> GetModelByIdAsync(Guid id)
    {
        return await dbContext.Models.Where(m => m.Id == id).Include(m => m.Properties).FirstOrDefaultAsync();
    }

    public async Task<Model?> UpdateModelAsync(Model? model)
    {
        if (model == null) return null;

        var exist = await dbContext.Models
            .Include(m => m.Properties) 
            .FirstOrDefaultAsync(m => m.Id == model.Id);

        if (exist == null) return null;

        exist.Name = model.Name;

        dbContext.PropertyInfos.RemoveRange(exist.Properties);
        exist.Properties = model.Properties;

        await dbContext.SaveChangesAsync();
        return exist;
    }

    public async Task DeleteModelAsync(Model? model)
    {
        var exist = await dbContext.Models.Where(m => m.Id == model.Id).FirstOrDefaultAsync();
        if (exist != null) dbContext.Models.Remove(exist);
        await dbContext.SaveChangesAsync();
    }

    public async Task<Guid> CreateModelAsync(Model? model)
    {
        if (model != null) await dbContext.Models.AddAsync(model);
        await dbContext.SaveChangesAsync();
        return model.Id;
    }

    public async Task<IEnumerable<Model?>> GetModelsOfUserAsync(Guid userId)
    {
        var models = await dbContext.Models.Where(m => m.UserId == userId).Include(m => m.Properties).ToListAsync();
        return models;
    }
}