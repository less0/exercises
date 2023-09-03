using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using producer;

var hostBuilder = Host.CreateApplicationBuilder();
hostBuilder.Services.AddHostedService<RabbitMqProducer>();

var host = hostBuilder.Build();
host.Run();
