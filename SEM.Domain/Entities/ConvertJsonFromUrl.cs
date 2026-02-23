using System.ComponentModel.DataAnnotations;

namespace SEM.Domain.Entities;

public class ConvertJsonFromUrl
{
    [Required]
    public string Url { get; set; }
    [Required]
    public string Name { get; set; }
}