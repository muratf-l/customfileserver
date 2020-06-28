namespace Reflect.WebServer.Data
{
    public delegate void ServiceEventHandler(object sender, ServiceData e);

    public delegate void LogEventHandler(object sender, string msg);

    public enum ServiceStatus
    {
        None = 0,
        Working = 1,
        Completed = 2,
        Error = 3
    }

    public class ServiceData
    {
        public ServiceStatus Status { get; set; }
        public int Total { get; set; }
        public int Current { get; set; }
        public string Message { get; set; }
    }
}