using MQTTnet;
using MQTTnet.Protocol;

namespace CodeChavez.EventBus.MQTT.Interfaces;

public interface IEventBusMqttClient
{
    Task<IMqttClient> ConnectAsync(string? clientId = null);
    Task DisconnectAsync(IMqttClient mqttClient);
    Task SubscribeTopicAsync(
        IMqttClient mqttClient,
        MqttQualityOfServiceLevel qosLevel = MqttQualityOfServiceLevel.AtLeastOnce);
}
