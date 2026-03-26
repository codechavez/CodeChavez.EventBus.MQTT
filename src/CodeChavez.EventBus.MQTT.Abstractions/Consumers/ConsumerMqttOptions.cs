namespace CodeChavez.EventBus.MQTT.Abstractions.Consumers;

public record ConsumerMqttOptions
{
    public string ClientId { get; set; } = Guid.NewGuid().ToString();
    public string ConsumerGroup { get; set; } = string.Empty;
    public string Topic { get; set; } = string.Empty;
    public ushort KeepAlinePeriod { get; set; } = 60;
    public ushort MaxConcurrency { get; set; } = 4;
    public bool CleanSession { get; set; } = true;
}
