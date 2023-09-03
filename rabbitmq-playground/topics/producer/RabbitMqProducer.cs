using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using Timer = System.Timers.Timer;

namespace producer;

internal class RabbitMqProducer : IHostedService
{
    private readonly ILogger<RabbitMqProducer> _logger;
    private readonly IConfiguration _configuration;
    private readonly string _topic;
    private readonly Timer _timer;
    private IConnection? _connection;
    private IModel? _channel;

    public RabbitMqProducer(ILogger<RabbitMqProducer> logger, IConfiguration configuration)
    {
        _logger = logger;
        _timer = new()
        {
            Enabled = false,
            AutoReset = true,
            Interval = 2000.0
        };
        _timer.Elapsed += OnTimerElapsed;
        _configuration = configuration;
        _topic = _configuration["Topic"]!;
    }

    public bool IsConnected => _connection?.IsOpen ?? false;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await ConnectWithRetryAsync(cancellationToken);
        _timer.Enabled = true;

        _logger.LogInformation("Started service.");
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer.Dispose();
        _connection?.Dispose();
        _channel?.Dispose();

        _logger.LogInformation("Stopped service");
        return Task.CompletedTask;
    }

    private void OnTimerElapsed(object? sender, System.Timers.ElapsedEventArgs e)
    {
        _logger.LogInformation("Timer elapsed.");

        if(!IsConnected)
        {
            _logger.LogWarning("Not connected.");
            return;
        }

        _logger.LogInformation("Sending message");
        var (message, severity) = Random.Shared.NextDouble() switch
        {
            <.1 => ("Error message", "error"),
            <.3 => ("Warning message", "warning"), 
            _ => ("Info message", "info")
        };

        _channel.BasicPublish("logs",
            $"{_topic}.{severity}",
            null,
            Encoding.UTF8.GetBytes(message));
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
        _channel.ExchangeDeclare("logs",
            ExchangeType.Topic);
    }
}