using System.Text;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

using Timer = System.Timers.Timer;

namespace producer;

internal class RabbitMqProducer : IHostedService
{
    private readonly ILogger<RabbitMqProducer> _logger;
    private Timer? _timer;
    private IConnection? _connection;
    private IModel? _channel;

    public RabbitMqProducer(ILogger<RabbitMqProducer> logger)
    {
        _logger = logger;
    }

    private bool IsConnected => _connection?.IsOpen ?? false;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await ConnectWithRetryAsyny(cancellationToken);

        _timer = new()
        {
            Enabled = true,
            AutoReset = true,
            Interval = 2000.0
        };
        _timer.Elapsed += OnTimerElapsed;

        _logger.LogInformation("Started service.");
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _connection?.Dispose();
        _channel?.Dispose();
        _timer?.Dispose();

        _logger.LogInformation("Stopped service.");
        return Task.CompletedTask;
    }

    private async Task ConnectWithRetryAsyny(CancellationToken cancellationToken)
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
        _channel.ExchangeDeclare("logs", ExchangeType.Direct);
    }

    private void OnTimerElapsed(object? sender, System.Timers.ElapsedEventArgs e)
    {
        _logger.LogInformation("Timer elapsed.");

        if(!IsConnected)
        {
            _logger.LogWarning("Not connected.");
            return;
        }

        var (message, route) = Random.Shared.NextDouble() switch
        {
            <.1 => ("Error message", "error"),
            <.3 => ("Warning message", "warning"),
            _ => ("Info message", "info")
        };

        _logger.LogInformation("Sending message. ({severity})", route);
        _channel.BasicPublish("logs", route, null, Encoding.UTF8.GetBytes(message));
    }
}