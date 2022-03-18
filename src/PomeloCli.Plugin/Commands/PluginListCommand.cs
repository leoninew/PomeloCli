using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.CommandLineUtils;
using PomeloCli.Attributes;
using PomeloCli.Plugin.Native;

namespace PomeloCli.Plugin.Commands {
    [Command("list")]
    class PluginListCommand : Command<PluginCommand> {
        private readonly IPluginProvider _pluginProvider;

        public PluginListCommand(IPluginProvider pluginProvider) {
            _pluginProvider = pluginProvider;
        }

        [CommandOption("-s|--source", CommandOptionType.SingleValue,
            Description = "The NuGet package source to use during the restore.")]
        public String Source { get; set; }

        public override Int32 Execute() {
            var pluginCsproj = _pluginProvider.GetPluginCsproj(false);
            if (File.Exists(pluginCsproj) == false) {
                return 0;
            }

            var args = new List<String> {"list", "package"};
            if (Source != null) {
                args.Add("-s");
                args.Add(Source);
            }

            var dotnet = DotnetHelper.DetectExecutable();
            var pluginDir = _pluginProvider.GetPluginDir(false);
            var result = CommandRunner.Run(dotnet, args, true, pluginDir);
            Console.WriteLine(result.AllOutput);
            return result.ExitCode;
        }
    }
}