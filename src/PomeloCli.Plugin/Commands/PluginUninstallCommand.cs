using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using PomeloCli.Attributes;
using PomeloCli.Commands;
using PomeloCli.Plugin.Native;

namespace PomeloCli.Plugin.Commands {
    [Command("uninstall", Parent = typeof(PluginCommand))]
    class PluginUninstallCommand : Command {
        private readonly IPluginProvider _pluginProvider;

        public PluginUninstallCommand(IPluginProvider pluginProvider) {
            _pluginProvider = pluginProvider;
        }

        [Required]
        [CommandArgument("name", false,
            Description = "The package reference to remove.")]
        public String Name { get; set; }

        protected override Int32 OnExecute() {
            var pluginCsproj = _pluginProvider.GetPluginCsproj(false);
            if (File.Exists(pluginCsproj) == false) {
                return 0;
            }

            var args = new List<String>();
            args.Add("remove");
            //args.Add(_pluginProvider.GetPluginDir());            
            args.Add("package");
            args.Add(Name);

            var dotnet = DotnetHelper.DetectExecutable();
            var pluginDir = _pluginProvider.GetPluginDir(false);
            var result = CommandRunner.Run(dotnet, args, true, pluginDir);
            Console.WriteLine(result.AllOutput);
            return result.ExitCode;
        }
    }
}