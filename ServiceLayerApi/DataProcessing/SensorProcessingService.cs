using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MQTTnet.Extensions.ManagedClient;
using ServiceLayerApi.Common;
using ServiceLayerApi.DataProcessing.Messages;
using ServiceLayerApi.DeviceNetwork.Description;
using ServiceLayerApi.DeviceNetwork.Sensors;
using ServiceLayerApi.MQTT;
using ServiceLayerApi.MQTT.Client;

namespace ServiceLayerApi.DataProcessing
{
    public class SensorProcessingService : BaseProcessingService<SensorValues>
    {
        private readonly MqttClientRepository _mqttClientRepository;
        private readonly IParameterAggregator[] _parameterAggregators;
        private readonly ConcurrentDictionary<Guid, ISensor> _sensors = new ConcurrentDictionary<Guid, ISensor>();
        private readonly ConcurrentQueue<SensorResult> _sensorResults = new ConcurrentQueue<SensorResult>();
        private const int maxValuesToProcess = 100;
        private string microclimateParametersTopic = "data/microclimate";
        
        public SensorProcessingService(MqttClientRepository mqttClientRepository,
            IParameterAggregator[] parameterAggregators) : base(mqttClientRepository)
        {
            _mqttClientRepository = mqttClientRepository;
            _parameterAggregators = parameterAggregators;
        }

        protected override string Topic => "data/sensors";

        protected override async Task Process(Guid deviceId, SensorValues message)
        {
            if(!_sensors.TryGetValue(deviceId, out var sensor))
                return;
            var sensorResult = sensor.NormalizeValue(message);
            _sensorResults.Enqueue(sensorResult);
        }

        protected override Task OnStart() => Task.CompletedTask;

        private Task AggregateResults()
        {
            var sensorResults = GetSensorResults().Take(maxValuesToProcess).ToArray();

            var publishTasks = sensorResults.GroupBy(x => x.Parameter)
                .Select(Aggregate)
                .Select(x => MqttClient.PublishAsync(microclimateParametersTopic, x));

            return Task.WhenAll(publishTasks);
            
            MicroClimateParameterValue Aggregate(IGrouping<ParameterType, SensorResult> parameterValues)
            {
                var aggregator = _parameterAggregators
                    .Where(x => x.CanAggregate(parameterValues.Key))
                    .OrderBy(x => x.Order)
                    .FirstOrDefault();

                if (aggregator == null)
                    throw new InvalidOperationException($"Can't aggregate parameter: {parameterValues.Key}");

                return aggregator.Aggregate(parameterValues.ToArray(), parameterValues.Key);
            }
        }

        private IEnumerable<SensorResult> GetSensorResults()
        {
            while (!_sensorResults.IsEmpty)
            {
                if(!_sensorResults.TryDequeue(out var result))
                    yield break;
                yield return result;
            }
        }
    }
}