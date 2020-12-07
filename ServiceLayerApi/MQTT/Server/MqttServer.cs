using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Server;

namespace ServiceLayerApi.MQTT.Server
{
    public class MqttServer : BackgroundService
    {
        private readonly ILogger<MqttServer> _logger;
        

        public MqttServer(ILogger<MqttServer> logger)
        {
            _logger = logger;
        }
        
        public IMqttServer NativeMqttServer { get; private set; }
        
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await StartServer().ConfigureAwait(false);
            _logger.LogInformation($"Started MQTT server: addres={ServerConfigurationProvider.ServerAddress}, port={ServerConfigurationProvider.Port}");
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }

        private Task StartServer()
        {
            var optionsBuilder = new MqttServerOptionsBuilder()
                .WithDefaultEndpoint()
                .WithDefaultEndpointPort(ServerConfigurationProvider.Port)
                .WithSubscriptionInterceptor(
                    c =>
                    {
                        c.AcceptSubscription = true;
                        LogMessage(c, true);
                    }).WithApplicationMessageInterceptor(
                    c =>
                    {
                        c.AcceptPublish = true;
                        LogMessage(c);
                    });

            NativeMqttServer = new MqttFactory().CreateMqttServer();
            return NativeMqttServer.StartAsync(optionsBuilder.Build());
        }
        
        private void LogMessage(MqttApplicationMessageInterceptorContext context)
        {
            if (context == null)
            {
                return;
            }

            var payload = context.ApplicationMessage?.Payload == null ? null : Encoding.UTF8.GetString(context.ApplicationMessage?.Payload);

            _logger.LogInformation(
                $"Message: ClientId = {context.ClientId}, Topic = {context.ApplicationMessage?.Topic},"
                + $" Payload = {payload}, QoS = {context.ApplicationMessage?.QualityOfServiceLevel},"
                + $" Retain-Flag = {context.ApplicationMessage?.Retain}");
        }
        
        private void LogMessage(MqttSubscriptionInterceptorContext context, bool successful)
        {
            if (context == null)
            {
                return;
            }

            var message = successful
                ? $"New subscription: ClientId = {context.ClientId}, TopicFilter = {context.TopicFilter}"
                : $"Subscription failed for clientId = {context.ClientId}, TopicFilter = {context.TopicFilter}";
            
            _logger.LogInformation(message);
        }
    }
}