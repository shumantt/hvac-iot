using System;
using System.Linq;
using System.Threading.Tasks;
using ServiceLayerApi.CommandProcessing.Models;
using ServiceLayerApi.Common;
using ServiceLayerApi.DeviceNetwork;
using ServiceLayerApi.DeviceNetwork.Actuator;
using ServiceLayerApi.DeviceNetwork.Description;

namespace ServiceLayerApi.CommandProcessing
{
    public class CommandProcessingService
    {
        private readonly DeviceRepository _deviceRepository;

        public CommandProcessingService(DeviceRepository deviceRepository)
        {
            _deviceRepository = deviceRepository;
        }
        
        public async Task<DecisionCommandProcessResult> ProcessDecisionCommand(DecisionCommand decisionCommand)
        {
            if (decisionCommand.ParameterCommands.GroupBy(c => c.Parameter)
                .Any(x => x.Count() > 1))
            {
                throw new InvalidOperationException($"Can't process command with duplicated parameter commands: {decisionCommand.ToJson()}");
            }

            var commandTasks = decisionCommand.ParameterCommands.Select(ProcessParameterCommand).ToArray();
            var parameterCommandResults = await Task.WhenAll(commandTasks).ConfigureAwait(false);
            return new DecisionCommandProcessResult
            {
                ParameterCommandProcessResults = parameterCommandResults
            };
        }

        private async Task<ParameterCommandProcessResult> ProcessParameterCommand(ParameterCommand parameterCommand)
        {
            var actuators = _deviceRepository.GetActuatorsByParameter(parameterCommand.Parameter);
            var constantImpactActuators = actuators.Where(x => x.ActuatorDeviceInfo.IsConstantImpact).ToArray();
            var constantCommandResults = constantImpactActuators.Select(ActuatorCommandProcessResult.CreateForConstantActuator);
            if (parameterCommand.CommandImpact == CommandImpact.NoChange)
            {
                if (constantImpactActuators.Length > 0)
                {
                    return CombineProcessingResult(constantCommandResults.ToArray());
                }
                
                return new ParameterCommandProcessResult { Impact = (double)CommandImpact.NoChange };
            }

            var selectedActuators = actuators.Where(x => IsActuatorApplicable(x, parameterCommand.CommandImpact));
            var actuatorsTasks = selectedActuators.Select(x => x.Act(parameterCommand));
            var results = await Task.WhenAll(actuatorsTasks.ToArray()).ConfigureAwait(false);
            return CombineProcessingResult(results.Concat(constantCommandResults).ToArray());
        }
        
        private ParameterCommandProcessResult CombineProcessingResult(ActuatorCommandProcessResult[] commandProcessingResults)
        {
            var impact = commandProcessingResults
                .Where(x => !x.Failed)
                .Select(x => (double) x.ExecutedCommand.CommandImpact)
                .ToArray()
                .Mean();
            var failedMessages = commandProcessingResults.Where(x => x.Failed)
                .Select(x => x.Error).ToArray();
            return new ParameterCommandProcessResult()
            {
                Error = string.Join(";", failedMessages),
                Failed = failedMessages.Length == commandProcessingResults.Length,
                Impact = impact
            };
        }

        private bool IsActuatorApplicable(IActuator actuator, CommandImpact requestedImpact)
        {
            return !actuator.ActuatorDeviceInfo.IsConstantImpact &&
                   actuator.ActuatorDeviceInfo.Impacts.Any(x => x.IsLessOrSameFromSameDirection(requestedImpact));
        }
    }
}