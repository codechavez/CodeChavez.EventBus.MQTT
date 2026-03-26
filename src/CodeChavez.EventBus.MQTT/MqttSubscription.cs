using CodeChavez.EventBus.MQTT.Abstractions;
using CodeChavez.EventBus.MQTT.Abstractions.Consumers;
using CodeChavez.EventBus.MQTT.Abstractions.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Formatter;
using System.Text;

namespace CodeChavez.EventBus.MQTT;

public sealed class MqttSubscription : IMqttSubscription, IAsyncDisposable
{
    private readonly ILogger<MqttSubscription> _logger;
    private readonly ConsumerOptions _consumerMqttConfig;
    private readonly SemaphoreSlim _semaphore;
    private readonly Func<string, string, Task>? _messageHandler;

    private IMqttClient? _mqttClient;
    private volatile bool _isConnected;

    public event EventHandler<MqttMessageReceivedEventArgs>? MessageReceived;
    public bool IsConnected => _isConnected;

    public MqttSubscription(
        ILogger<MqttSubscription> logger,
        IOptions<ConsumerOptions> mqttOptions)
    {
        _logger = logger;
        _consumerMqttConfig = mqttOptions.Value;
        _isConnected = false;
        _semaphore = new SemaphoreSlim(_consumerMqttConfig.Consumer.MaxConcurrency, _consumerMqttConfig.Consumer.MaxConcurrency);
    }

    public async Task ConnectAsync(CancellationToken cancellation = default)
    {
        _mqttClient = new MqttClientFactory().CreateMqttClient();

        // Register event handlers
        RegisterEventHandlers();

        var clientId = $"{_consumerMqttConfig.Consumer.ClientId}-{Guid.NewGuid()}";
        var options = new MqttClientOptionsBuilder()
            .WithTcpServer(_consumerMqttConfig.Host, _consumerMqttConfig.Port)
            .WithClientId(clientId)
            .WithProtocolVersion(MqttProtocolVersion.V500)
            .WithCleanStart(true)
            .Build();

        await _mqttClient.ConnectAsync(options, cancellation);
        _logger.LogInformation("Connected to MQTT broker at {Host}:{Port} with client ID {ClientId}",
            _consumerMqttConfig.Host, _consumerMqttConfig.Port, clientId);

        // Subscribe to shared topic for load balancing
        var sharedTopic = $"$share/{_consumerMqttConfig.Consumer.ConsumerGroup}/{_consumerMqttConfig.Consumer.Topic}";

        var subscribeOptions = new MqttClientSubscribeOptionsBuilder()
            .WithTopicFilter(filter =>
            {
                filter.WithTopic(sharedTopic);
                filter.WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce);
            })
            .Build();

        await _mqttClient.SubscribeAsync(subscribeOptions, cancellation);
        _logger.LogInformation("Subscribing to: $share/{ConsumerGroup}/{Topic}", _consumerMqttConfig.Consumer.ConsumerGroup, _consumerMqttConfig.Consumer.Topic);

        _isConnected = true;
    }

    public async Task DisconnectAsync(CancellationToken cancellationToken = default)
    {
        if (_mqttClient is null)
        {
            return;
        }

        try
        {
            if (_mqttClient.IsConnected)
            {
                _logger.LogInformation("Disconnecting from MQTT broker");
                await _mqttClient.DisconnectAsync(cancellationToken: cancellationToken);
            }

            _isConnected = false;
            _logger.LogInformation("Successfully disconnected from MQTT broker");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error disconnecting from MQTT broker");
        }
    }

    public async ValueTask DisposeAsync()
    {
        try
        {
            await DisconnectAsync();
            _mqttClient?.Dispose();
            _semaphore?.Dispose();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during disposal");
        }

        GC.SuppressFinalize(this);
    }

    private void RegisterEventHandlers()
    {
        if (_mqttClient is null)
        {
            return;
        }

        _mqttClient.ConnectedAsync += async e =>
        {
            _isConnected = true;
            _logger.LogInformation("MQTT client connected successfully");
            await Task.CompletedTask;
        };

        _mqttClient.DisconnectedAsync += async e =>
        {
            _isConnected = false;
            _logger.LogWarning("MQTT client disconnected. Reason: {Reason}", e.Reason);
            await Task.CompletedTask;
        };

        _mqttClient.ApplicationMessageReceivedAsync += HandleMessageReceivedAsync;
    }

    private async Task HandleMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs e)
    {
        // Acquire semaphore permit for concurrency control
        await _semaphore.WaitAsync();

        try
        {
            // Decode payload
            var payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
            var topic = e.ApplicationMessage.Topic;

            _logger.LogDebug("Received MQTT message - Topic: {Topic}, Payload length: {PayloadLength}",
                topic, e.ApplicationMessage.Payload.Length);

            // Call configured handler if available
            if (_messageHandler is not null)
            {
                await _messageHandler(topic, payload);
            }

            // Raise event for subscribers
            MessageReceived?.Invoke(this, new MqttMessageReceivedEventArgs(topic, payload));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing received MQTT message");
        }
        finally
        {
            _semaphore.Release();
        }
    }
}