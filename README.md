# CodeChavez.EventBus.MQTT

A comprehensive .NET 10 Event Bus library for MQTT messaging that provides simplified connection management, subscription handling, and event publishing capabilities using MQTTnet.

## Overview

`CodeChavez.EventBus.MQTT` is a modular event bus library that simplifies MQTT client operations with built-in logging and configuration management. It provides an abstraction layer over MQTTnet, making it easy to integrate MQTT messaging patterns (pub/sub, event bus) into your .NET applications.

### Packages

This project provides two NuGet packages:

- **CodeChavez.EventBus.MQTT.Abstractions** (v1.0.0-preview004) - Interfaces and abstractions for MQTT implementations
- **CodeChavez.EventBus.MQTT** (v1.0.0-preview004) - Full implementation of the MQTT event bus

## Features

- **Simple Connection Management** - Easily connect to MQTT brokers with configurable options
- **Subscription Management** - Subscribe to MQTT topics with event handling
- **Event Publishing** - Publish messages to MQTT topics through the event bus pattern
- **Logging Support** - Built-in logging for debugging and monitoring
- **Configuration Options** - Flexible configuration through dependency injection
- **Async/Await Support** - Fully asynchronous operations
- **Nullable Reference Types** - Full nullable reference type support for safer code
- **Implicit Usings** - Streamlined using statements for .NET 10

## Installation

Add the `CodeChavez.EventBus.MQTT` NuGet package to your project:

```bash
dotnet add package CodeChavez.EventBus.MQTT
```

Or, if you only need the abstractions:

```bash
dotnet add package CodeChavez.EventBus.MQTT.Abstractions
```

## Quick Start

### Configuration

First, configure your MQTT options in your dependency injection container:

```csharp
services.Configure<ConsumerMqttOptions>(configuration.GetSection("MqttConfig"));
services.AddScoped<IEventBusMqttClient, EventBusMqttClient>();
```

### Usage Example

```csharp
public class MqttService
{
    private readonly IEventBusMqttClient _mqttClient;
    private IMqttClient _client;

    public MqttService(IEventBusMqttClient mqttClient)
    {
        _mqttClient = mqttClient;
    }

    public async Task ConnectAsync()
    {
        // Connect to MQTT broker
        _client = await _mqttClient.ConnectAsync("my-client-id");
    }

    public async Task SubscribeAsync(string topic)
    {
        // Subscribe to a topic
        await _mqttClient.SubscribeAsync(topic);
    }

    public async Task DisconnectAsync()
    {
        // Disconnect from MQTT broker
        if (_client != null)
        {
            await _mqttClient.DisconnectAsync(_client);
        }
    }
}
```

## Configuration Example

Add the following to your `appsettings.json`:

```json
{
  "MqttConfig": {
    "Host": "mqtt.example.com",
    "Port": 1883,
    "Consumer": {
      "ClientId": "my-mqtt-client",
      "KeepAlivePeriod": 60,
      "CleanSession": true
    }
  }
}
```

## Requirements

- .NET 10 or higher
- MQTTnet 5.1.0.1559 or higher

## License

MIT