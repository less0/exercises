using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using publisher;

var hostBuilder = Host.CreateApplicationBuilder();
hostBuilder.Services.AddHostedService<RabbitMQPublisher>();

var host = hostBuilder.Build();
host.Run();