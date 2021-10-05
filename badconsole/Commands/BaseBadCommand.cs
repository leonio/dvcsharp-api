using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Flurl;

public abstract class BaseBadCommand : IBadCommand
{
    public HttpMethod Method => HttpMethod.Get;

    public object CommandParam => null;

    public abstract Task<bool> ExecuteAsync(Url baseUrl, BadApiSession session, CancellationToken token = default);

    public void OnFailure(Dictionary<string, object> stateBag)
    {
        Console.WriteLine("OnFailure...");
    }
}
