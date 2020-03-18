using System.Globalization;
using System.Linq;
using ServiceLayerApi.Common;
using ServiceLayerApi.DataProcessing.Messages;
using ServiceLayerApi.DeviceNetwork.Description;
using ServiceLayerApi.DeviceNetwork.Sensors;

namespace ServiceLayerApi.DataProcessing
{
    public interface IParameterAggregator
    {
        bool CanAggregate(ParameterType parameterType);
        public int Order { get; }
        MicroClimateParameterValue Aggregate(SensorResult[] sensorResults, ParameterType parameterType);
    }

    public class MeanTemperatureAggregator : IParameterAggregator
    {
        public bool CanAggregate(ParameterType parameterType) => parameterType.IsTemperature();
        public int Order => int.MaxValue;

        public MicroClimateParameterValue Aggregate(SensorResult[] sensorResults, ParameterType parameterType)
        {
            var mean = sensorResults.Sum(x => x.GetValue<double>()) / sensorResults.Length;
            return new MicroClimateParameterValue
            {
                ParameterType = parameterType,
                Value = mean.ToString(CultureInfo.InvariantCulture)
            };
        }
    }
}