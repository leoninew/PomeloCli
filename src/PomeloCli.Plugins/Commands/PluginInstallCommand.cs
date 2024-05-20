#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using PomeloCli.Plugins.Runtime;

namespace PomeloCli.Plugins.Commands;

[Command("install")]
class PluginInstallCommand : Command<PluginCommand>
{
    private readonly IPackageManager _packageManager;

    [Required]
    [Argument(0, "name",
        Description = "The package reference to add.")]
    public String Name { get; set; }

    [Required]
    [RegularExpression("(\\d+\\.){1,4}.+")]
    [Option("-v|--version", CommandOptionType.SingleValue,
        Description = "The version of the package to add.")]
    public String Version { get; set; }

    [Option("-s|--source", CommandOptionType.SingleValue,
        Description = "The NuGet package source to use during the restore.")]
    public String Source { get; set; }

    [Option("-n|--nupkg", CommandOptionType.SingleValue)]
    public String Nupkg { get; set; }

    public PluginInstallCommand(IPackageManager packageManager)
    {
        _packageManager = packageManager;
    }

    protected override async Task<int> OnExecuteAsync(CancellationToken cancellationToken)
    {
        var preferences = _packageManager.GetPreferences();
        await preferences.InitializeAsync();

        if (String.IsNullOrWhiteSpace(Nupkg))
        {
            var args = new List<String>
            {
                "add",
                "package",
                Name,
                "-v",
                Version,
                "-f",
                TargetFrameworkMoniker.Current(),
            };

            if (Source != null)
            {
                args.Add("-s");
                args.Add(Source);
            }
            // --package-directory 由 nuget.config 设置
#if DEBUG
            Console.WriteLine("dotnet {0}", ArgumentEscaper.EscapeAndConcatenate(args));
#endif
            CommandExecutor.Start("dotnet", ArgumentEscaper.EscapeAndConcatenate(args),
                workingDir: preferences.AssemblyDir, quiet: false);
        }
        else
        {
            var nupkgFile = Path.IsPathRooted(Nupkg) ? Nupkg : Path.Combine(Directory.GetCurrentDirectory(), Nupkg);
            if (File.Exists(nupkgFile) == false)
            {
                throw new FileNotFoundException($"file '{Nupkg}' not found");
            }

            var packageDir = Path.Combine(preferences.PackageDir, Name.ToLower(), Version);
            if (Directory.Exists(packageDir) == false)
            {
                Directory.CreateDirectory(packageDir);
            }

            await Console.Out.WriteLineAsync("  Unpacking the specified package...");
            ZipFile.ExtractToDirectory(nupkgFile, packageDir, Encoding.UTF8, true);

            await Console.Out.WriteLineAsync("  Saving dependency information...");
            preferences.AddNupkgPackage(Name, Version, new KeyValuePair<string, string>("nupkg", nupkgFile));
        }
        return 0;
    }
}