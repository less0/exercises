using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace consumer;

internal class RabbitMqConsumer : IHostedService
{
    private readonly ILogger<RabbitMqConsumer> _logger;

    public RabbitMqConsumer(ILogger<RabbitMqConsumer> logger)
    {
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Started service.");
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopped service.");
        return Task.CompletedTask;
    }
}