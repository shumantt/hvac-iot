using System;

namespace ServiceLayerApi.MQTT.Client.RPC
{
    public interface IRpcCommand
    {
        Guid CommandId { get; }
    }
}