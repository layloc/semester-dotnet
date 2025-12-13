using System.Text.Json.Nodes;
using SEM.Entities;

namespace SEM.Abstractions;

public interface IJsonToModelParser
{
    public Model ParseJsonToModel(string name, JsonNode json);
}