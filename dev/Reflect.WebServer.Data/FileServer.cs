using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using Newtonsoft.Json;

namespace Reflect.WebServer.Data
{
    public class FileServer
    {
        private static readonly IDictionary<string, string> MimeTypeMappings =
            new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
            {
                #region extension to MIME type list

                {".asf", "video/x-ms-asf"},
                {".asx", "video/x-ms-asf"},
                {".avi", "video/x-msvideo"},
                {".bin", "application/octet-stream"},
                {".cco", "application/x-cocoa"},
                {".crt", "application/x-x509-ca-cert"},
                {".css", "text/css"},
                {".deb", "application/octet-stream"},
                {".der", "application/x-x509-ca-cert"},
                {".dll", "application/octet-stream"},
                {".dmg", "application/octet-stream"},
                {".ear", "application/java-archive"},
                {".eot", "application/octet-stream"},
                {".exe", "application/octet-stream"},
                {".flv", "video/x-flv"},
                {".gif", "image/gif"},
                {".hqx", "application/mac-binhex40"},
                {".htc", "text/x-component"},
                {".htm", "text/html"},
                {".html", "text/html"},
                {".ico", "image/x-icon"},
                {".img", "application/octet-stream"},
                {".iso", "application/octet-stream"},
                {".jar", "application/java-archive"},
                {".jardiff", "application/x-java-archive-diff"},
                {".jng", "image/x-jng"},
                {".jnlp", "application/x-java-jnlp-file"},
                {".jpeg", "image/jpeg"},
                {".jpg", "image/jpeg"},
                {".js", "application/x-javascript"},
                {".mml", "text/mathml"},
                {".mng", "video/x-mng"},
                {".mov", "video/quicktime"},
                {".mp3", "audio/mpeg"},
                {".mpeg", "video/mpeg"},
                {".mpg", "video/mpeg"},
                {".msi", "application/octet-stream"},
                {".msm", "application/octet-stream"},
                {".msp", "application/octet-stream"},
                {".pdb", "application/x-pilot"},
                {".pdf", "application/pdf"},
                {".pem", "application/x-x509-ca-cert"},
                {".pl", "application/x-perl"},
                {".pm", "application/x-perl"},
                {".png", "image/png"},
                {".prc", "application/x-pilot"},
                {".ra", "audio/x-realaudio"},
                {".rar", "application/x-rar-compressed"},
                {".rpm", "application/x-redhat-package-manager"},
                {".rss", "text/xml"},
                {".run", "application/x-makeself"},
                {".sea", "application/x-sea"},
                {".shtml", "text/html"},
                {".sit", "application/x-stuffit"},
                {".swf", "application/x-shockwave-flash"},
                {".tcl", "application/x-tcl"},
                {".tk", "application/x-tcl"},
                {".txt", "text/plain"},
                {".war", "application/java-archive"},
                {".wbmp", "image/vnd.wap.wbmp"},
                {".wmv", "video/x-ms-wmv"},
                {".xml", "text/xml"},
                {".xpi", "application/x-xpinstall"},
                {".zip", "application/zip"},
                {".mp4", "video/mp4"},

                #endregion
            };

        private readonly string[] _indexFiles =
        {
            "index.html",
            "index.htm",
            "default.html",
            "default.htm"
        };

        private bool _isRunning;

        private HttpListener _listener;

        private Thread _serverThread;

        public LogEventHandler LogEventHandler;
        public LogEventHandler StatusEventHandler;


        //public FileServer(string path)
        //{
        //    var l = new TcpListener(IPAddress.Loopback, 0);
        //    l.Start();

        //    var port = ((IPEndPoint)l.LocalEndpoint).Port;
        //    l.Stop();

        //    Initialize(path, port);
        //}

        public string Root => GlobalSettings.ServerRoot;

        public int Port => GlobalSettings.ServerPort;

        public bool IsRunning
        {
            get => _isRunning;
            private set
            {
                _isRunning = value;
                StatusEventHandler?.Invoke(this, "");
            }
        }

        private void SetLog(string msg)
        {
            LogEventHandler?.Invoke(this, msg);
        }

        public void Start()
        {
            if (IsRunning) return;
            SetLog($"FileServer Start port:{Port} root:{Root}");
            IsRunning = true;
            _serverThread = new Thread(Listen);
            _serverThread.Start();
        }

        public void Stop()
        {
            SetLog("FileServer Stop");
            IsRunning = false;
            _serverThread.Abort();
            _listener.Stop();
        }

        private void Listen()
        {
            IsRunning = true;
            _listener = new HttpListener();
            _listener.Prefixes.Clear();
            _listener.Prefixes.Add($"http://*:{Port}/");


            try
            {
                _listener.Start();
            }
            catch (HttpListenerException ex)
            {
                IsRunning = false;
                SetLog($"FileServer Listen Start err:{ex.Message}");
                return;
            }

            while (true)
                try
                {
                    ThreadPool.QueueUserWorkItem(Process, _listener.GetContext());

                    //var context = _listener.GetContext();
                    //Process(context);
                }
                catch (Exception ex)
                {
                    IsRunning = false;
                    SetLog($"FileServer Listen err:{ex.Message}");
                }
        }

        private void Process(object o)
        {
            var context = o as HttpListenerContext;

            var filename = WebUtility.UrlDecode(context.Request.Url.AbsolutePath.Substring(1));

            SetLog($"FileServer Process file:{filename}");

            if (filename.EndsWith("getfiles"))
            {
                var fName = filename.Split('/');
                filename = string.Join("/", fName, 0, fName.Length - 1);
                filename = Path.Combine(Root, filename);

                try
                {
                    var files = new List<string>();

                    var dirFolders = Directory.GetDirectories(filename, @"*");

                    foreach (var f in dirFolders) files.Add(Path.GetFileName(f));

                    var dirFiles = Directory.GetFiles(filename, @"*");

                    foreach (var f in dirFiles) files.Add(Path.GetFileName(f));

                    var jsonContent = JsonConvert.SerializeObject(files);

                    var buffer = Encoding.UTF8.GetBytes(jsonContent);

                    context.Response.ContentLength64 = buffer.Length;

                    var output = context.Response.OutputStream;
                    output.Write(buffer, 0, buffer.Length);

                    context.Response.StatusCode = (int) HttpStatusCode.OK;
                    context.Response.OutputStream.Flush();

                    output.Close();
                    output.Dispose();
                    return;
                }
                catch
                {
                    context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
                    return;
                }
            }

            if (string.IsNullOrEmpty(filename))
                foreach (var indexFile in _indexFiles)
                    if (File.Exists(Path.Combine(Root, indexFile)))
                    {
                        filename = indexFile;
                        break;
                    }


            filename = Path.Combine(Root, filename);

            SetLog($"FileServer Process path:{filename}");

            if (File.Exists(filename))
                try
                {
                    var response = context.Response;

                    using (var fileStream = File.OpenRead(filename))
                    {
                        response.ContentType =
                            MimeTypeMappings.TryGetValue(Path.GetExtension(filename), out var mime)
                                ? mime
                                : "application/octet-stream";
                        response.ContentLength64 = new FileInfo(filename).Length;
                        //response.AddHeader(
                        //    "Content-Disposition",
                        //    "Attachment; filename=\"" + Path.GetFileName(filename) + "\"");
                        fileStream.CopyTo(response.OutputStream);
                    }

                    response.StatusCode = (int) HttpStatusCode.OK;
                    response.OutputStream.Close();
                }
                catch (Exception ex)
                {
                    SetLog($"FileServer Process err:{ex.Message}");
                    context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
                }
            else
                context.Response.StatusCode = (int) HttpStatusCode.NotFound;

            context.Response.OutputStream.Close();
            context.Response.OutputStream.Dispose();
        }

        private void SendErrorResponse(HttpListenerResponse response, int statusCode, string statusResponse)
        {
            response.ContentLength64 = 0;
            response.StatusCode = statusCode;
            response.StatusDescription = statusResponse;
            response.OutputStream.Close();
            SetLog($"*** Sent error: {statusCode} {statusResponse}");
        }
    }
}