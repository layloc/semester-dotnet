using System.Text.Json.Nodes;
using SEM.Abstractions;
using SEM.Entities;

namespace SEM.Services;

public class JsonToModelParser : IJsonToModelParser
{
    public Model ParseJsonToModel(string name, JsonNode json)
    {
        var model = new Model { Name = name, Properties = new List<PropertyInfo>()};
        if (json is JsonObject jsonObject)
        {
            foreach (var jn in jsonObject)
            {
                var propertyName = jn.Key;
                var propertyType = GetTypeOfJsonNode(jn.Value, propertyName);
                model.Properties.Add(new PropertyInfo { Name = propertyName, Type = propertyType, IsReadonly = false });
            }
        }

        return model;
    }

    private string GetTypeOfJsonNode(JsonNode? node, string propertyName)
    {
        if (node is null)
        {
            return "object";
        }

        if (node is JsonObject)
        {
            return propertyName;
        }
        
        if (node is JsonValue val)
        {
            if (val.TryGetValue<int>(out _)) return "int";
            if (val.TryGetValue<long>(out _)) return "long";
            if (val.TryGetValue<float>(out _)) return "float";
            if (val.TryGetValue<double>(out _)) return "double";
            if (val.TryGetValue<decimal>(out _)) return "decimal";
            if (val.TryGetValue<bool>(out _)) return "bool";
            if (val.TryGetValue<string>(out _)) return "string";
        }

        
        if (node is JsonArray jsonArray)
        {
            foreach (var i in jsonArray)
            {
                return $"List<{GetTypeOfJsonNode(i, propertyName)}>";
            }
        }

        return "object";
    }
}