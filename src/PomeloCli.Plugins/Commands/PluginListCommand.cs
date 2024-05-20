#nullable disable
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using PomeloCli.Plugins.Runtime;

namespace PomeloCli.Plugins.Commands;

[Command("list")]
class PluginListCommand : Command<PluginCommand>
{
    private readonly IPackageManager _packageManager;

    public PluginListCommand(IPackageManager packageManager)
    {
        _packageManager = packageManager;
    }

    protected override async Task<int> OnExecuteAsync(CancellationToken cancellationToken)
    {
        var preferences = _packageManager.GetPreferences();
        await preferences.InitializeAsync();

        var args = new List<String> { "list", "package" };
#if DEBUG
        Console.WriteLine("dotnet {0}", ArgumentEscaper.EscapeAndConcatenate(args));
#endif
        CommandExecutor.Start("dotnet", ArgumentEscaper.EscapeAndConcatenate(args),
            workingDir: preferences.AssemblyDir, quiet: false);
        return 0;
    }
}