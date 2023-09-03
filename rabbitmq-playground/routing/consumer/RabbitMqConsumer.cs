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
    readonly ILogger<RabbitMqConsumer> _logger;
    readonly IConfiguration _configuration;
    private readonly string[] _routes;
    private IConnection? _connection;
    private IModel? _channel;

    public RabbitMqConsumer(ILogger<RabbitMqConsumer> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;

        // The routes can be set via appsettings.json or by setting the "Route" environment 
        // variable, as can be seen the file docker-compose.yml.
        _routes = _configuration["Routes"]?.Split(",") ?? Array.Empty<string>();
    }

    private bool IsConnected => _connection?.IsOpen ?? false;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Routes: {routes}", string.Join(", ", _routes));
        await ConnectWithRetryAsync(cancellationToken);

        _logger.LogInformation("Service started.");
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _channel?.Dispose();
        _connection?.Dispose();

        _logger.LogInformation("Service stopped.");
        return Task.CompletedTask;
    }

    private async Task ConnectWithRetryAsync(CancellationToken cancellationToken)
    {
        var timeout = TimeSpan.FromMinutes(5);
        var startedAt = DateTime.Now;

        _logger.LogInformation("Connecting...");

        while(!cancellationToken.IsCancellationRequested
            && !IsConnected)
        {
            try
            {
                Connect();
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

        _channel.ExchangeDeclare("logs", ExchangeType.Direct);
        var queueName = _channel.QueueDeclare().QueueName;

        foreach(var route in _routes)
        {
            _channel.QueueBind(queueName, "logs", route);
        }

        EventingBasicConsumer consumer = new(_channel);
        consumer.Received += OnMessageReceived;
        _channel.BasicConsume(queueName, true, consumer);
    }

    private void OnMessageReceived(object? sender, BasicDeliverEventArgs e)
    {
        string message = Encoding.UTF8.GetString(e.Body.ToArray());
        _logger.LogInformation("Received message.");
        _logger.LogInformation("{severity}: {message}", e.RoutingKey, message);
    }
}