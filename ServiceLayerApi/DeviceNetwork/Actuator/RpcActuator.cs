using System;
using System.Linq;
using System.Threading.Tasks;
using ServiceLayerApi.CommandProcessing.Models;
using ServiceLayerApi.DeviceNetwork.Description;
using ServiceLayerApi.DeviceNetwork.Messages;
using ServiceLayerApi.DeviceNetwork.Sensors;
using ServiceLayerApi.MQTT.Client.RPC;

namespace ServiceLayerApi.DeviceNetwork.Actuator
{
    public class RpcActuator : IActuator
    {
        private readonly RpcMqttClient _rpcMqttClient;

        public RpcActuator(ActuatorDeviceInfo actuatorDeviceInfo, RpcMqttClient rpcMqttClient)
        {
            _rpcMqttClient = rpcMqttClient;
            ActuatorDeviceInfo = actuatorDeviceInfo;
        }

        public DeviceInfo DeviceInfo => ActuatorDeviceInfo;
        public ActuatorDeviceInfo ActuatorDeviceInfo { get; }
        public virtual async Task<ActuatorCommandProcessResult> Act(ParameterCommand command)
        {
            var commandImpacts = ActuatorDeviceInfo
                .Impacts
                .Where(x => x.IsLessOrSameFromSameDirection(command.CommandImpact))
                .ToArray();
            if (!commandImpacts.Any())
            {
                throw new InvalidOperationException($"Can't process command with impact {command.CommandImpact}");
            }
            
            var selectedImpact = (CommandImpact)commandImpacts.Min(x => Math.Abs(Math.Abs((int)x) - Math.Abs((int)command.CommandImpact)));
            var commandToExecute = new RpcCommandRequest()
            {
                CommandId = Guid.NewGuid(),
                Impact = selectedImpact,
                Parameter = command.Parameter,
                DeviceId = ActuatorDeviceInfo.Id
            };

            var requestResult =
                await _rpcMqttClient.ProcessRpcRequest<RpcCommandResponse, RpcCommandRequest>(
                    "command/response",
                    "command/request",
                    commandToExecute).ConfigureAwait(false);

            return new ActuatorCommandProcessResult()
            {
                Error = requestResult.FailedMessage,
                Failed = requestResult.IsFailed,
                ExecutedCommand = new ParameterCommand()
                {
                    CommandImpact = commandToExecute.Impact,
                    Parameter = command.Parameter
                }
            };
        }
    }

    public class RpcActuatorBuilder : IDeviceBuilder
    {
        private readonly RpcMqttClient _rpcMqttClient;

        public RpcActuatorBuilder(RpcMqttClient rpcMqttClient)
        {
            _rpcMqttClient = rpcMqttClient;
        }
        
        public bool CanBuild(DeviceInfo deviceInfo)
        {
            return deviceInfo.Type == DeviceType.Actuator
                   && deviceInfo.DeviceCode == "Custom";
        }

        public IDevice Build(DeviceInfo deviceInfo)
        {
            var actuatorInfo = (ActuatorDeviceInfo) deviceInfo;
            return new RpcActuator(actuatorInfo, _rpcMqttClient);
        }
    }
}