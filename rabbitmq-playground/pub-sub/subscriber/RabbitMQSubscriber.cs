using System.Text;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace subscriber;

internal class RabbitMQSubscriber : IHostedService
{
    private readonly ILogger<RabbitMQSubscriber> _logger;
    private IConnection? _connection;
    private IModel? _channel;

    public RabbitMQSubscriber(ILogger<RabbitMQSubscriber> logger)
    {
        _logger = logger;
    }

    private bool IsConnected => _connection?.IsOpen ?? false;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await ConnectWithRetryAsync(cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _connection?.Dispose();
        _channel?.Dispose();
    }

    private async Task ConnectWithRetryAsync(CancellationToken cancellationToken)
    {
        TimeSpan timeout = TimeSpan.FromMinutes(5);
        DateTime startedAt = DateTime.Now;

        _logger.LogInformation("Connecting...");

        while(!cancellationToken.IsCancellationRequested
            && !IsConnected)
        {
            try
            {
                ConnectionFactory connectionFactory = new() { HostName = "rabbitmq" };
                _connection = connectionFactory.CreateConnection();
                _channel = _connection.CreateModel();

                _channel.ExchangeDeclare(exchange: "logs", type: ExchangeType.Fanout);

                var queueName = _channel.QueueDeclare().QueueName;
                _channel.QueueBind(queueName, "logs", string.Empty);

                EventingBasicConsumer consumer = new(_channel);
                consumer.Received += OnMessageReceived;

                _channel.BasicConsume(queueName, true, consumer);
            }
            catch (BrokerUnreachableException)
            {
                if(DateTime.Now - startedAt > timeout)
                {
                    _logger.LogCritical("Connection timeout exceeded.");
                    throw new TimeoutException();
                }
                
                _logger.LogWarning("Connection failed. Retrying in 5 s.");
                await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
            }
        }
    }

    private void OnMessageReceived(object? sender, BasicDeliverEventArgs e)
    {
        _logger.LogInformation("Received message");
        var message = Encoding.UTF8.GetString(e.Body.ToArray());
        _logger.LogInformation(message);
    }
}