using CodeChavez.EventBus.MQTT.Abstractions.Consumers;
using CodeChavez.EventBus.MQTT.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Protocol;

namespace CodeChavez.EventBus.MQTT;

public class EventBusMqttClient : IEventBusMqttClient
{
    private readonly ConsumerOptions _mqttOptions;
    private readonly ILogger<EventBusMqttClient> _logger;

    public EventBusMqttClient(
        IOptions<ConsumerOptions> mqttOptions,
        ILogger<EventBusMqttClient> logger)
    {
        _mqttOptions = mqttOptions.Value ?? throw new ArgumentNullException(nameof(mqttOptions));
        _logger = logger;
    }

    public async Task<IMqttClient> ConnectAsync(string? clientId = null)
    {
        var mqttClient = new MqttClientFactory().CreateMqttClient();
        if (!string.IsNullOrEmpty(clientId))
            _mqttOptions.Consumer.ClientId = clientId;

        //TODO: Add TLS support
        var options = new MqttClientOptionsBuilder()
            .WithTcpServer(_mqttOptions.Host, _mqttOptions.Port)
            .WithKeepAlivePeriod(TimeSpan.FromSeconds(_mqttOptions.Consumer.KeepAlinePeriod))
            .WithCleanSession(_mqttOptions.Consumer.CleanSession)
            .WithClientId(_mqttOptions.Consumer.ClientId)
            .Build();

        var resp = await mqttClient.ConnectAsync(options);
        if (resp.ResultCode == MqttClientConnectResultCode.Success)
            _logger.LogDebug("MQTT client connected successfully to {Host}:{Port} with Client Id: {ClientId}", _mqttOptions.Host, _mqttOptions.Port, _mqttOptions.Consumer.ClientId);
        else
            _logger.LogError("Failed to connect MQTT client to {Host}:{Port}. Result code: {ResultCode}. Reasoning: {ReasonString}", _mqttOptions.Host, _mqttOptions.Port, resp.ResultCode, resp.ReasonString);

        return mqttClient;
    }

    public async Task DisconnectAsync(IMqttClient mqttClient)
    {
        ArgumentNullException.ThrowIfNull(mqttClient);

        try
        {
            await mqttClient.DisconnectAsync();
            _logger.LogDebug("MQTT client disconnected successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to disconnect MQTT client");
            throw;
        }
    }

    public async Task SubscribeTopicAsync(
        IMqttClient mqttClient,
        MqttQualityOfServiceLevel qosLevel = MqttQualityOfServiceLevel.AtLeastOnce)
    {
        ArgumentNullException.ThrowIfNull(mqttClient);
        ArgumentException.ThrowIfNullOrEmpty(_mqttOptions.Consumer.Topic, "Topic is not configured");

        try
        {
            var subscribeOptions = new MqttClientSubscribeOptionsBuilder()
                .WithTopicFilter(filter =>
                {
                    filter.WithTopic(_mqttOptions.Consumer.Topic);
                    filter.WithQualityOfServiceLevel(qosLevel);
                })
                .Build();

            await mqttClient.SubscribeAsync(subscribeOptions);
            _logger.LogDebug("Subscribed to MQTT Shared Topic: {Topic} with ClientId: {ClientId}", _mqttOptions.Consumer.Topic, _mqttOptions.Consumer.ClientId);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to subscribe to shared topic {Topic} with Client Id {ClientId}", _mqttOptions.Consumer.Topic, _mqttOptions.Consumer.ClientId);
            throw;
        }
    }
}
