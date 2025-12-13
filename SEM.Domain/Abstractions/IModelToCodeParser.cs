using SEM.Entities;

namespace SEM.Abstractions;

public interface IModelToCodeParser
{
    public string ParseModelToCode(Model model);
}