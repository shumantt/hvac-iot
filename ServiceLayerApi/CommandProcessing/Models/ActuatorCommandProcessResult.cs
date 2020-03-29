using ServiceLayerApi.DeviceNetwork.Actuator;
using ServiceLayerApi.DeviceNetwork.Description;

namespace ServiceLayerApi.CommandProcessing.Models
{
    public class ActuatorCommandProcessResult
    {
        public ParameterCommand ExecutedCommand { get; set; }
        public bool Failed { get; set; }
        public string Error { get; set; }

        public static ActuatorCommandProcessResult Create(ParameterType parameter, CommandImpact impact) =>
            new ActuatorCommandProcessResult
            {
                ExecutedCommand = new ParameterCommand
                {
                    Parameter = parameter,
                    CommandImpact = impact
                }
            };
        
        public static ActuatorCommandProcessResult CreateFullSuccess(ParameterCommand sentCommand) =>
            new ActuatorCommandProcessResult
            {
                ExecutedCommand = sentCommand,
            };

        public static ActuatorCommandProcessResult CreateNotChanged(ParameterType parameter) =>
            new ActuatorCommandProcessResult
            {
                ExecutedCommand = new ParameterCommand
                {
                    CommandImpact = CommandImpact.NoChange,
                    Parameter = parameter
                }
            };

        public static ActuatorCommandProcessResult CreateFailed(string error)
        {
            return new ActuatorCommandProcessResult
            {
                Failed = true,
                Error = error
            };
        }

        public static ActuatorCommandProcessResult CreateForConstantActuator(IActuator constantActuator)
        {
            return Create(constantActuator.ActuatorDeviceInfo.Parameter, constantActuator.ActuatorDeviceInfo.ConstantImpactValue);
        }
    }
}