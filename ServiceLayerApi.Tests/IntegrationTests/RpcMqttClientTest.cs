using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using ServiceLayerApi.Common;
using ServiceLayerApi.MQTT.Client;
using ServiceLayerApi.MQTT.Client.RPC;
using Xunit;

namespace ServiceLayerApi.Tests.IntegrationTests
{
    public class RpcMqttClientTest : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;

        public RpcMqttClientTest(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task TestRpcSuccessWithTimeout()
        {
            var requestTopic = "command/request";
            var responseTopic = "command/response";
            var request = new TestRpcMessage() {CommandId = Guid.NewGuid()};
            var rpcClient = _factory.Services.GetService<RpcMqttClient>();
            var clientRepository = _factory.Services.GetService<MqttClientRepository>();
            var pushDeviceClient = await clientRepository.Subscribe("_", (s, bytes) => Task.CompletedTask);
            var deviceClient = clientRepository.Subscribe(requestTopic, messageHandler: (s, bytes)
                => pushDeviceClient.PublishAsync(responseTopic, bytes.DeserializeJsonBytes<TestRpcMessage>()));

            var result = await rpcClient
                .ProcessRpcRequest<TestRpcMessage, TestRpcMessage>(responseTopic, requestTopic, request, TimeSpan.FromSeconds(5))
                .ConfigureAwait(false);

            
            Assert.Equal(result.CommandId, request.CommandId);
        }
        
        [Fact]
        public async Task TestRpcFailedWithTimeout()
        {
            var requestTopic = "command/request";
            var responseTopic = "command/response";
            var request = new TestRpcMessage() {CommandId = Guid.NewGuid()};
            var rpcClient = _factory.Services.GetService<RpcMqttClient>();
            var clientRepository = _factory.Services.GetService<MqttClientRepository>();
            var pushDeviceClient = await clientRepository.Subscribe("_", (s, bytes) => Task.CompletedTask);
            var deviceClient = clientRepository.Subscribe(requestTopic, (s, bytes) 
                => pushDeviceClient.PublishAsync(responseTopic, bytes.DeserializeJsonBytes<TestRpcMessage>()));


            var callTask = rpcClient
                .ProcessRpcRequest<TestRpcMessage, TestRpcMessage>(responseTopic, requestTopic, request,
                    TimeSpan.FromMilliseconds(10));

            var exception = await Assert.ThrowsAsync<TimeoutException>(async () => await callTask);
            Assert.Contains(request.CommandId.ToString(), exception.Message);
        }

        public class TestRpcMessage : IRpcCommand
        {
            public Guid CommandId { get; set; }
        }
    }
}