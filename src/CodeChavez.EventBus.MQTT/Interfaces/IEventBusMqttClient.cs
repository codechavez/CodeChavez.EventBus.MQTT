using MQTTnet;

namespace CodeChavez.EventBus.MQTT.Interfaces;

public interface IEventBusMqttClient
{
    Task<IMqttClient> ConnectAsync(string? clientId = null);
    Task DisconnectAsync(IMqttClient mqttClient);
}
