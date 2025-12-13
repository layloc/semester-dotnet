using System.Text;
using SEM.Abstractions;
using SEM.Entities;

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
                ? $"\tpublic required {property.Type} {property.Name} {{ get; "
                : $"\tpublic {property.Type} {property.Name} {{ get; ").Append(property.IsReadonly ? "}\n" : "set; }\n");
            output.Append(propToCode.ToString());
        }
        output.AppendLine("}");
        return output.ToString();
    }
}