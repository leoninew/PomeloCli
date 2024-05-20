// Copyright (c) Nate McMaster.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using McMaster.Extensions.CommandLineUtils;

namespace SubcommandSample
{
    [Command(Description = "Add file contents to the index")]
    class GitAddCommand : GitCommandBase
    {
        [Argument(0)]
        [LegalFilePath]
        public string[] Files { get; set; }

        // You can use this pattern when the parent command may have options or methods you want to
        // use from sub-commands.
        // This will automatically be set before OnExecute is invoked
        private GitCommand Parent { get; set; }

        protected override int OnExecute(CommandLineApplication app)
        {
            app.ShowHelp();
            return base.OnExecute(app);
        }

        public override List<string> CreateArgs()
        {
            var args = Parent.CreateArgs();
            args.Add("add");

            if (Files != null)
            {
                args.AddRange(Files);
            }

            return args;
        }
    }
}
