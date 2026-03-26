namespace CodeChavez.EventBus.MQTT.Abstractions.Consumers;

public record ConsumerOptions : MqttOptions
{
    public const string ConsumerSection = "MqttConsumer";
    public ConsumerMqttOptions Consumer { get; set; } = new();
}
