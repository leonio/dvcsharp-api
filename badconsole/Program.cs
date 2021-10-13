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

            Console.Write("Is target there? scan localhost from 5000");
            var probe = new BadApiProbe("localhost", 5000);

            var found = await probe.ProbeAsync(cts.Token);

            if (found)
            {
                Console.WriteLine("Found...establishing session");
                var cmdRunner = new BadApiCommandRunner(probe.BaseUrl);
                
                var session = await cmdRunner.InitSessionAsync(cts.Token);

                if (session.IsEstablished)
                {
                    Console.WriteLine("Session established");
                    var exit = false;
                    do
                    {
                        exit = await ProcessInputAsync(session, cts.Token);
                    } while (!exit);
                    // await new ListUsersCommand().ExecuteAsync(session, cts.Token);

                    // await new XmlBombCommand().ExecuteAsync(session, cts.Token);
                    // await new ImportAnythingCommand().ExecuteAsync(session, cts.Token);
                    // await new PasswordResetAnyoneCommand().ExecuteAsync(session, cts.Token);
                    // await new ChangeRoleCommand().ExecuteAsync(session, cts.Token);
                }
                else
                {
                    Console.WriteLine("Failed to establish session... existing");
                }
            }

            Console.WriteLine("Did you find a target? {0}", found);
        }

        private static async Task<bool> ProcessInputAsync(BadApiSession session, CancellationToken token)
        {
            Console.WriteLine("Try what?? or 'q' to exit...");
            Console.WriteLine("1. List all users/roles?");
            Console.WriteLine("2. xml bomb?");
            Console.WriteLine("3. Import attacks?");
            Console.WriteLine("4. Reset someones password?");
            Console.WriteLine("5. Change users role to Administrator?");
            Console.Write("> ");

            var key = Console.ReadKey(intercept: false);

            var task = key.Key switch
            {
                ConsoleKey.D1 => new ListUsersCommand().ExecuteAsync(session, token),
                ConsoleKey.D2 => new XmlBombCommand().ExecuteAsync(session, token),
                ConsoleKey.D3 => new ImportAnythingCommand().ExecuteAsync(session, token),
                ConsoleKey.D4 => new PasswordResetAnyoneCommand().ExecuteAsync(session, token),
                ConsoleKey.D5 => new ChangeRoleCommand().ExecuteAsync(session, token),
                _ => Task.FromResult<bool>(true),
            };

            await task;

            Console.WriteLine();
            Console.WriteLine("*** finished ***");
            Console.WriteLine();


            return key.Key == ConsoleKey.Q;
        }
    }
}
