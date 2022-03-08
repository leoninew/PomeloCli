// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;

namespace PomeloCli.Plugin.Native {
    // https://github.com/NuGet/NuGet.Client/blob/dev/test/TestUtilities/Test.Utility/CommandRunnerResult.cs
    public class CommandRunnerResult {
        internal CommandRunnerResult(Process process, Int32 exitCode, String output, String error) {
            Process = process;
            ExitCode = exitCode;
            Output = output;
            Errors = error;
        }

        public Process Process { get; }

        /// <summary>
        ///     Item 1. Multi-purpose
        /// </summary>
        /// <remarks>
        ///     In occasions, it refers to Exit Status Code of the command execution result
        /// </remarks>
        public Int32 ExitCode { get; }

        /// <summary>
        ///     Item 2. Multi-purpose
        /// </summary>
        /// <remarks>
        ///     In occasions, it refers to the Standard Output of the command execution
        /// </remarks>
        public String Output { get; }

        /// <summary>
        ///     Item 3. Multi-purpose
        /// </summary>
        /// <remarks>
        ///     In occasions, it refers to the Standard Error of the command execution
        /// </remarks>
        public String Errors { get; }


        public Boolean Success => ExitCode == 0;

        /// <summary>
        ///     All output messages including errors
        /// </summary>
        public String AllOutput => Output + Environment.NewLine + Errors;
    }
}