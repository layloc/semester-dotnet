using SEM.Domain.Entities;

namespace SEM.Domain.Abstractions;

public interface IApiGenerator
{
    public string GenerateCrudEndpoints(Model model);
    public string GenerateDbContext(Model model);
}