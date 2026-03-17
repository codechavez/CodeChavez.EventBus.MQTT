# CodeChavez.EventBus.MQTT

A lightweight .NET 10 MQTT client wrapper that provides simplified connection and disconnection management for MQTT brokers using MQTTnet.

## Overview

`CodeChavez.EventBus.MQTT` is a utility library that simplifies MQTT client operations with built-in logging and configuration management. It abstracts away common MQTT connection patterns, making it easy to integrate MQTT messaging into your .NET applications.

## Features

- **Simple Connection Management** - Easily connect to MQTT brokers with configurable options
- **Logging Support** - Built-in logging for debugging and monitoring
- **Configuration Options** - Flexible configuration through dependency injection
- **Async/Await Support** - Fully asynchronous operations

## Installation

Add the `CodeChavez.EventBus.MQTT` NuGet package to your project:

```bash
dotnet add package CodeChavez.EventBus.MQTT
```

## Quick Start

### Configuration

First, configure your MQTT options in your dependency injection container:

```csharp
services.Configure<ConsumerMqttConfig>(configuration.GetSection("MqttConfig"));
services.AddScoped<EventBusMqttClient>();
```

### Usage Example

```csharp
public class MqttService
{
    private readonly EventBusMqttClient _mqttClient;
    private IMqttClient _client;

    public MqttService(EventBusMqttClient mqttClient)
    {
        _mqttClient = mqttClient;
    }

    public async Task ConnectAsync()
    {
        // Connect to MQTT broker
        _client = await _mqttClient.ConnectAsync("my-client-id");
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
      "KeepAlinePeriod": 60,
      "CleanSession": true
    }
  }
}
```

## License

MIT