using System;
using ServiceLayerApi.DeviceNetwork.Description;
using ServiceLayerApi.MQTT.Client.RPC;

namespace ServiceLayerApi.CommandProcessing.Models
{
    public class RpcCommandRequest : IRpcCommand
    {
        public Guid DeviceId { get; set; }
        public Guid CommandId { get; set; }
        public ParameterType Parameter { get; set; }
        public CommandImpact Impact { get; set; }
    }
}