using consumer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var hostBuilder = Host.CreateApplicationBuilder();
hostBuilder.Services.AddHostedService<RpcConsumer>();

var host = hostBuilder.Build();
host.Run();
