using Kertscher.RabbitMq.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var hostBuilder = Host.CreateApplicationBuilder();
hostBuilder.Services.AddHostedService<RabbitMqWorker>();

var host = hostBuilder.Build();
host.Run();