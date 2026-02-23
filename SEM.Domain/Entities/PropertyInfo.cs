namespace SEM.Domain.Entities;

public class PropertyInfo
{
    public Guid Id { get; set; }
    public required string Type { get; set; }
    public required string Name { get; set; }
    public bool IsReadonly { get; set; } = false;
    public bool IsRequired { get; set; } = false;
    public Model Model { get; set; }
    public Guid ModelId { get; set; }
}