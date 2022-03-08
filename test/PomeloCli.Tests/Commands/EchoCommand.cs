using System;
using Microsoft.Extensions.CommandLineUtils;
using PomeloCli.Attributes;
using PomeloCli.Commands;

namespace PomeloCli.Tests.Commands {
    [Command("echo")]
    class EchoCommand : Command {
        [CommandArgument("string", false)]
        public String Input { get; set; }

        [CommandOption("-n|--newline", CommandOptionType.NoValue, Description = "do not output the trailing newline")]
        public Boolean Newline { get; set; }

        protected override int OnExecute() {
            if (Newline) {
                Console.Write(Input);
            }
            else {
                Console.WriteLine(Input);
            }

            return 0;
        }
    }
}