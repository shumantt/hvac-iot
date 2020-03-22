using System;
using System.Threading.Tasks;
using ServiceLayerApi.DeviceNetwork.Description;
using ServiceLayerApi.DeviceNetwork.Messages;
using ServiceLayerApi.MQTT;
using ServiceLayerApi.MQTT.Client;

namespace ServiceLayerApi.DeviceNetwork
{
    public class DeviceInfoProcessingService : BaseProcessingService<DeviceInfo>
    {
        private readonly DeviceFactory _deviceFactory;
        private readonly DeviceRepository _deviceRepository;

        public DeviceInfoProcessingService(
            MqttClientRepository mqttClientRepository,
            DeviceFactory deviceFactory,
            DeviceRepository deviceRepository) : base(mqttClientRepository)
        {
            _deviceFactory = deviceFactory;
            _deviceRepository = deviceRepository;
        }

        protected override string Topic => "data/device";

        protected override Task Process(DeviceInfo message)
        {
            var device = _deviceFactory.Build(message);
            if (device == null)
                return Task.CompletedTask;
            _deviceRepository.StoreDevice(device);
            return Task.CompletedTask;
        }

        protected override Task OnStart() => Task.CompletedTask;

        protected override Task OnStop() => Task.CompletedTask;
    }
}