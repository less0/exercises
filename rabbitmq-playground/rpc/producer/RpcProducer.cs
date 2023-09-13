using System.Timers;
using Kertscher.RabbitMq.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using Timer = System.Timers.Timer;

namespace producer;

public class RpcProducer : RabbitMqWorker
{
    private readonly ILogger<RpcProducer> _logger;
    private readonly Timer _timer;

    public RpcProducer(ILogger<RpcProducer> logger, IConfiguration configuration) 
        : base(logger, configuration)
    {
        _logger = logger;
        _timer = new()
        {
            Enabled = false,
            AutoReset = true,
            Interval = 5000
        };
        _timer.Elapsed += TimerOnElapsed;
    }

    protected override async Task OnConnected(IConnection connection, bool reconnected)
    {
        await base.OnConnected(connection, reconnected);
        _timer.Start();
    }

    private void TimerOnElapsed(object? sender, ElapsedEventArgs e)
    {
        _logger.LogInformation("Timer elapsed");

        if (!IsConnected)
        {
            _logger.LogError("Not connected.");
            return;
        }
    }
}