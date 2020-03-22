using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using MQTTnet.Extensions.ManagedClient;
using ServiceLayerApi.Common;
using ServiceLayerApi.DataProcessing.Messages;
using ServiceLayerApi.DeviceNetwork.Description;
using ServiceLayerApi.DeviceNetwork.Messages;
using ServiceLayerApi.DeviceNetwork.Sensors;
using ServiceLayerApi.MQTT.Client;
using Xunit;

namespace ServiceLayerApi.Tests.IntegrationTests
{
    public class SensorProcessingTest : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;

        public SensorProcessingTest(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task TestSensorDataProcessing()
        {
            var clientRepository = _factory.Services.GetService<MqttClientRepository>();
            byte[] actualBytes = null;
            var deviceId = Guid.NewGuid();
            var deviceClient = await clientRepository.Subscribe("data/microclimate", (s, bytes) =>
            {
                actualBytes = bytes;
                return Task.CompletedTask;
            });
            var deviceInfo = new DeviceInfo
            {
                DeviceCode = "CustomTemp",
                Id = deviceId,
                Parameter = ParameterType.TemperatureInside,
                Type = DeviceType.Sensor
            };
            var sensorTemperature = 20.4d.ToString(CultureInfo.InvariantCulture);
            var sensorValue = new SensorValues()
            {
                RawValue = sensorTemperature,
                DeviceId = deviceId
            };
            
            await deviceClient.PublishAsync("data/device", deviceInfo);
            await Task.Delay(1000);
            await deviceClient.PublishAsync("data/sensors", sensorValue);
            await Task.Delay(8000);
            var actualMicroClimateParameter = actualBytes.DeserializeJsonBytes<MicroClimateParameterValue>();
            
            Assert.Equal(actualMicroClimateParameter.ParameterType, deviceInfo.Parameter);
            Assert.Equal(actualMicroClimateParameter.Value, sensorTemperature);
        }
    }
}