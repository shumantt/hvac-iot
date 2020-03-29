using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using ServiceLayerApi.CommandProcessing.Models;
using ServiceLayerApi.Common;
using ServiceLayerApi.DeviceNetwork.Description;
using ServiceLayerApi.DeviceNetwork.Messages;
using ServiceLayerApi.MQTT.Client;
using Xunit;

namespace ServiceLayerApi.Tests.IntegrationTests
{
    public class CommandsProcessingTest : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;

        public CommandsProcessingTest(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task TestDecisionCommandProcessing()
        {
            var rpcRequestTopic = "command/request";
            var rpcResponseTopic = "command/response";
            var apiClient = _factory.CreateClient();
            var mqttClientRepository = _factory.Services.GetService<MqttClientRepository>();
            var decisionCommand = new DecisionCommand()
            {
                ParameterCommands = new []
                {
                    new ParameterCommand()
                    {
                        CommandImpact = CommandImpact.Decrease,
                        Parameter = ParameterType.TemperatureInside
                    }
                }
            };
            var clientReceivedCommands = new List<RpcCommandRequest>();
            var devicePushClient = await mqttClientRepository.Subscribe("_", ((s, bytes) => Task.CompletedTask));
            var deviceClient = await mqttClientRepository.Subscribe(rpcRequestTopic, (s, bytes) =>
            {
                var receivedCommand = bytes.DeserializeJsonBytes<RpcCommandRequest>();
                clientReceivedCommands.Add(receivedCommand);
                return devicePushClient.PublishAsync(rpcResponseTopic, new RpcCommandResponse()
                {
                    CommandId = receivedCommand.CommandId,
                    FailedMessage = null,
                    IsFailed = false
                });
            });
            var tempDecreaseDeviceInfo = new ActuatorDeviceInfo()
            {
                DeviceCode = "Custom",
                Id = Guid.NewGuid(),
                IsConstantImpact = false,
                Impacts = new[] {CommandImpact.Decrease, CommandImpact.StrongDecrease},
                Parameter = ParameterType.TemperatureInside,
            };

            await deviceClient.PublishAsync("data/device", tempDecreaseDeviceInfo);
            await Task.Delay(1000);
            var request = new StringContent(decisionCommand.ToJson());
            request.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
            var decisionCommandHttpResult = await apiClient.PostAsync("commands/decision", request);
            await Task.Delay(1000);

            decisionCommandHttpResult.EnsureSuccessStatusCode();
            var decisionCommandResult = (await decisionCommandHttpResult.Content.ReadAsStringAsync())
                .DeserializeJson<DecisionCommandProcessResult>();
            var executedParameterCommand = decisionCommandResult.ParameterCommandProcessResults.Single();
            var executedImpact = executedParameterCommand.Impacts.Single();
            var receivedDeviceCommand = clientReceivedCommands.Single();
            
            Assert.Null(executedParameterCommand.Error);
            Assert.False(executedParameterCommand.Failed);
            Assert.Equal(CommandImpact.Decrease, executedImpact);
            Assert.Equal(CommandImpact.Decrease, receivedDeviceCommand.Impact);
            Assert.Equal(ParameterType.TemperatureInside, receivedDeviceCommand.Parameter);
            Assert.Equal(tempDecreaseDeviceInfo.Id, receivedDeviceCommand.DeviceId);
        }
    }
}