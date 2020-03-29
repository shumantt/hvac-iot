using System.Text.Json;

namespace ServiceLayerApi
{
    public class JsonPascalCaseNamingPolicy : JsonNamingPolicy
    {
        public override string ConvertName(string name) => name;
    }
}