using System;
using System.Collections.Concurrent;
using ServiceLayerApi.DeviceNetwork.Description;
using ServiceLayerApi.DeviceNetwork.Sensors;

namespace ServiceLayerApi.DeviceNetwork
{
    public class DeviceRepository
    {
        private readonly ConcurrentDictionary<Guid, ISensor> _sensors = new ConcurrentDictionary<Guid, ISensor>();

        public void StoreDevice(IDevice device)
        {
            switch (device)
            {
                case ISensor sensor:
                    _sensors.TryAdd(sensor.Id, sensor);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(device));
            }
        }

        public ISensor GetSensor(Guid sensorId)
        {
            if (!_sensors.TryGetValue(sensorId, out var sensor))
                return null;
            return sensor;
        }
    }
}