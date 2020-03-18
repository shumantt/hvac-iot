using System;
using System.Threading.Tasks;
using ServiceLayerApi.DeviceNetwork.Messages;
using ServiceLayerApi.MQTT;
using ServiceLayerApi.MQTT.Client;

namespace ServiceLayerApi.DeviceNetwork
{
    public class DeviceInfoProcessingService : BaseProcessingService<DeviceInfo>
    {
        public DeviceInfoProcessingService(MqttClientRepository mqttClientRepository) : base(mqttClientRepository)
        {
        }

        protected override string Topic => "data/device";
        protected override Task Process(Guid deviceId, DeviceInfo message)
        {
            
        }

        protected override Task OnStart()
        {
            return Task.CompletedTask;
        }
    }
}