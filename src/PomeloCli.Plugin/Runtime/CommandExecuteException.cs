using System;

namespace PomeloCli.Plugin {
    public class CommandExecuteException : Exception {
        public int ExitCode { get; private set; }

        public CommandExecuteException(string msg, int exitCode)
            : base(msg ?? "The process returned an exit code of " + exitCode) {
            ExitCode = exitCode;
        }
    }
}