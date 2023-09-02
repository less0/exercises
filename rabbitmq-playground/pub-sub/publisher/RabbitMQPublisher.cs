using System.Text;
using System.Timers;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

namespace publisher;

internal class RabbitMQPublisher : IHostedService
{
    readonly ILogger<RabbitMQPublisher> _logger;
    System.Timers.Timer? _timer;
    private IConnection? _connection;
    private IModel? _channel;
    private int _runningNumber = 0;

    public RabbitMQPublisher(ILogger<RabbitMQPublisher> logger)
    {
        _logger = logger;
    }

    private bool IsConnected => _connection?.IsOpen ?? false;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await ConnectWithRetryAsync(cancellationToken);
        StartTimer();
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Dispose();
        _connection?.Dispose();
        _channel?.Dispose();
    }

    private async Task ConnectWithRetryAsync(CancellationToken cancellationToken)
    {
        var timeout = TimeSpan.FromMinutes(5);
        var startedAt = DateTime.Now;

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
        ConnectionFactory connectionFactory = new() { HostName = "rabbitmq" };
        _connection = connectionFactory.CreateConnection();

        _channel = _connection.CreateModel();
        _channel.ExchangeDeclare("logs", ExchangeType.Fanout);
    }

    private void StartTimer()
    {
        _timer = new(2000)
        {
            AutoReset = true,
            Enabled = true,
        };
        _timer.Elapsed += OnTimerElapsed;
    }

    private void OnTimerElapsed(object? sender, ElapsedEventArgs e)
    {
        _logger.LogInformation("Timer elapsed");
        if(!IsConnected)
        {
            _logger.LogInformation("Not connected.");
        }

        _logger.LogInformation("Sending message");

        var message = $"Message {_runningNumber}";
        _channel.BasicPublish("logs", string.Empty, null, Encoding.UTF8.GetBytes(message));

        _runningNumber++;
    }
}