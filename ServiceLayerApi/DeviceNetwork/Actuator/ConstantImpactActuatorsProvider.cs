using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using ServiceLayerApi.CommandProcessing.Models;
using ServiceLayerApi.DeviceNetwork.Description;
using ServiceLayerApi.DeviceNetwork.Messages;

namespace ServiceLayerApi.DeviceNetwork.Actuator
{
    public class ConstantImpactActuatorsProvider
    {
        public static readonly Guid RadiatorId = Guid.NewGuid();
        private readonly bool _addRadiators;

        public ConstantImpactActuatorsProvider(IConfiguration configuration)
        {
            _addRadiators = bool.Parse(configuration["AddRadiators"]);
        }
        
        public IEnumerable<IActuator> ProvideConstantImpactActuators()
        {
            if (_addRadiators)
            {
                //батареи
                yield return new ConstantActuator(ParameterType.TemperatureInside, CommandImpact.Increase,"radiators", RadiatorId);
            }
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