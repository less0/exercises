using System.Text;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using Timer = System.Timers.Timer;

namespace producer_worker;

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
        await ConnectWithRetryAsync(cancellationToken);
        StartTimer();

        _logger.LogInformation("StartAsync was called.");
        return;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Dispose();
        _connection?.Dispose();
        _channel?.Dispose();
        _logger.LogInformation("StopAsync was called");
        return Task.CompletedTask;
    }

    private async Task ConnectWithRetryAsync(CancellationToken cancellationToken)
    {
        TimeSpan timeout = TimeSpan.FromMinutes(5);
        DateTime startedAt = DateTime.Now;

        while (!cancellationToken.IsCancellationRequested
            && !IsConnected)
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
        var factory = new ConnectionFactory { HostName = "rabbitmq" };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.QueueDeclare(queue: "hello",
                             durable: false,
                             exclusive: false,
                             autoDelete: false,
                             arguments: null);
    }

    private void StartTimer()
    {
        _timer = new()
        {
            AutoReset = true,
            Enabled = true,
            Interval = 2000.0,
        };

        _timer.Elapsed += OnTimerElapsed;
    }

    private void OnTimerElapsed(object? sender, System.Timers.ElapsedEventArgs e)
    {
        _logger.LogInformation("Timer elapsed.");
        if (!IsConnected)
        {
            _logger.LogInformation("Not connected.");
            return;
        }

        SendMessage();
    }

    private void SendMessage()
    {
        _logger.LogInformation("Sending message...");

        const string message = "Hello World!";
        var body = Encoding.UTF8.GetBytes(message);

        _channel.BasicPublish(exchange: string.Empty,
                             routingKey: "hello",
                             basicProperties: null,
                             body: body);

        _logger.LogInformation("Sent message.");
    }
}