using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ServiceLayerApi.Common;
using ServiceLayerApi.DeviceNetwork.Actuator;
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
        private readonly ConstantImpactActuatorsProvider _constantImpactActuatorsProvider;
        private readonly ILogger<BaseProcessingService<DeviceInfo>> _logger;

        public DeviceInfoProcessingService(
            MqttClientRepository mqttClientRepository,
            DeviceFactory deviceFactory,
            DeviceRepository deviceRepository,
            ConstantImpactActuatorsProvider constantImpactActuatorsProvider,
            ILogger<BaseProcessingService<DeviceInfo>> logger) : base(mqttClientRepository, logger)
        {
            _deviceFactory = deviceFactory;
            _deviceRepository = deviceRepository;
            _constantImpactActuatorsProvider = constantImpactActuatorsProvider;
            _logger = logger;
        }

        protected override string Topic => "data/device";

        protected override Task Process(DeviceInfo message, byte[] originalPayload)
        {
            if (message.Type == DeviceType.Actuator)
            {
                message = originalPayload.DeserializeJsonBytes<ActuatorDeviceInfo>();
            }

            var deviceString = message.ToJson();
            _logger.LogInformation($"Registering device: {deviceString}");
            var device = _deviceFactory.Build(message);
            if (device == null)
            {
                _logger.LogInformation($"Can't build device: {deviceString}");
                return Task.CompletedTask;
            }
            _deviceRepository.StoreDevice(device);
            return Task.CompletedTask;
        }

        protected override Task OnStart()
        {
            var constantActuators = _constantImpactActuatorsProvider.ProvideConstantImpactActuators();
            foreach (var constantActuator in constantActuators)
            {
                _deviceRepository.StoreDevice(constantActuator);
            }
            return Task.CompletedTask;
        }

        protected override Task OnStop() => Task.CompletedTask;
    }
}