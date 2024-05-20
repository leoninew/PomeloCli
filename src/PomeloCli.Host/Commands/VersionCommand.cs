using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;

namespace PomeloCli.Host.Commands;

[Command(Name = "version")]
class VersionCommand : Command
{
    protected override Task<int> OnExecuteAsync(CancellationToken cancellationToken)
    {
        var informationalVersion = Assembly.GetExecutingAssembly()
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            ?.InformationalVersion;
        var runtimeVersion = Environment.Version;

        Console.WriteLine("application version: {0}", informationalVersion);
        Console.WriteLine("dotnet version     : {0}", runtimeVersion);
        return Task.FromResult(0);
    }
}