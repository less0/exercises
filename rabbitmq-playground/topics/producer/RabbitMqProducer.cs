using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Timer = System.Timers.Timer;

namespace producer;

internal class RabbitMqProducer : IHostedService
{
    private readonly ILogger<RabbitMqProducer> _logger;
    private readonly Timer _timer;

    public RabbitMqProducer(ILogger<RabbitMqProducer> logger)
    {
        _logger = logger;
        _timer = new()
        {
            Enabled = false,
            AutoReset = true,
            Interval = 2000.0
        };
        _timer.Elapsed += OnTimerElapsed;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _timer.Enabled = true;

        _logger.LogInformation("Started service.");
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopped service");
        return Task.CompletedTask;
    }

    private void OnTimerElapsed(object? sender, System.Timers.ElapsedEventArgs e)
    {
        _logger.LogInformation("Timer elapsed.");
    }
}