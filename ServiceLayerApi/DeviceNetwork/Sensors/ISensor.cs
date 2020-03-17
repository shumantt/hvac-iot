using ServiceLayerApi.DataProcessing.Messages;
using ServiceLayerApi.DeviceNetwork.Description;
using ServiceLayerApi.DeviceNetwork.Messages;

namespace ServiceLayerApi.DeviceNetwork.Sensors
{
    public interface ISensor<TValue> : IDevice
    {
        SensorResult<TValue> ProcessMessage(SensorValues sensorValues);
    }

    public class SensorResult<TValue>
    {
        public SensorResult(TValue value, ParameterType parameter)
        {
            Value = value;
            Parameter = parameter;
        }

        public TValue Value { get; }
        public ParameterType Parameter { get; }
    }
}