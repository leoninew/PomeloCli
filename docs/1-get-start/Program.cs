using System;
using PomeloCli;
using PomeloCli.Attributes;

namespace test
{
    class Program
    {
        static void Main(string[] args)
        {
            var commands = new[]{
                new EchoCommand()
            };
            new CommandService(commands)
                .ConfigureApplication()
                .Execute(args.Length > 0 ? args: new[] { "echo", "Hello PomeloCli" });
        }
    }

    [Command("echo")]
    class EchoCommand : Command
    {
        [CommandArgument("input", false)]
        public String Input { get; set; }

        public override int Execute()
        {
            Console.WriteLine(Input);
            return 0;
        }
    }
}