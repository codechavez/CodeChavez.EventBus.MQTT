namespace CodeChavez.EventBus.MQTT.Abstractions;

public class MqttMessageReceivedEventArgs(string topic, string payload) : EventArgs
{
    public string Topic { get; } = topic ?? throw new ArgumentNullException(nameof(topic));
    public string Payload { get; } = payload ?? throw new ArgumentNullException(nameof(payload));
}