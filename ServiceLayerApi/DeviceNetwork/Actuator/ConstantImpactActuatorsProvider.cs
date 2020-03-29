using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ServiceLayerApi.CommandProcessing.Models;
using ServiceLayerApi.DeviceNetwork.Description;
using ServiceLayerApi.DeviceNetwork.Messages;

namespace ServiceLayerApi.DeviceNetwork.Actuator
{
    public class ConstantImpactActuatorsProvider
    {
        public IEnumerable<IActuator> ProvideConstantImpactActuators()
        {
            //батареи
            yield return new ConstantActuator(ParameterType.TemperatureInside, CommandImpact.Increase,"radiators", Guid.NewGuid());
        }
    }

    public class ConstantActuator : IActuator
    {
        public ConstantActuator(ParameterType parameter, CommandImpact impact, string deviceCode, Guid deviceId)
        {
            ActuatorDeviceInfo = new ActuatorDeviceInfo()
            {
                Impacts = new [] { impact },
                IsConstantImpact = true,
                DeviceCode = deviceCode,
                Id = deviceId,
                Parameter = parameter,
                Type = DeviceType.Actuator
            };
        }

        public DeviceInfo DeviceInfo => ActuatorDeviceInfo;
        public ActuatorDeviceInfo ActuatorDeviceInfo { get; }
        public Task<ActuatorCommandProcessResult> Act(ParameterCommand command)
        {
            throw new System.NotImplementedException();
        }
    }
}