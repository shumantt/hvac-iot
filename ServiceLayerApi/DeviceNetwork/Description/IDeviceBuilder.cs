using ServiceLayerApi.DeviceNetwork.Messages;

namespace ServiceLayerApi.DeviceNetwork.Description
{
    public interface IDeviceBuilder
    {
        bool CanBuild(DeviceInfo deviceInfo);
        IDevice Build(DeviceInfo deviceInfo);
    }
}