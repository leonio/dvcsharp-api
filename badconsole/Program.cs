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

            if (found)
            {
                var cmdRunner = new BadApiCommandRunner(probe.BaseUrl);
                
                var session = await cmdRunner.InitSessionAsync(cts.Token);

                if (session.IsEstablished)
                {
                    // await new ListUsersCommand().ExecuteAsync(session, cts.Token);

                    // await new XmlBombCommand().ExecuteAsync(session, cts.Token);
                    // await new ImportAnythingCommand().ExecuteAsync(session, cts.Token);
                    // await new PasswordResetAnyoneCommand().ExecuteAsync(session, cts.Token);
                    await new ChangeRoleCommand().ExecuteAsync(session, cts.Token);
                }
                else
                {
                    Console.WriteLine("Failed to establish session... existing");
                }
            }

            Console.WriteLine("Did you find a target? {0}", found);
        }
    }
}
