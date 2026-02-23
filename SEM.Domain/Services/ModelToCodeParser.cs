using System.Text;
using SEM.Abstractions;
using SEM.Domain.Entities;

namespace SEM.Services;

public class ModelToCodeParser : IModelToCodeParser
{
    public string ParseModelToCode(Model model)
    {
        var output = new StringBuilder();
        output.AppendLine($"public class {model.Name}\n{{");
        foreach (var property in model.Properties)
        {
            var propToCode = new StringBuilder();
            propToCode.Append(property.IsRequired
                ? $"    public required {property.Type} {property.Name} {{ get; "
                : $"    public {property.Type} {property.Name} {{ get; ").Append(property.IsReadonly ? "}\n" : "set; }\n");
            output.Append(propToCode.ToString());
        }
        output.AppendLine("}");
        return output.ToString();
    }
}