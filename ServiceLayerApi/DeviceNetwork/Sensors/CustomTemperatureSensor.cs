using System;
using System.Globalization;
using ServiceLayerApi.DataProcessing.Messages;
using ServiceLayerApi.DeviceNetwork.Description;
using ServiceLayerApi.DeviceNetwork.Messages;

namespace ServiceLayerApi.DeviceNetwork.Sensors
{
    public class CustomTemperatureSensor : ISensor
    {
        public CustomTemperatureSensor(DeviceInfo deviceInfo)
        {
            DeviceInfo = deviceInfo;
        }

        public DeviceInfo DeviceInfo { get; }


        public SensorResult NormalizeValue(SensorValues sensorValues)
        {
            if (!double.TryParse(sensorValues.RawValue, NumberStyles.Any, CultureInfo.InvariantCulture, out var temp))
            {
                throw new InvalidOperationException($"Can't parse double value '{sensorValues.RawValue}' for CustomTemperatureSensor");
            }
            
            return new SensorResult
            {
                RawValue = sensorValues.RawValue,
                Parameter = DeviceInfo.Parameter,
                DeviceId = DeviceInfo.Id
            };
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
            return new CustomTemperatureSensor(deviceInfo);
        }
    }
}