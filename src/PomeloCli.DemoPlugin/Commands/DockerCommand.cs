using System;
using Microsoft.Extensions.CommandLineUtils;
using PomeloCli.Attributes;

namespace PomeloCli.DemoPlugin.Commands {
    [Command("docker")]
    class DockerCommand: Command {
    }
    
    [Command("ps", Description = "List containers")]
    class DockerPsCommand: Command<DockerCommand> {
        [CommandOption("-a|--all", CommandOptionType.NoValue,
            Description = "Show all containers (default shows just running)")]
        public Boolean All { get; set; }
        
        protected override Int32 OnExecute() {
            return base.OnExecute();
        }
    }
    
    [Command("images", Description = "List images")]
    class DockerImagesCommand: Command<DockerCommand> {
        [CommandOption("-f|--filter", CommandOptionType.SingleValue,
            Description = "Filter output based on conditions provided")]
        public String Filter { get; set; }
        
        protected override Int32 OnExecute() {
            return base.OnExecute();
        }
    }
    
    [Command("logs", Description = "Fetch the logs of a container")]
    class DockerLogsCommand: Command<DockerCommand> {
        [CommandArgument("container", false)]
        public String Container { get; set; }
        
        protected override Int32 OnExecute() {
            return base.OnExecute();
        }
    }
}