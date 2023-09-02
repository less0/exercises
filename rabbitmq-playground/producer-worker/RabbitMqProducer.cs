using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Timer = System.Timers.Timer;

namespace producer_worker;

internal class RabbitMqProducer : IHostedService
{
    private readonly ILogger<RabbitMqProducer> _logger;
    private Timer _timer;

    public RabbitMqProducer(ILogger<RabbitMqProducer> logger)
    {
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _timer = new()
        {
            AutoReset = true,
            Enabled = true,
            Interval = 2000.0,
        };

        _timer.Elapsed += OnTimerElapsed;

        _logger.LogInformation("StartAsync was called.");
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer.Dispose();
        _logger.LogInformation("StopAsync was called");
        return Task.CompletedTask;
    }

    private void OnTimerElapsed(object? sender, System.Timers.ElapsedEventArgs e)
    {
        _logger.LogInformation("Timer elapsed.");
    }
}