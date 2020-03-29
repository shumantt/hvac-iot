using System;
using System.Linq;
using ServiceLayerApi.DeviceNetwork.Description;

namespace ServiceLayerApi.DeviceNetwork.Messages
{
    public class DeviceInfo
    {
        public Guid Id { get; set; }
        public string DeviceCode { get; set; }
        public string AsyncMessageQueue { get; set; }
        public string SyncAddress { get; set; }
        public ParameterType Parameter { get; set; }
        public DeviceType Type { get; set; }
    }

    public class ActuatorDeviceInfo : DeviceInfo
    {
        public bool IsConstantImpact { get; set; }
        public CommandImpact[] Impacts { get; set; }
        public CommandImpact ConstantImpactValue => Impacts.Single();
        public bool IsConstantImpactful => IsConstantImpact && Impacts.Single() != CommandImpact.NoChange;
    }
}