using System;
using ServiceLayerApi.DataProcessing.Messages;
using ServiceLayerApi.DeviceNetwork.Description;
using ServiceLayerApi.DeviceNetwork.Description.DeviceBuilders;
using ServiceLayerApi.DeviceNetwork.Messages;

namespace ServiceLayerApi.DeviceNetwork.Sensors
{
    public class CustomTemperatureSensor : ISensor<double>
    {
        public CustomTemperatureSensor(Guid id, bool inside)
        {
            Id = id;
            Parameter = inside ? ParameterType.TemperatureInside : ParameterType.TemperatureOutside;
        }

        public Guid Id { get; }
        public ParameterType Parameter { get; }


        public SensorResult<double> ProcessMessage(SensorValues sensorValues)
        {
            var temperatureCelsius = double.Parse(sensorValues.RawValue);
            return new SensorResult<double>(temperatureCelsius, Parameter);
        }
    }
    
    public class CustomTemperatureSensorBuilder : IDeviceBuilder
    {
        public bool CanBuild(DeviceInfo deviceInfo)
        {
            return deviceInfo.Type == DeviceType.Sensor 
                   && deviceInfo.Parameter.IsTemperature()
                   && deviceInfo.DeviceCode == "CustomTemp";
        }

        public IDevice Build(DeviceInfo deviceInfo)
        {
            return new CustomTemperatureSensor(deviceInfo.Id, deviceInfo.Parameter == ParameterType.TemperatureInside);
        }
    }
}