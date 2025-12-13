using System.ComponentModel.DataAnnotations.Schema;

namespace SEM.Entities;

public class Model
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; }
    public List<PropertyInfo> Properties { get; set; }
    public List<Model> RelatedModels { get; set; }
    public User User { get; set; }
    [ForeignKey(nameof(User))]
    public Guid UserId { get; set; }
}