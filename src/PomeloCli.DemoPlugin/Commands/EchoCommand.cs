using System;
using System.Collections.Generic;
using Microsoft.Extensions.CommandLineUtils;
using PomeloCli.Commands;

namespace PomeloCli.DemoPlugin.Commands {
    class EchoCommand : Command {
        protected override CommandDeclaration DeclareCommand() {
            return new CommandDeclaration("echo", "display a line of text");
        }

        protected override IEnumerable<CommandArgument> DeclareArguments() {
            yield return new CommandArgument {
                Name = "input",
                MultipleValues = true
            };
        }

        protected override IEnumerable<CommandOption> DeclareOptions() {
            yield return new CommandOption("-n|--newline", CommandOptionType.NoValue) {
                Description = "do not output the trailing newline"
            };
        }

        protected override Int32 OnExecute() {
            var inputArg = CommandContainer.GetArgument("input");
            var newlineOption = CommandContainer.GetOption("-n|--newline");

            if (newlineOption.HasValue()) {
                Console.Write(String.Join(" ", inputArg.Values));
            }
            else {
                Console.WriteLine(String.Join(" ", inputArg.Values));
            }

            return 0;
        }
    }
}