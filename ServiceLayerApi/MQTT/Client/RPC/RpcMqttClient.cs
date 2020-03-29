using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ServiceLayerApi.Common;

namespace ServiceLayerApi.MQTT.Client.RPC
{
    public class RpcMqttClient
    {
        private readonly MqttClientRepository _mqttClientRepository;
        private readonly ILogger<RpcMqttClient> _logger;
        private static readonly TimeSpan DefaultRpcWaitTimeout = TimeSpan.FromSeconds(5);
        private readonly ConcurrentDictionary<Guid, (bool isProcessed, object result)> _scheduledCommands = new ConcurrentDictionary<Guid, (bool isProcessed, object result)>();
        public RpcMqttClient(MqttClientRepository mqttClientRepository, ILogger<RpcMqttClient> logger)
        {
            _mqttClientRepository = mqttClientRepository;
            _logger = logger;
        }

        public async Task<TResult> ProcessRpcRequest<TResult, TRequest>(
            string responseTopic,
            string requestTopic,
            TRequest requestMessage,
            TimeSpan? waitTimeout = null) 
            where TRequest : IRpcCommand 
            where TResult : IRpcCommand
        {
            var checkTimeout = waitTimeout ?? DefaultRpcWaitTimeout;
            
            _scheduledCommands.TryAdd(requestMessage.CommandId, (false, null));
            
            var client = await _mqttClientRepository.Subscribe(responseTopic, HandleRpcResponse).ConfigureAwait(false);
            _logger.LogInformation($"Make rpc request for command: {requestMessage.ToJson()}");
            await client.PublishAsync(requestTopic, requestMessage).ConfigureAwait(false);
          
            using (var cts = new CancellationTokenSource())
            {
                try
                {
                    cts.CancelAfter(checkTimeout);
                    var result = await Processing(cts.Token).ConfigureAwait(false);
                    _scheduledCommands.TryRemove(requestMessage.CommandId, out _);
                    return result;
                }
                catch (OperationCanceledException)
                {
                    _scheduledCommands.TryRemove(requestMessage.CommandId, out _);
                    throw new TimeoutException($"Can't process command with timeout = {checkTimeout}: {requestMessage.ToJson()}");
                }
            }
            
            Task HandleRpcResponse(string clientId, byte[] responseMessage)
            {
                var response = responseMessage.DeserializeJsonBytes<TResult>();
                if (!_scheduledCommands.ContainsKey(response.CommandId))
                {
                    _logger.LogInformation($"Received unregistered command: {response.ToJson()}");
                    return Task.CompletedTask;
                }
                _logger.LogInformation($"Received rpc response {response.ToJson()}");
                _scheduledCommands[response.CommandId] = (true, response);
                return Task.CompletedTask;
            }
            
            async Task<TResult> Processing(CancellationToken ct)
            {
                var (isProcessed, result) = _scheduledCommands[requestMessage.CommandId];
                while (!isProcessed && !ct.IsCancellationRequested)
                {
                    await Task.Delay(100, ct).ConfigureAwait(false);
                    (isProcessed, result) = _scheduledCommands[requestMessage.CommandId];
                }

                if (result == null && ct.IsCancellationRequested)
                {
                    throw new OperationCanceledException();
                }
                return (TResult)result;
            }
        }
    }
}