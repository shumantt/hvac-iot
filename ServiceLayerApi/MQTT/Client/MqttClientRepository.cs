using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Client.Options;
using MQTTnet.Diagnostics;
using MQTTnet.Extensions.ManagedClient;
using ServiceLayerApi.Common;

namespace ServiceLayerApi.MQTT.Client
{
    public class MqttClientRepository
    {
        private readonly ILogger<MqttClientRepository> _logger;
        private ConcurrentDictionary<string, IManagedMqttClient> _clients;
            
        public MqttClientRepository(ILogger<MqttClientRepository> logger)
        {
            _logger = logger;
        }
        
        public async Task<IManagedMqttClient> Subscribe(string topic, Func<string, byte[], Task> messageHandler)
        {
            if (_clients.ContainsKey(topic))
                return _clients[topic];

            var logger = new MqttNetLogger();
            logger.LogMessagePublished += (sender, args) =>
            {
                _logger.LogInformation(args.TraceMessage.Exception, args.TraceMessage.Message);
            };
            
            // Create a new MQTT client.
            var factory = new MqttFactory(logger);
            var mqttClient = factory.CreateManagedMqttClient();
            
            var options = new ManagedMqttClientOptionsBuilder()
                .WithAutoReconnectDelay(TimeSpan.FromSeconds(5))
                .WithClientOptions(new MqttClientOptionsBuilder()
                    .WithClientId(Guid.NewGuid().ToString())
                    .WithTcpServer("broker.com")
                    .Build())
                .Build();
            
            mqttClient.UseConnectedHandler(e =>
            {
                _logger.LogInformation($"Connected: {e.AuthenticateResult.ToJson()}");
            });
            
            mqttClient.UseApplicationMessageReceivedHandler(async e =>
            {
                await messageHandler(e.ClientId, e.ApplicationMessage.Payload).ConfigureAwait(false);
            });
            
            await mqttClient.SubscribeAsync(new TopicFilterBuilder().WithTopic(topic).Build());
            await mqttClient.StartAsync(options);

            _clients.TryAdd(topic, mqttClient);
            return mqttClient;
        }
    }
}