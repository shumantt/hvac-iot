using System;
using System.Text.Json.Serialization;
using ServiceLayerApi.DeviceNetwork.Description;
using ServiceLayerApi.MQTT.Client.RPC;

namespace ServiceLayerApi.CommandProcessing.Models
{
    public class DecisionCommand
    {
        public ParameterCommand[] ParameterCommands { get; set; } 
    }

    public class ParameterCommand
    {
        public ParameterType Parameter { get; set; }
        public CommandImpact CommandImpact { get; set; }
    }
}