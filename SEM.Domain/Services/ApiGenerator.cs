using SEM.CodeGen;
using SEM.Domain.Abstractions;
using SEM.Domain.Entities;

namespace SEM.Domain.Services;

public class ApiGenerator : IApiGenerator
{
    public string GenerateCrudEndpoints(Model model) =>
        CrudEndpointsGenerator.Generate(ToTypeDefinition(model));

    public string GenerateDbContext(Model model) =>
        DbContextGenerator.Generate(ToTypeDefinition(model));

    private static TypeDefinition ToTypeDefinition(Model model) =>
        DomainModelMapper.ToTypeDefinition(
            model.Name,
            model.Properties.Select(p => (p.Name, p.Type, p.IsRequired, p.IsReadonly)));
}
