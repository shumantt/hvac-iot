using System;
using System.Linq;
using System.Text.Json.Serialization;
using ServiceLayerApi.DeviceNetwork.Description;

namespace ServiceLayerApi.DeviceNetwork.Messages
{
    public class DeviceInfo
    {
        public Guid Id { get; set; }
        public string DeviceCode { get; set; }
        public ParameterType Parameter { get; set; }
        public DeviceType Type { get; set; }
    }

    public class ActuatorDeviceInfo : DeviceInfo
    {
        public ActuatorDeviceInfo()
        {
            Type = DeviceType.Actuator;
        }
        public bool IsConstantImpact { get; set; }
        public CommandImpact[] Impacts { get; set; }
        [JsonIgnore]
        public CommandImpact ConstantImpactValue => Impacts.Single();
        [JsonIgnore]
        public bool IsConstantImpactful => IsConstantImpact && Impacts.Single() != CommandImpact.NoChange;
    }
}