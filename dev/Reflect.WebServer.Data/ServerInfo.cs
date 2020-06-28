using Newtonsoft.Json;

namespace Reflect.WebServer.Data
{
    public class ServerInfo
    {
        [JsonProperty("server-online")] public bool ServerIsOnline { get; set; }
    }
}