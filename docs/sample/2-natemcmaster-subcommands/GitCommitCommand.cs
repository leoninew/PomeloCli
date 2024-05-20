// Copyright (c) Nate McMaster.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using McMaster.Extensions.CommandLineUtils;

namespace SubcommandSample
{
    [Command(Description = "Record changes to the repository")]
    class GitCommitCommand : GitCommandBase
    {
        [Option("-m")]
        public string Message { get; set; }

        // This will automatically be set before OnExecute is invoked.
        private GitCommand Parent { get; set; }

        public override List<string> CreateArgs()
        {
            var args = Parent.CreateArgs();
            args.Add("commit");

            if (Message != null)
            {
                args.Add("-m");
                args.Add(Message);
            }

            return args;
        }
    }
}
