using System;
using Microsoft.Extensions.CommandLineUtils;

namespace PomeloCli.Commands {
    class CommandContainer : ICommandContainer {
        private CommandLineApplication _cmdApp;

        public CommandArgument GetArgument(String name) {
            return _cmdApp.Arguments.Find(x => x.Name == name);
        }

        public CommandOption GetOption(String template) {
            return _cmdApp.Options.Find(x => x.Template == template);
        }

        public void SetApplication(CommandLineApplication cmdApp) {
            _cmdApp = cmdApp;
        }
    }
}