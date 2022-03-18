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

        public override int Execute() {
            Console.WriteLine("docker ps");
            return 0;
        }
    }
    
    [Command("images", Description = "List images")]
    class DockerImagesCommand: Command<DockerCommand> {
        [CommandOption("-f|--filter", CommandOptionType.SingleValue,
            Description = "Filter output based on conditions provided")]
        public String Filter { get; set; }

        public override int Execute() {
            Console.WriteLine("docker images");
            return 0;
        }
    }
    
    [Command("logs", Description = "Fetch the logs of a container")]
    class DockerLogsCommand: Command<DockerCommand> {
        [CommandArgument("container", false)]
        public String Container { get; set; }

        public override int Execute() {
            Console.WriteLine("docker logs");
            return 0;
        }
    }
}