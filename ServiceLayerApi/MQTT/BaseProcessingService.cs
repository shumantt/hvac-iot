using System;
using System.Text;
using System.Threading.Tasks;
using MQTTnet.Extensions.ManagedClient;
using ServiceLayerApi.Common;
using ServiceLayerApi.MQTT.Client;

namespace ServiceLayerApi.MQTT
{
    public abstract class BaseProcessingService<TMessage>
    {
        private readonly MqttClientRepository _mqttClientRepository;

        protected BaseProcessingService(MqttClientRepository mqttClientRepository)
        {
            _mqttClientRepository = mqttClientRepository;
        }

        protected abstract string Topic { get; }

        public async Task Start()
        {
            MqttClient = await _mqttClientRepository.Subscribe(Topic, HandleMessage);
            await OnStart();
        }

        protected IManagedMqttClient MqttClient { get; private set; }

        private Task HandleMessage(string clientId, byte[] payload)
        {
            var message = Encoding.UTF8.GetString(payload).Deserialize<TMessage>();
            var deviceId = Guid.Parse(clientId);
            return Process(deviceId, message);
        }
        
        protected abstract Task Process(Guid deviceId, TMessage message);

        protected abstract Task OnStart();
    }
}