using System;
using ServiceLayerApi.DataProcessing.Messages;
using ServiceLayerApi.DeviceNetwork.Description;
using ServiceLayerApi.DeviceNetwork.Description.DeviceBuilders;
using ServiceLayerApi.DeviceNetwork.Messages;

namespace ServiceLayerApi.DeviceNetwork.Sensors
{
    public class CustomTemperatureSensor : ISensor
    {
        public CustomTemperatureSensor(Guid id, bool inside)
        {
            Id = id;
            Parameter = inside ? ParameterType.TemperatureInside : ParameterType.TemperatureOutside;
        }

        public Guid Id { get; }
        public ParameterType Parameter { get; }


        public SensorResult NormalizeValue(SensorValues sensorValues)
        {
            var temperatureCelsius = double.Parse(sensorValues.RawValue);
            return new SensorResult(temperatureCelsius, Parameter, Id);
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