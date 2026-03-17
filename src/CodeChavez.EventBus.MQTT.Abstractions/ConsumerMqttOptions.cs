namespace CodeChavez.EventBus.MQTT.Abstractions;

public record ConsumerMqttOptions
{
    public string ClientId { get; set; } = Guid.NewGuid().ToString();
    public string Topic { get; set; } = string.Empty;
    public ushort KeepAlinePeriod { get; set; } = 60;
    public ushort MaxParallelism { get; set; } = 4;
    public bool CleanSession { get; set; } = true;
}


