using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace producer;

internal class RabbitMqProducer : IHostedService
{
    private readonly ILogger<RabbitMqProducer> _logger;

    public RabbitMqProducer(ILogger<RabbitMqProducer> logger)
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