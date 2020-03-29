using System;

namespace ServiceLayerApi.DeviceNetwork.Description
{
    public enum CommandImpact
    {
        StrongDecrease = -2,
        Decrease = -1,
        NoChange = 0,
        Increase = 1,
        StrongIncrease = 2
    }

    public static class CommandImpactExtensions
    {
        public static bool IsSameChangeDirection(this CommandImpact value, CommandImpact toCompare)
        {
            return value == toCompare
                   || (value > 0 && toCompare > 0)
                   || (value < 0 && toCompare < 0);

        }

        public static bool IsLessOrSameFromSameDirection(this CommandImpact value, CommandImpact toCompare)
        {
            return value.IsSameChangeDirection(toCompare) && Math.Abs((int)value) <= Math.Abs((int)toCompare);
        }
    }
}