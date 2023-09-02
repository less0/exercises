using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using subscriber;

var hostBuilder = Host.CreateApplicationBuilder();
hostBuilder.Services.AddHostedService<RabbitMQSubscriber>();

var host = hostBuilder.Build();
host.Run();