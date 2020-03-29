using System.Text;
using System.Text.Json;

namespace ServiceLayerApi.Common
{
    public static class Serializer
    {
        public static string ToJson<T>(this T value)
        {
            return JsonSerializer.Serialize(value);
        }

        public static T DeserializeJson<T>(this string value)
        {
            return JsonSerializer.Deserialize<T>(value);
        }

        public static byte[] ToJsonBytes<T>(this T value)
        {
            var json = JsonSerializer.Serialize(value);
            return Encoding.UTF8.GetBytes(json);
        }

        public static T DeserializeJsonBytes<T>(this byte[] value)
        {
            var json = Encoding.UTF8.GetString(value);
            return DeserializeJson<T>(json);
        }
    }
}