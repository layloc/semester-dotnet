using System.ComponentModel.DataAnnotations;
using SEM.Domain.Entities;

namespace SEM.Domain.DTOs;

public class ModelDto
{
    public Guid Id { get; set; }
    [Required]
    public string Name { get; set; }
    [Required]
    public List<PropertyInfo> Properties { get; set; }
}