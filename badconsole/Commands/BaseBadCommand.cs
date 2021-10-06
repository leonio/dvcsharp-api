using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Flurl;

public abstract class BaseBadCommand : IBadCommand
{
    public abstract Task<bool> ExecuteAsync(BadApiSession session, CancellationToken token = default);

    public void OnFailure(Dictionary<string, object> stateBag)
    {
        Console.WriteLine("OnFailure...");
    }
}
