using System.Collections.Generic;

namespace PomeloCli.Plugin.Runtime {
    class CommandExecuteResult {
        public int ExitCode { get; }
        public IEnumerable<string> Lines { get; }

        public CommandExecuteResult(IEnumerable<string> lines) {
            Lines = lines;
        }

        public CommandExecuteResult(IEnumerable<string> lines, int exitCode)
            : this(lines) {
            ExitCode = exitCode;
        }
    }
}