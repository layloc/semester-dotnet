using System.Text;
using RabbitMQ.Client;

namespace SEM.Infrastructure.Brokers;

public class RabbitService
{
    private readonly IChannel _channel;

    public RabbitService()
    {
        var factory = new ConnectionFactory
        {
            HostName = "localhost"
        };

        var connection = factory.CreateConnectionAsync().Result;
        _channel = connection.CreateChannelAsync().Result;
    }

    public async Task PublishAsync(string queue, string message)
    {
        await _channel.QueueDeclareAsync(
            queue: queue,
            durable: false,
            exclusive: false,
            autoDelete: false);

        var body = Encoding.UTF8.GetBytes(message);

        await _channel.BasicPublishAsync(
            exchange: "",
            routingKey: queue,
            body: body);
    }
}