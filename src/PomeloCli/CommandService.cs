using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.CommandLineUtils;

namespace PomeloCli {
    public class CommandService : ICommandService {
        private readonly IEnumerable<ICommand> _commands;

        public CommandService(IEnumerable<ICommand> commands) {
            _commands = commands;
        }

        public CommandLineApplication ConfigureApplication() {
            var application = new CommandLineApplication();
            application.HelpOption("-?|-h|--help");

            var rootCommands = GetRootCommands();
            foreach (var command in rootCommands) {
                // CommandLineApplication.Name must not null,
                // but could assigned in Action<CommandLineApplication>
                var commandName = command.GetType().FullName;
                application.Command(commandName, cmdApp => ConfigureCommand(command, cmdApp));
            }

            return application;
        }

        private void ConfigureCommand(ICommand command, CommandLineApplication cmdApp) {
            command.Configure(cmdApp);
            var childCommands = GetChildCommands(command);
            foreach (var childCommand in childCommands) {
                var commandName = childCommands.GetType().FullName;
                cmdApp.Command(commandName, childCmdApp => ConfigureCommand(childCommand, childCmdApp));
            }

            if (command is AsyncCommand asyncCommand) {
                cmdApp.OnExecute(asyncCommand.ExecuteAsync);
            }
            else {
                cmdApp.OnExecute(command.Execute);
            }
        }

        private IEnumerable<ICommand> GetRootCommands() {
            return _commands.Where(x => x is Command == false
                || x is Command cmd && cmd.DeclareParent() == null);
        }

        private IEnumerable<ICommand> GetChildCommands(ICommand command) {
            var parentType = command.GetType();
            return _commands.Where(x => x is Command cmd && cmd.DeclareParent() == parentType);
        }
    }
}