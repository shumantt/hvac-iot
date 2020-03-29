using ServiceLayerApi.DeviceNetwork.Messages;

namespace ServiceLayerApi.DeviceNetwork.Description
{
    public interface IDevice
    {
        DeviceInfo DeviceInfo { get; }
    }
}