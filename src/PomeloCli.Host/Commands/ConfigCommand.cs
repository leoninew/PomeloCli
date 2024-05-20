using System;
using System.Threading;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;

namespace PomeloCli.Host.Commands;

[Command(Name = "config")]
class ConfigCommand : Command
{
    protected override Task<int> OnExecuteAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(0);
    }
}