using System;
using System.Globalization;
using ServiceLayerApi.DataProcessing.Messages;
using ServiceLayerApi.DeviceNetwork.Description;
using ServiceLayerApi.DeviceNetwork.Messages;

namespace ServiceLayerApi.DeviceNetwork.Sensors
{
    public interface ISensor : IDevice
    {
        SensorResult NormalizeValue(SensorValues sensorValues);
    }

    public class SensorResult
    {
        public string RawValue { get; set; }
        public ParameterType Parameter { get; set; }
        public Guid DeviceId { get; set; }

        public double GetDoubleValue() => Double.Parse(RawValue, NumberStyles.Any, CultureInfo.InvariantCulture);
    }
}