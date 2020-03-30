namespace ServiceLayerApi.DeviceNetwork.Description
{
    public enum ParameterType
    {
        Unknown = 0,
        TemperatureInside = 1,
        TemperatureOutside = 2,
        Humidity = 3,
        Co2 = 4,
        AtmosphericPressure = 5,
        Energy = 6
    }

    public static class ParameterTypeExtensions
    {
        public static bool IsTemperature(this ParameterType parameterType)
            => parameterType == ParameterType.TemperatureInside || parameterType == ParameterType.TemperatureOutside;
    }
}