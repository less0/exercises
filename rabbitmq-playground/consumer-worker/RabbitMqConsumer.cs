using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace consumer_worker;

internal class RabbitMqConsumer : IHostedService
{
    private readonly ILogger<RabbitMqConsumer> _logger;

    public RabbitMqConsumer(ILogger<RabbitMqConsumer> logger)
    {
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("StartAsync was called.");
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("StopAsync was called.");
        return Task.CompletedTask;
    }
}