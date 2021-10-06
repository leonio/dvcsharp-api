using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public interface IBadCommand
{
    Task<bool> ExecuteAsync(BadApiSession session, CancellationToken token = default);

    void OnFailure(Dictionary<string, object> stateBag);
}