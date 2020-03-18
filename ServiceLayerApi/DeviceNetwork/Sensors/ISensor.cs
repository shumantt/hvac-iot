using System;
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
        public SensorResult(object value, ParameterType parameter, Guid deviceId)
        {
            RawValue = value;
            Parameter = parameter;
            DeviceId = deviceId;
        }

        protected object RawValue { get; }
        public ParameterType Parameter { get; }
        public Guid DeviceId { get; }

        public T GetValue<T>() => (T) RawValue;
    }
}