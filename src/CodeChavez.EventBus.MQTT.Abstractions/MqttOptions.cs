namespace CodeChavez.EventBus.MQTT.Abstractions;

public record MqttOptions
{
    public string Host { get; set; } = string.Empty;
    public ushort Port { get; set; } = 1883;
}
