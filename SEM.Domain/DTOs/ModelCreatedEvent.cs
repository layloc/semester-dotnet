namespace SEM.Domain.DTOs;

public class ModelCreatedEvent
{
    Guid ModelId { get; set; }
    Guid UserId { get; set; }
    DateTime CreatedAt { get; set; }
}