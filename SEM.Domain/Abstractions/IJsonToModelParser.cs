using System.Text.Json.Nodes;
using SEM.Domain.Entities;

namespace SEM.Abstractions;

public interface IJsonToModelParser
{
    public Model ParseJsonToModel(string name, JsonNode json);
}