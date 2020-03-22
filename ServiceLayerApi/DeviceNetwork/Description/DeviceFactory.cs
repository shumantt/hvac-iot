using System;
using System.Collections.Generic;
using System.Linq;
using ServiceLayerApi.Common;
using ServiceLayerApi.DeviceNetwork.Messages;

namespace ServiceLayerApi.DeviceNetwork.Description
{
    public class DeviceFactory
    {
        private readonly IDeviceBuilder[] _deviceBuilders;

        public DeviceFactory(IEnumerable<IDeviceBuilder> deviceBuilders)
        {
            _deviceBuilders = deviceBuilders.ToArray();
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
            return null;
        }
    }
}