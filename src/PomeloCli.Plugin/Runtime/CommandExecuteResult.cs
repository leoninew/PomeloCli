using System;
using System.Collections.Generic;

namespace PomeloCli.Plugin.Runtime {
    class CommandExecuteResult {
        public CommandExecuteResult(IEnumerable<String> lines) {
            Lines = lines;
        }

        public CommandExecuteResult(IEnumerable<String> lines, Int32 exitCode)
            : this(lines) {
            ExitCode = exitCode;
        }

        public Int32 ExitCode { get; }
        public IEnumerable<String> Lines { get; }
    }
}