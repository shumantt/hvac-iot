using System;
using ServiceLayerApi.DeviceNetwork.Description;

namespace ServiceLayerApi.DataProcessing.Messages
{
    public class SensorValues
    {
        public Guid DeviceId { get; set; }
        public string DeviceCode { get; set; }
        public ParameterType Parameter { get; set; }
        public string RawValue { get; set; }
    }
}