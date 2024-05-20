#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using PomeloCli.Plugins.Runtime;

namespace PomeloCli.Plugins.Commands;

[Command("uninstall")]
class PluginUninstallCommand : Command<PluginCommand>
{
    private readonly IPackageManager _packageManager;

    public PluginUninstallCommand(IPackageManager packageManager)
    {
        _packageManager = packageManager;
    }

    [Required]
    [Argument(0, "name",
        Description = "The package reference to add.")]
    public String Name { get; set; }

    protected override async Task<int> OnExecuteAsync(CancellationToken cancellationToken)
    {
        var preferences = _packageManager.GetPreferences();
        await preferences.InitializeAsync();

        var args = new List<String> { "remove", "package", Name };
#if DEBUG
        Console.WriteLine("dotnet {0}", ArgumentEscaper.EscapeAndConcatenate(args));
#endif
        CommandExecutor.Start("dotnet", ArgumentEscaper.EscapeAndConcatenate(args),
            workingDir: preferences.AssemblyDir, quiet: false);
        return 0;
    }
}