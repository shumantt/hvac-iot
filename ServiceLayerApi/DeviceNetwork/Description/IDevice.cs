using System;

namespace ServiceLayerApi.DeviceNetwork.Description
{
    public interface IDevice
    {
        Guid Id { get; }
        ParameterType Parameter { get; }
    }
}