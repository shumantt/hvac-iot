using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using ServiceLayerApi.DeviceNetwork;
using ServiceLayerApi.DeviceNetwork.Description;
using ServiceLayerApi.DeviceNetwork.Messages;
using ServiceLayerApi.MQTT.Client;
using Xunit;

namespace ServiceLayerApi.Tests.IntegrationTests
{
    public class DeviceProcessingTest : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;

        public DeviceProcessingTest(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task TestProcessDeviceInfo()
        {
            var clientRepository = _factory.Services.GetService<MqttClientRepository>();
            var deviceRepository = _factory.Services.GetService<DeviceRepository>();
            var deviceTopic = "data/device";
            var client = await clientRepository.Subscribe(deviceTopic, ((s, bytes) => Task.CompletedTask));
            var deviceInfo = new DeviceInfo
            {
                DeviceCode = "CustomTemp",
                Id = Guid.NewGuid(),
                Parameter = ParameterType.TemperatureInside,
                Type = DeviceType.Sensor
            };

            await client.PublishAsync(deviceTopic, deviceInfo);
            await Task.Delay(1000);
            var registeredDevice = deviceRepository.GetSensor(deviceInfo.Id);
            
            Assert.NotNull(registeredDevice);
            Assert.Equal(registeredDevice.DeviceInfo.Id, deviceInfo.Id);
            Assert.Equal(registeredDevice.DeviceInfo.Parameter, deviceInfo.Parameter);
        }
    }
}