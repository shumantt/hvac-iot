namespace ServiceLayerApi.DeviceNetwork.Description
{
    public enum ParameterType
    {
        Unknown = 0,
        TemperatureInside,
        TemperatureOutside,
        Humidity,
        Co2,
        AtmosphericPressure,
        Energy
    }

    public static class ParameterTypeExtensions
    {
        public static bool IsTemperature(this ParameterType parameterType)
            => parameterType == ParameterType.TemperatureInside || parameterType == ParameterType.TemperatureOutside;
    }
}