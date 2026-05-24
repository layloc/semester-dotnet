using System.Collections.Generic;
using System.Linq;

namespace SEM.CodeGen;

public static class DomainModelMapper
{
    public static TypeDefinition ToTypeDefinition(
        string name,
        IEnumerable<(string Name, string Type, bool IsRequired, bool IsReadonly)> properties) =>
        new(
            name,
            properties
                .Select(p => new PropertyDefinition(p.Name, p.Type, p.IsRequired, p.IsReadonly))
                .ToList());
}
