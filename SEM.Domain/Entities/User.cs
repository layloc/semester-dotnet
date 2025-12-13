using System.ComponentModel.DataAnnotations;

namespace SEM.Entities;

public class User
{
    public Guid Id { get; set; }
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    [Required]
    public string HashPassword { get; set; }
    public List<Model> SavedModels { get; set; }
}