using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace consumer;

internal class RabbitMqConsumer : IHostedService
{
    readonly ILogger<RabbitMqConsumer> _logger;
    readonly IConfiguration _configuration;
    private readonly string[] _routes;

    public RabbitMqConsumer(ILogger<RabbitMqConsumer> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;

        // The routes
        _routes = _configuration["Routes"]?.Split(",") ?? Array.Empty<string>();
    }


    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Routes: {routes}", string.Join(", ", _routes));
        _logger.LogInformation("Service started.");
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Service stopped.");
        return Task.CompletedTask;
    }
}