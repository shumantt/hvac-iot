using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MQTTnet.Extensions.ManagedClient;
using ServiceLayerApi.Common;

namespace ServiceLayerApi.MQTT.Client
{
    public abstract class BaseProcessingService<TMessage> : BackgroundService
    {
        private readonly MqttClientRepository _mqttClientRepository;
        private readonly ILogger<BaseProcessingService<TMessage>> _logger;

        protected BaseProcessingService(MqttClientRepository mqttClientRepository, ILogger<BaseProcessingService<TMessage>> logger)
        {
            _mqttClientRepository = mqttClientRepository;
            _logger = logger;
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
            try
            {
                return Process(message);
            }
            catch (Exception e)
            {
                _logger.LogError($"Error processing message: {message.ToJson()}. ClientId: {clientId}");
                return Task.CompletedTask;
            }
        }
        
        protected abstract Task Process(TMessage message);

        protected abstract Task OnStart();
    }
}