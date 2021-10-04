using System;
using System.Threading;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;

namespace badconsole
{
    public class BadApiProbe
    {
        private readonly Url _baseUrl;

        public BadApiProbe(string host, int port = 80, bool isHttps = false)
        {
            _baseUrl = new Url
            {
                Scheme = isHttps ? "https" : "http",
                Host = host,
                Port = port
            };
        }

        public Url BaseUrl => _baseUrl;

        public string BaseUrlString => _baseUrl.ToString();

        public async Task<bool> ProbeAsync(CancellationToken token = default)
        {
            try
            {
                var result = await BaseUrlString.WithTimeout(10).GetStringAsync(token);
                
                return result.Contains("DVCSharp API: Route not found");
            }
            catch (Exception)
            {
                System.Diagnostics.Trace.WriteLine("Probe failed on {0}", _baseUrl);
            }
            
            return false;
        }
    }
}
