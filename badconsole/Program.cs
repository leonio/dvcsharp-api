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

            var probe = new BadApiProbe("localhost", 5000);

            var found = await probe.ProbeAsync(cts.Token);

            Console.WriteLine("Did you find a target? {0}", found);
        }
    }
}
