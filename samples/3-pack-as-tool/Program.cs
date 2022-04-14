using System;
using PomeloCli;
using PomeloCli.Attributes;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;

namespace test
{
    class Program
    {
        static void Main(string[] args)
        {
            var services = new ServiceCollection()
                .AddTransient<ICommand, DockerCommand>()
                .AddTransient<ICommand, DockerPsCommand>()
                .AddTransient<ICommandService, CommandService>()
                .BuildServiceProvider();

            var commandService = services.GetRequiredService<ICommandService>();
            commandService.ConfigureApplication()
                .Execute(args);
        }
    }

    [Command("docker")]
    class DockerCommand : Command
    {
    }

    [Command("ps")]
    class DockerPsCommand : Command<DockerCommand>
    {
        [CommandOption("-a|--all", CommandOptionType.NoValue, Description = "Show all containers (default shows just running)")]
        public Boolean All { get; set; }

        public override int Execute()
        {
            // Util.Cmd("docker", "ps"); //can execute under LINQPad
            Console.WriteLine("This is docker list command");
            return 0;
        }
    }
}
