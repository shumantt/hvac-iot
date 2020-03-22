using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using ServiceLayerApi.Common;
using ServiceLayerApi.MQTT;
using ServiceLayerApi.MQTT.Client;
using Xunit;

namespace ServiceLayerApi.Tests.IntegrationTests
{
    public class MqttClientServerTest : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;

        public MqttClientServerTest(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task TestServerCommunication()
        {
            var clientRepository = _factory.Services.GetService<MqttClientRepository>();
            var testTopic = "testTopic";
            var sentMessage = "test message";
            byte[] actualBytes = null;
            string actualClientId = null;
            
            var client = await clientRepository.Subscribe(testTopic, (id, bytes) =>
            {
                actualClientId = id;
                actualBytes = bytes;
                return Task.CompletedTask;
            });
            var clientId = client.Options.ClientOptions.ClientId;

            await client.PublishAsync(testTopic, sentMessage);

            await Task.Delay(1000);
            
            var actualMessage = actualBytes.DeserializeJsonBytes<string>();
            Assert.Equal(clientId, actualClientId);
            Assert.Equal(actualMessage, sentMessage);
        }
    }
}