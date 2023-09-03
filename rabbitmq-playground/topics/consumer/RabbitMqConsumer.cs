using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace consumer;

internal class RabbitMqConsumer : IHostedService
{
    private readonly ILogger<RabbitMqConsumer> _logger;
    private readonly IConfiguration _configuration;
    private readonly string _topic;
    private IConnection? _connection;
    private IModel? _channel;

    public RabbitMqConsumer(ILogger<RabbitMqConsumer> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;

        _topic = _configuration["Topic"]!;
    }

    public bool IsConnected => _connection?.IsOpen ?? false;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await ConnectWithRetryAsync(cancellationToken);
        _logger.LogInformation("Topic: {topic}", _topic);

        _logger.LogInformation("Started service.");
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _connection?.Dispose();
        _channel?.Dispose();

        _logger.LogInformation("Stopped service.");
        return Task.CompletedTask;
    }

    private async Task ConnectWithRetryAsync(CancellationToken cancellationToken)
    {
        var timeout = TimeSpan.FromMinutes(5);
        var startedAt = DateTime.Now;

        _logger.LogInformation("Connecting...");

        while (!cancellationToken.IsCancellationRequested
            && !IsConnected)
        {
            try
            {
                Connect();
                _logger.LogInformation("Connected.");
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

    private void Connect()
    {
        ConnectionFactory connectionFactory = new()
        {
            HostName = "rabbitmq"
        };
        _connection = connectionFactory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.ExchangeDeclare("logs", ExchangeType.Topic);

        var queueName = _channel.QueueDeclare().QueueName;
        _channel.QueueBind(queueName, "logs", _topic);

        EventingBasicConsumer consumer = new(_channel);
        consumer.Received += OnMessageReceived;
        _channel.BasicConsume(consumer, queueName, autoAck: true);
    }

    private void OnMessageReceived(object? sender, BasicDeliverEventArgs e)
    {
        _logger.LogInformation("Message received");
        var message = Encoding.UTF8.GetString(e.Body.ToArray());
        _logger.LogInformation("{rountingKey}: {message}", e.RoutingKey, message);
    }
}