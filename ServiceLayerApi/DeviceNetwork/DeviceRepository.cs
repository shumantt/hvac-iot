using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using ServiceLayerApi.DeviceNetwork.Actuator;
using ServiceLayerApi.DeviceNetwork.Description;
using ServiceLayerApi.DeviceNetwork.Sensors;

namespace ServiceLayerApi.DeviceNetwork
{
    public class DeviceRepository
    {
        private readonly ConcurrentDictionary<Guid, ISensor> _sensors = new ConcurrentDictionary<Guid, ISensor>();
        private readonly ConcurrentDictionary<ParameterType, List<IActuator>> _actuators = new ConcurrentDictionary<ParameterType, List<IActuator>>();

        public void StoreDevice(IDevice device)
        {
            switch (device)
            {
                case IActuator actuator:
                    StoreActuator(actuator);
                    break;
                case ISensor sensor:
                    _sensors.TryAdd(sensor.DeviceInfo.Id, sensor);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(device));
            }

            void StoreActuator(IActuator actuator)
            {
                var parameter = actuator.DeviceInfo.Parameter;
                List<IActuator> actuatorsByType;
                if(!_actuators.TryGetValue(parameter, out actuatorsByType))
                {
                    actuatorsByType = new List<IActuator>();
                    _actuators.AddOrUpdate(parameter, actuatorsByType, (type, list) => (actuatorsByType = list));
                }
                actuatorsByType.Add(actuator);
            }
        }

        public ISensor GetSensor(Guid sensorId)
        {
            if (!_sensors.TryGetValue(sensorId, out var sensor))
                return null;
            return sensor;
        }

        public IActuator[] GetActuatorsByParameter(ParameterType parameter)
        {
            if (!_actuators.TryGetValue(parameter, out var actuators))
            {
                return new IActuator[0];
            }

            return actuators.ToArray();
        }
    }
}