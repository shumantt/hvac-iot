using System.Linq;
using System.Net;

namespace ServiceLayerApi.MQTT.Server
{
    public static class ServerConfigurationProvider
    {
        private static string _serverAddress;
        
        public static string ServerAddress
        {
            get
            {
                if (_serverAddress == null)
                {
                    var ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                    _serverAddress = ipHostInfo.AddressList.First().ToString();
                }

                return _serverAddress;
            }
        }
        
        public const int Port = 54893;
    }
}