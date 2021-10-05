using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Flurl;

public interface IBadCommand
{
    HttpMethod Method { get; }

    object CommandParam { get; }

    Task<bool> ExecuteAsync(Url baseUrl, BadApiSession session, CancellationToken token = default);

    void OnFailure(Dictionary<string, object> stateBag);
}

public enum HttpMethod
{
    Get,
    Post,
    Put
}