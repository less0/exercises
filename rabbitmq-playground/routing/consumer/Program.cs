using consumer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var hostBuilder = Host.CreateApplicationBuilder();
hostBuilder.Services.AddHostedService<RabbitMqConsumer>();

var host = hostBuilder.Build();
host.Run();
