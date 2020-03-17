using System.Text.Json;

namespace ServiceLayerApi.Common
{
    public static class Serializer
    {
        public static string ToJson(this object value)
        {
            return JsonSerializer.Serialize(value);
        }
    }
}