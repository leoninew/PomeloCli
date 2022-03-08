// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace PomeloCli.Plugin.Native {
    // https://github.com/NuGet/NuGet.Client/blob/dev/test/TestUtilities/Test.Utility/CommandRunner.cs
    public class CommandRunner {
        // Item1 of the returned tuple is the exit code. Item2 is the standard output, and Item3
        // is the error output.
        private static String JoinArgs(IEnumerable<String> args) {
            var builder = new StringBuilder();
            foreach (var arg in args) {
                builder.Append(arg.IndexOf(' ') == -1 ? arg : $"\"{arg}\"");
                builder.Append(' ');
            }

            if (builder.Length > 0) {
                builder.Length -= 1;
            }

            return builder.ToString();
        }

        public static CommandRunnerResult Run(
            String process,
            IEnumerable<String> arguments,
            Boolean waitForExit,
            String workingDirectory = null,
            Int32 timeOutInMilliseconds = 60000,
            Action<StreamWriter> inputAction = null,
            IDictionary<String, String> environmentVariables = null) {
            var args = JoinArgs(arguments);
            // Console.WriteLine(args);
            return Run(process, args, waitForExit, workingDirectory, timeOutInMilliseconds, inputAction,
                environmentVariables);
        }

        public static CommandRunnerResult Run(
            String process,
            String arguments,
            Boolean waitForExit,
            String workingDirectory = null,
            Int32 timeOutInMilliseconds = 60000,
            Action<StreamWriter> inputAction = null,
            IDictionary<String, String> environmentVariables = null) {
            var psi = new ProcessStartInfo(Path.GetFullPath(process), arguments) {
                WorkingDirectory = Path.GetFullPath(workingDirectory ?? AppContext.BaseDirectory),
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = inputAction != null
            };

#if !IS_CORECLR
            psi.EnvironmentVariables["NuGetTestModeEnabled"] = "True";
#else
            psi.Environment["NuGetTestModeEnabled"] = "True";
#endif

            if (environmentVariables != null) {
                foreach (var pair in environmentVariables) {
#if !IS_CORECLR
                    psi.EnvironmentVariables[pair.Key] = pair.Value;
#else
                    psi.Environment[pair.Key] = pair.Value;
#endif
                }
            }

            var exitCode = 1;
            var output = new StringBuilder();
            var errors = new StringBuilder();
            Process p = null;

            using (p = new Process()) {
                p.OutputDataReceived += OutputHandler;
                p.ErrorDataReceived += ErrorHandler;

                p.StartInfo = psi;
                p.Start();

                inputAction?.Invoke(p.StandardInput);

                p.BeginOutputReadLine();
                p.BeginErrorReadLine();

                if (waitForExit) {
                    var processExited = p.WaitForExit(timeOutInMilliseconds);
                    if (processExited) {
                        p.WaitForExit();
                        exitCode = p.ExitCode;
                    }
                    else {
                        Kill(p);
                        WaitForExit(p);

                        var processName = Path.GetFileName(process);

                        throw new TimeoutException(
                            $"{processName} timed out: {psi.Arguments}{Environment.NewLine}Output:{output}{Environment.NewLine}Error:{errors}");
                    }
                }

                p.CancelOutputRead();
                p.CancelErrorRead();
            }

            void OutputHandler(Object sendingProcess, DataReceivedEventArgs e) {
                if (!String.IsNullOrEmpty(e.Data)) {
                    output.AppendLine(e.Data);
                }
            }

            void ErrorHandler(Object sendingProcess, DataReceivedEventArgs e) {
                if (!String.IsNullOrEmpty(e.Data)) {
                    errors.AppendLine(e.Data);
                }
            }

            return new CommandRunnerResult(p, exitCode, output.ToString(), errors.ToString());
        }

        private static void Kill(Process process) {
            try {
                process.Kill();
            }
            catch (InvalidOperationException) {
            }
            catch (Win32Exception) {
            }
        }

        private static void WaitForExit(Process process) {
            try {
                if (!process.HasExited) {
                    process.WaitForExit();
                }
            }
            catch (InvalidOperationException) {
            }
            catch (Win32Exception) {
            }
            catch (SystemException) {
            }
        }
    }
}