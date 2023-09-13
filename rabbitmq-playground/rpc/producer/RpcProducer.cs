using System.Collections.Concurrent;
using System.Text;
using System.Timers;
using Kertscher.RabbitMq.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Timer = System.Timers.Timer;

namespace producer;

public class RpcProducer : RabbitMqWorker
{
    private readonly ILogger<RpcProducer> _logger;
    private readonly Timer _timer;
    private IModel? _channel;
    private string? _replyQueueName;
    private EventingBasicConsumer? _consumer;

    private readonly ConcurrentDictionary<string, TaskCompletionSource<byte[]>> _callbackMapper = new();

    public RpcProducer(ILogger<RpcProducer> logger, IConfiguration configuration) 
        : base(logger, configuration)
    {
        _logger = logger;
        _timer = new()
        {
            Enabled = false,
            AutoReset = true,
            Interval = 5000
        };
        _timer.Elapsed += TimerOnElapsed;
    }

    protected override Task OnDisconnected()
    {
        _channel?.Dispose();
        if (_consumer != null)
        {
            _consumer.Received -= ConsumerOnReceived;
        }
        _replyQueueName = null;
        return base.OnDisconnected();
    }

    protected override async Task OnConnected(IConnection connection, bool reconnected)
    {
        await base.OnConnected(connection, reconnected);
        _channel = connection.CreateModel();
        _replyQueueName = _channel.QueueDeclare().QueueName;
        _channel.ExchangeDeclare("target", ExchangeType.Direct);

        _consumer = new EventingBasicConsumer(_channel);
        _consumer.Received += ConsumerOnReceived;
        _channel.BasicConsume(_consumer, _replyQueueName, true);
        
        _timer.Start();
    }

    private void TimerOnElapsed(object? sender, ElapsedEventArgs e)
    {
        _logger.LogInformation("Timer elapsed");

        if (!IsConnected)
        {
            _logger.LogError("Not connected.");
            return;
        }

        CancellationTokenSource cancellationTokenSource = new(2000);
        CancellationToken cancellationToken = cancellationTokenSource.Token;
        TaskCompletionSource<byte[]> taskCompletionSource = new();
        
        var properties = _channel!.CreateBasicProperties();
        properties.ReplyTo = _replyQueueName;
        properties.CorrelationId = Guid.NewGuid().ToString();
        _callbackMapper.TryAdd(properties.CorrelationId, taskCompletionSource);
        
        _channel.BasicPublish("target", "getSomething", properties, Array.Empty<byte>());

        cancellationToken.Register(() =>
        {
            if (!_callbackMapper.TryRemove(properties.CorrelationId, out var tcs))
            {
                return;
            }
            tcs.SetCanceled(cancellationToken);
            _logger.LogInformation("Call was cancelled.");
        });

        taskCompletionSource.Task.Wait(cancellationToken);
        if (taskCompletionSource.Task.IsCompletedSuccessfully)
        {
            var message = Encoding.UTF8.GetString(taskCompletionSource.Task.Result);
            _logger.LogInformation("Received response: {message}", message);
        }
    }

    private void ConsumerOnReceived(object? sender, BasicDeliverEventArgs e)
    {
        if (!_callbackMapper.TryRemove(e.BasicProperties.CorrelationId, out var taskCompletionSource))
        {
            return;
        }

        taskCompletionSource.TrySetResult(e.Body.ToArray());
    }
}