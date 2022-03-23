using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PomeloCli.Attributes;
using PomeloCli.Plugin.Native;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.Options;

namespace PomeloCli.Plugin.Commands {
    [Command("migrate")]
    class PluginMigrateCommand : Command<PluginCommand> {
        private readonly IOptions<PluginOptions> _pluginOptions;
        private readonly IPluginProvider _pluginProvider;

        public PluginMigrateCommand(IPluginProvider pluginProvider, IOptions<PluginOptions> pluginOptions) {
            _pluginProvider = pluginProvider;
            _pluginOptions = pluginOptions;
        }

        [CommandOption("-v|--version", CommandOptionType.SingleValue,
            Description = "The version of the package to add.")]
        public String Version { get; set; }
        
        public override Int32 Execute() {
            var pluginCsproj = _pluginProvider.GetPluginCsproj(false);
            if (File.Exists(pluginCsproj) == false) {
                return 0;
            }

            var plugins = _pluginProvider.FindAll().ToArray();
            var dotnet = DotnetHelper.DetectExecutable();
            foreach (var plugin in plugins) {
                
                var args = new List<String>();
                args.Add("add");
                args.Add("package");
                args.Add(plugin.Name);

                if (Version != null) {
                    args.Add("-v");
                    args.Add(Version);
                }

                if (plugin.Source != null) {
                    args.Add("-s");
                    args.Add(plugin.Source);
                }

                args.Add("--package-directory");
                args.Add(_pluginOptions.Value.GetPackageDir());

                var pluginDir = _pluginProvider.GetPluginDir(false);
                var result = CommandRunner.Run(dotnet, args, true, pluginDir);
                Console.WriteLine(result.AllOutput);
                
                if (result.ExitCode == 0) {
                    _pluginProvider.Save(new Plugin {
                        Name = plugin.Name,
                        Version = Version,
                        Source = plugin.Source,
                        Assembly = plugin.Assembly
                    });
                }
            }
            return 0;
        }
    }
}