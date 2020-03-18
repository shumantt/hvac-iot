using ServiceLayerApi.DeviceNetwork.Description;

namespace ServiceLayerApi.DataProcessing.Messages
{
    public class MicroClimateParameterValue
    {
        public ParameterType ParameterType { get; set; }
        public string Value { get; set; }
    }
}