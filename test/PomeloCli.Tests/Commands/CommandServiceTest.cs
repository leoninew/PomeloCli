using System.Linq;
using System.Reflection;
using Microsoft.Extensions.CommandLineUtils;
using PomeloCli.Attributes;
using Xunit;

namespace PomeloCli.Tests.Commands {
    public class CommandServiceTest {
        [Fact]
        public void ConfigureApplication_ApplyAdvancedCommand_SetupEntrypoint() {
            var commands = new ICommand[] {
                new EchoCommand()
            };
            var commandService = new CommandService(commands);
            var commandApplication = commandService.ConfigureApplication();

            var commandApp = AssertCommand(commandApplication);
            AssertCommandArguments(commandApp);
            AssertCommandOptions(commandApp);
        }

        private void AssertCommandOptions(CommandLineApplication commandApplication) {
            var props = typeof(EchoCommand).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Select(x => x.GetCustomAttribute<CommandOptionAttribute>())
                .Where(x => x != null)
                .ToArray();
            var commandOptionAttr = Assert.Single(props);
            Assert.NotNull(commandOptionAttr);

            //exclude help option 
            var commandOption = Assert.Single(commandApplication.Options.Where(x => x.LongName != "help"));
            Assert.NotNull(commandOption);

            Assert.Equal(commandOption.Template, commandOptionAttr.Template);
            Assert.Equal(commandOption.OptionType, commandOptionAttr.OptionType);
            Assert.Equal(commandOption.Description, commandOptionAttr.Description);
        }

        private static void AssertCommandArguments(CommandLineApplication commandApplication) {
            var props = typeof(EchoCommand).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Select(x => x.GetCustomAttribute<CommandArgumentAttribute>())
                .Where(x => x != null)
                .ToArray();
            var commandArgumentAttr = Assert.Single(props);
            Assert.NotNull(commandArgumentAttr);

            var commandArgument = Assert.Single(commandApplication.Arguments);
            Assert.NotNull(commandArgument);

            Assert.Equal(commandArgument.Name, commandArgumentAttr.Name);
            Assert.Equal(commandArgument.MultipleValues, commandArgumentAttr.MultipleValues);
            Assert.Equal(commandArgument.Description, commandArgumentAttr.Description);
        }

        private static CommandLineApplication AssertCommand(CommandLineApplication commandApplication) {
            var commandApp = Assert.Single(commandApplication.Commands);
            Assert.NotNull(commandApp);

            var commandAttr = typeof(EchoCommand).GetCustomAttribute<CommandAttribute>();
            Assert.NotNull(commandAttr);

            Assert.Equal(commandApp.Parent, commandApplication);
            Assert.Equal(commandApp.Name, commandAttr.Name);
            Assert.Equal(commandApp.Description, commandAttr.Description);
            return commandApp;
        }
    }
}