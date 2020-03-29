using System.Threading.Tasks;
using ServiceLayerApi.CommandProcessing.Models;
using ServiceLayerApi.DeviceNetwork.Description;
using ServiceLayerApi.DeviceNetwork.Messages;

namespace ServiceLayerApi.DeviceNetwork.Actuator
{
    public interface IActuator : IDevice
    {
        ActuatorDeviceInfo ActuatorDeviceInfo { get; }
        Task<ActuatorCommandProcessResult> Act(ParameterCommand command);
    }
}