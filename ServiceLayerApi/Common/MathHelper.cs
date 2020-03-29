using System;
using System.Collections.Generic;
using System.Linq;
using ServiceLayerApi.DeviceNetwork.Description;

namespace ServiceLayerApi.Common
{
    public static class MathHelper
    {
        public static double Mean(this double[] values)
        {
            if (values.Length == 0)
            {
                return 0;
            }
            return values.Sum() / values.Length;
        }
    }
}