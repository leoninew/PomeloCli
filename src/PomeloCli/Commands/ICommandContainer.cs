using System;
using Microsoft.Extensions.CommandLineUtils;

namespace PomeloCli.Commands {
    public interface ICommandContainer {
        CommandArgument GetArgument(String name);
        CommandOption GetOption(String template);
        void SetApplication(CommandLineApplication cmdApp);
    }
}