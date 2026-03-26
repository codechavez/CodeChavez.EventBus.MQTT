using CodeChavez.EventBus.MQTT.Abstractions.Consumers;
using CodeChavez.EventBus.MQTT.Abstractions.Interfaces;
using CodeChavez.EventBus.MQTT.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CodeChavez.EventBus.MQTT;

public static class EventBusMQTTModule
{
    public static IServiceCollection AddMQTTSubscription(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        services.Configure<ConsumerOptions>(configuration.GetSection(ConsumerOptions.ConsumerSection));
        services.AddSingleton<IMqttSubscription, MqttSubscription>();

        return services;
    }

    public static IServiceCollection AddConsumerMQTT(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<ConsumerOptions>(configuration.GetSection(ConsumerOptions.ConsumerSection));
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
