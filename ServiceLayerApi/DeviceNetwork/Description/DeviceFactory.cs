using System;
using System.Linq;
using ServiceLayerApi.Common;
using ServiceLayerApi.DeviceNetwork.Description.DeviceBuilders;
using ServiceLayerApi.DeviceNetwork.Messages;

namespace ServiceLayerApi.DeviceNetwork.Description
{
    public class DeviceFactory
    {
        private readonly IDeviceBuilder[] _deviceBuilders;

        public DeviceFactory(IDeviceBuilder[] deviceBuilders)
        {
            _deviceBuilders = deviceBuilders;
        }
        
        public IDevice Build(DeviceInfo deviceInfo)
        {
            var builders = _deviceBuilders.Where(x => x.CanBuild(deviceInfo)).ToArray();
            if (builders.Length > 1)
            {
                throw new InvalidOperationException($"Several builders for device: {deviceInfo.ToJson()}");
            }

            var builder = builders.SingleOrDefault();
            
            return builder == null ? BuildDefault(deviceInfo) : builder.Build(deviceInfo);
        }

        private IDevice BuildDefault(DeviceInfo deviceInfo)
        {
            throw new InvalidOperationException($"No builders for device: {deviceInfo.ToJson()}");
        }
    }
}