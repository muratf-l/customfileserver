using System;
using Newtonsoft.Json;

namespace Reflect.WebServer.Data
{
    [Serializable]
    public enum ServerAction
    {
        Info = 0,
        ServerStatus = 10,
        StartServer = 20,
        RebootServer = 30,
        StopServer = 40,
        Log = 50
    }

    [Serializable]
    public class ServerCommand
    {
        public ServerAction Action { get; set; }

        public string Data { get; set; }

        public T ToObjects<T>() where T : new()
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(Data);
            }
            catch
            {
                //
            }

            return new T();
        }
    }
}