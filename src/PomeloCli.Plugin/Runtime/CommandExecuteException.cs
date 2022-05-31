using System;

namespace PomeloCli.Plugin {
    public class CommandExecuteException : Exception {
        public CommandExecuteException(String msg, Int32 exitCode)
            : base(msg ?? "The process returned an exit code of " + exitCode) {
            ExitCode = exitCode;
        }

        public Int32 ExitCode { get; }
    }
}