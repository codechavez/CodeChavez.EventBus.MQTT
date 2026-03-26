using System;
using System.Collections.Generic;
using System.Text;

namespace CodeChavez.EventBus.MQTT.Abstractions.Interfaces;

public interface IMqttSubscription
{
    event EventHandler<MqttMessageReceivedEventArgs>? MessageReceived;
    bool IsConnected { get; }
    Task ConnectAsync(CancellationToken cancellation = default);
    Task DisconnectAsync(CancellationToken cancellationToken = default);
    ValueTask DisposeAsync();
}
