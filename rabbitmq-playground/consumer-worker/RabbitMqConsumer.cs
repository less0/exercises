using System.Text;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace consumer_worker;

internal class RabbitMqConsumer : IHostedService
{
    private readonly ILogger<RabbitMqConsumer> _logger;
    private IConnection? _connection;
    private IModel? _channel;
    private EventingBasicConsumer? _consumer;

    public RabbitMqConsumer(ILogger<RabbitMqConsumer> logger)
    {
        _logger = logger;
    }

    private bool IsConnected => _connection?.IsOpen ?? false;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await ConnectWithRetryAsync(cancellationToken);
        _logger.LogInformation("StartAsync was called.");
        return;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _connection?.Dispose();
        _channel?.Dispose();

        _logger.LogInformation("StopAsync was called.");
        return Task.CompletedTask;
    }

    private async Task ConnectWithRetryAsync(CancellationToken cancellationToken)
    {
        TimeSpan timeout = TimeSpan.FromMinutes(5);
        DateTime startedAt = DateTime.Now;

        while (!cancellationToken.IsCancellationRequested && !IsConnected)
        {
            try
            {
                Connect();
            }
            catch (BrokerUnreachableException)
            {
                if (DateTime.Now - startedAt > timeout)
                {
                    _logger.LogCritical("Connection timeout exceeded.");
                    throw new TimeoutException();
                }

                _logger.LogWarning("Connection failed. Retrying in 5 s.");
                await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
            }
        }
    }

    private void Connect()
    {
        ConnectionFactory factory = new() { HostName = "rabbitmq" };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.QueueDeclare(queue: "hello",
             durable: false,
             exclusive: false,
             autoDelete: false,
             arguments: null);

        _consumer = new EventingBasicConsumer(_channel);
        _consumer.Received += OnMessageReceived;
        _channel.BasicConsume(queue: "hello",
                             autoAck: true,
                             consumer: _consumer);
    }

    private void OnMessageReceived(object? sender, BasicDeliverEventArgs e)
    {
        var body = e.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);
        _logger.LogInformation("Received message: {message}", message);
    }
}