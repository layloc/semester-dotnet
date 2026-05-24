using System.Collections.Generic;

namespace SEM.CodeGen;

public sealed record PropertyDefinition(
    string Name,
    string Type,
    bool IsRequired = false,
    bool IsReadonly = false);

public sealed record TypeDefinition(
    string Name,
    IReadOnlyList<PropertyDefinition> Properties);
