using CodeChavez.EventBus.MQTT.Abstractions;
using CodeChavez.EventBus.MQTT.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CodeChavez.EventBus.MQTT;

public record ConsumerMqttConfig : MqttOptions
{
    public const string ConsumerSection = "MqttConsumer";
    public ConsumerMqttOptions Consumer { get; set; } = new();
}

public static class EventBusMQTTModule
{
    public static IServiceCollection AddConsumerMQTT(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<ConsumerMqttConfig>(configuration.GetSection(ConsumerMqttConfig.ConsumerSection));
        services.AddSingleton<IEventBusMqttClient, EventBusMqttClient>();

        return services;
    }

    public static IServiceCollection AddProducerMQTT(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        return services;
    }
}
