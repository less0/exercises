using System.Text;
using Kertscher.RabbitMq.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace consumer;

public class RpcConsumer : RabbitMqWorker
{
    private readonly ILogger<RpcConsumer> _logger;
    private IModel? _channel;
    private EventingBasicConsumer? _consumer;

    public RpcConsumer(ILogger<RpcConsumer> logger, IConfiguration configuration) 
        : base(logger, configuration)
    {
        _logger = logger;
    }

    protected override Task OnDisconnected()
    {
        _channel?.Dispose();
        return Task.CompletedTask;
    }

    protected override Task OnConnected(IConnection connection, bool reconnected)
    {
        _channel = connection.CreateModel();
        _channel.ExchangeDeclare("target", ExchangeType.Direct);
        var queueName = _channel.QueueDeclare().QueueName;
        _channel.QueueBind(queueName, "target", "getSomething");
        _consumer = new(_channel);
        _consumer.Received += ConsumerOnReceived;
        _channel.BasicConsume(_consumer, queueName);
        
        return Task.CompletedTask;
    }

    private void ConsumerOnReceived(object? sender, BasicDeliverEventArgs e)
    {
        _logger.LogInformation("Received message.");
        
        var responseBytes = Encoding.UTF8.GetBytes("Something");

        var replyProps = _channel!.CreateBasicProperties();
        replyProps.CorrelationId = e.BasicProperties.CorrelationId;
        
        _channel.BasicPublish(exchange: string.Empty,
            routingKey: e.BasicProperties.ReplyTo,
            basicProperties: replyProps,
            body: responseBytes);
        _channel.BasicAck(deliveryTag: e.DeliveryTag, multiple: false);
    }
}