using System.ComponentModel.DataAnnotations;

namespace SEM.Domain.DTOs;

public record UserAuthRequest
{
    [Required]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }
}