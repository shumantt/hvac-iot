using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using MQTTnet.Extensions.ManagedClient;
using ServiceLayerApi.Common;

namespace ServiceLayerApi.MQTT.Client
{
    public abstract class BaseProcessingService<TMessage> : BackgroundService
    {
        private readonly MqttClientRepository _mqttClientRepository;

        protected BaseProcessingService(MqttClientRepository mqttClientRepository)
        {
            _mqttClientRepository = mqttClientRepository;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Start().ConfigureAwait(false);
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }

            await Stop().ConfigureAwait(false);
        }

        protected abstract string Topic { get; }

        private async Task Start()
        {
            MqttClient = await _mqttClientRepository.Subscribe(Topic, HandleMessage).ConfigureAwait(false);
            await OnStart().ConfigureAwait(false);
        }

        private async Task Stop()
        {
            await MqttClient.UnsubscribeAsync().ConfigureAwait(false);
            await OnStop().ConfigureAwait(false);
        }

        protected abstract Task OnStop();

        protected IManagedMqttClient MqttClient { get; private set; }

        private Task HandleMessage(string clientId, byte[] payload)
        {
            var message = payload.DeserializeJsonBytes<TMessage>();
            var deviceId = Guid.Parse(clientId);
            return Process(deviceId, message);
        }
        
        protected abstract Task Process(Guid deviceId, TMessage message);

        protected abstract Task OnStart();
    }
}