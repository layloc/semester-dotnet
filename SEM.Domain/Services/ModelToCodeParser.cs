using SEM.Abstractions;
using SEM.CodeGen;
using SEM.Domain.Entities;

namespace SEM.Services;

public class ModelToCodeParser : IModelToCodeParser
{
    public string ParseModelToCode(Model model) =>
        EntityClassGenerator.Generate(ToTypeDefinition(model));

    internal static TypeDefinition ToTypeDefinition(Model model) =>
        DomainModelMapper.ToTypeDefinition(
            model.Name,
            model.Properties.Select(p => (p.Name, p.Type, p.IsRequired, p.IsReadonly)));
}
