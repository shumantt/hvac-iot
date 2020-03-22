using System;
using ServiceLayerApi.DeviceNetwork.Description;

namespace ServiceLayerApi.DataProcessing.Messages
{
    public class SensorValues
    {
        public string RawValue { get; set; }
        public Guid DeviceId { get; set; }
    }
}