using System;
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
            var objectType = typeof(Object);
            var commandType = typeof(Command);
            return _commands.Where(x => {
                var type = x.GetType();
                return type.BaseType == objectType || type.BaseType == commandType;
            });
        }

        private IEnumerable<ICommand> GetChildCommands(ICommand command) {
            var commandType = typeof(Command<>).MakeGenericType(command.GetType());
            return _commands.Where(x => x.GetType().IsSubclassOf(commandType));
        }
    }
}