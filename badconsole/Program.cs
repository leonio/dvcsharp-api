using System;
using System.Threading;
using System.Threading.Tasks;

namespace badconsole
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (s, e) =>
            {
                Console.WriteLine("Canceling...");
                cts.Cancel();
                e.Cancel = true;
            };

            var connector = new BadApiConnector("localhost", 5000);

            var found = await connector.ProbeAsync(cts.Token);

            Console.WriteLine("Did you find a target? {0}", found);
        }
    }
}
