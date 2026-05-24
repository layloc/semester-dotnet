using System;
using System.Collections.Generic;

namespace SEM.CodeGen;

public static class TypeDefinitionMapper
{
    public static TypeDefinition FromSchema(string name, IReadOnlyList<PropertyDefinition> properties) =>
        new(name, properties);

    public static TypeDefinition FromJson(string json)
    {
        var document = System.Text.Json.JsonDocument.Parse(json);
        var root = document.RootElement;

        var name = root.GetProperty("name").GetString()
                   ?? throw new InvalidOperationException("Schema must contain 'name'.");

        var properties = new List<PropertyDefinition>();
        if (root.TryGetProperty("properties", out var propertiesElement) &&
            propertiesElement.ValueKind == System.Text.Json.JsonValueKind.Array)
        {
            foreach (var property in propertiesElement.EnumerateArray())
            {
                properties.Add(new PropertyDefinition(
                    property.GetProperty("name").GetString()!,
                    property.GetProperty("type").GetString()!,
                    property.TryGetProperty("isRequired", out var isRequired) && isRequired.GetBoolean(),
                    property.TryGetProperty("isReadonly", out var isReadonly) && isReadonly.GetBoolean()));
            }
        }

        return new TypeDefinition(name, properties);
    }
}
