using System;
using System.Threading;
using System.Threading.Tasks;

namespace PomeloCli;

public abstract class Command : ICommand
{
    public Task<Int32> ExecuteAsync(CancellationToken cancellationToken)
    {
        return OnExecuteAsync(cancellationToken);
    }

    protected virtual Task<Int32> OnExecuteAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(0);
    }
}