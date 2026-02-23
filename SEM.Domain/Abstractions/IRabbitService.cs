using RabbitMQ.Client;

namespace SEM.Domain.Abstractions;

public interface IRabbitService
{
    public void Publish(string queue, string message);
}