using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Deque.AxeCore.Commons.Test.Util
{
    public sealed class FixtureServer : IDisposable
    {
        private const string CspSandboxPrefix = "/csp-sandbox";

        private readonly string _root;
        private readonly HttpListener _listener = new HttpListener();

        public string BaseUrl { get; }

        public FixtureServer(string root)
        {
            _root = Path.GetFullPath(root).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            BaseUrl = $"http://localhost:{FreePort()}";
            _listener.Prefixes.Add($"{BaseUrl}/");
            _listener.Start();
            Task.Run(Serve);
        }

        public void Dispose()
        {
            _listener.Close();
        }

        private static int FreePort()
        {
            var probe = new TcpListener(IPAddress.Loopback, 0);
            probe.Start();
            var port = ((IPEndPoint)probe.LocalEndpoint).Port;
            probe.Stop();
            return port;
        }

        private async Task Serve()
        {
            while (_listener.IsListening)
            {
                HttpListenerContext context;
                try
                {
                    context = await _listener.GetContextAsync();
                }
                catch (Exception) when (!_listener.IsListening)
                {
                    return;
                }
                try
                {
                    Respond(context);
                }
                catch (Exception)
                {
                    context.Response.Abort();
                }
            }
        }

        private void Respond(HttpListenerContext context)
        {
            var response = context.Response;
            var path = Uri.UnescapeDataString(context.Request.Url.AbsolutePath);
            if (path.StartsWith(CspSandboxPrefix + "/", StringComparison.Ordinal))
            {
                path = path.Substring(CspSandboxPrefix.Length);
                response.AddHeader("Content-Security-Policy", "sandbox allow-same-origin");
            }

            var file = Path.GetFullPath(Path.Combine(_root, path.TrimStart('/')));
            if (file.StartsWith(_root + Path.DirectorySeparatorChar, StringComparison.Ordinal) && File.Exists(file))
            {
                var body = File.ReadAllBytes(file);
                response.ContentType = file.EndsWith(".html", StringComparison.OrdinalIgnoreCase)
                    ? "text/html"
                    : "application/octet-stream";
                response.ContentLength64 = body.Length;
                response.OutputStream.Write(body, 0, body.Length);
            }
            else
            {
                response.StatusCode = 404;
            }
            response.Close();
        }
    }
}
