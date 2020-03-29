using System;
using ServiceLayerApi.MQTT.Client.RPC;

namespace ServiceLayerApi.CommandProcessing.Models
{
    public class RpcCommandResponse : IRpcCommand
    {
        public Guid CommandId { get; set; }
        public bool IsFailed { get; set; }
        public string FailedMessage { get; set; }
    }
}