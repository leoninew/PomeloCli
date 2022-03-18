using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.Options;
using PomeloCli.Attributes;
using PomeloCli.Plugin.Native;

namespace PomeloCli.Plugin.Commands {
    [Command("install")]
    class PluginInstallCommand : Command<PluginCommand> {
        private readonly IOptions<PluginOptions> _pluginOptions;
        private readonly IPluginProvider _pluginProvider;

        public PluginInstallCommand(IOptions<PluginOptions> pluginOptions, IPluginProvider pluginProvider) {
            _pluginProvider = pluginProvider;
            _pluginOptions = pluginOptions;
        }

        [Required]
        [CommandArgument("name", false,
            Description = "The package reference to add.")]
        public String Name { get; set; }

        [CommandOption("-v|--version", CommandOptionType.SingleValue,
            Description = "The version of the package to add.")]
        public String Version { get; set; }

        [CommandOption("-s|--source", CommandOptionType.SingleValue,
            Description = "The NuGet package source to use during the restore.")]
        public String Source { get; set; }

        [CommandOption("-a|--assembly", CommandOptionType.SingleValue)]
        public String Assembly { get; set; }

        public override Int32 Execute() {
            var args = new List<String>();
            args.Add("add");
            args.Add("package");
            args.Add(Name);

            if (Version != null) {
                args.Add("-v");
                args.Add(Version);
            }

            if (Source != null) {
                args.Add("-s");
                args.Add(Source);
            }

            args.Add("--package-directory");
            args.Add(_pluginOptions.Value.GetPackageDir());

            _pluginProvider.GetPluginCsproj(true); // ensure initialized
            var pluginDir = _pluginProvider.GetPluginDir(false);

            var dotnet = DotnetHelper.DetectExecutable();
            var result = CommandRunner.Run(dotnet, args, true, pluginDir);
            Console.WriteLine(result.AllOutput);

            if (result.ExitCode == 0) {
                _pluginProvider.Save(new Plugin {
                    Name = Name,
                    Version = Version,
                    Source = Source,
                    Assembly = Assembly
                });
            }

            return result.ExitCode;
        }
    }
}