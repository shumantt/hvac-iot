using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ServiceLayerApi.DataProcessing.Messages;
using ServiceLayerApi.DeviceNetwork;
using ServiceLayerApi.DeviceNetwork.Description;
using ServiceLayerApi.DeviceNetwork.Sensors;
using ServiceLayerApi.MQTT.Client;

namespace ServiceLayerApi.DataProcessing
{
    public class SensorProcessingService : BaseProcessingService<SensorValues>
    {
        private readonly IParameterAggregator[] _parameterAggregators;
        private readonly DeviceRepository _deviceRepository;
        private ConcurrentQueue<SensorResult> _sensorResults = new ConcurrentQueue<SensorResult>();
        private readonly Timer _timer;
        private const int maxValuesToProcess = 100;
        private string microClimateParametersTopic = "data/microclimate";

        public SensorProcessingService(MqttClientRepository mqttClientRepository,
            IEnumerable<IParameterAggregator> parameterAggregators,
            DeviceRepository deviceRepository,
            IConfiguration configuration,
            ILogger<BaseProcessingService<SensorValues>> logger) : base(mqttClientRepository, logger)
        {
            var timerPeriod = int.Parse(configuration["SensorsAggregationTime"]);
            _parameterAggregators = parameterAggregators.ToArray();
            _deviceRepository = deviceRepository;
            _timer = new Timer(timerPeriod) { AutoReset = true };
            _timer.Elapsed += (_, __) => AggregateResults();
            _timer.Enabled = true;
        }

        protected override string Topic => "data/sensors";

        protected override Task Process(SensorValues message)
        {
            var sensor = _deviceRepository.GetSensor(message.DeviceId);
            if(sensor == null)
                return Task.CompletedTask;
            var sensorResult = sensor.NormalizeValue(message);
            _sensorResults.Enqueue(sensorResult);
            return Task.CompletedTask;
        }

        protected override Task OnStart() => Task.CompletedTask;
        protected override Task OnStop()
        {
            _timer.Enabled = false;
            return Task.CompletedTask;
        }

        private Task AggregateResults()
        {
            var sensorResults = GetSensorResults().Take(maxValuesToProcess).ToArray();

            var publishTasks = sensorResults.GroupBy(x => x.Parameter)
                .Select(Aggregate)
                .Select(x => MqttClient.PublishAsync(microClimateParametersTopic, x));

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