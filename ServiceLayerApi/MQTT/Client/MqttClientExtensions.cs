using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Extensions.ManagedClient;
using ServiceLayerApi.Common;
using ServiceLayerApi.MQTT.Client;

namespace ServiceLayerApi.MQTT
{
    public static class MqttClientExtensions
    {
        public static Task PublishAsync<T>(this IManagedMqttClient client, string topic, T payload)
        {
            var message = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(payload.ToJson())
                .WithExactlyOnceQoS()
                .WithRetainFlag()
                .Build();

            return client.PublishAsync(message);
        }
    }
}