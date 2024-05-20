// Copyright (c) Nate McMaster.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Reflection;
using McMaster.Extensions.CommandLineUtils;

namespace SubcommandSample
{
    /// <summary>
    /// In this example, each sub command type inherits from <see cref="GitCommandBase"/>,
    /// which provides shared functionality between all the commands.
    /// This example also shows you how the subcommands can be linked to their parent types.
    /// </summary>
    [Command("fake-git")]
    [VersionOptionFromMember("--version", MemberName = nameof(GetVersion))]
    [Subcommand(
        typeof(GitAddCommand),
        typeof(GitCommitCommand))]
    class GitCommand : GitCommandBase
    {

        [Option("-C <path>")]
        [FileExists]
        public string ConfigFile { get; set; }

        [Option("-c <name>=<value>")]
        public string[] ConfigSetting { get; set; }

        [Option("--exec-path[:<path>]")]
        public (bool hasValue, string value) ExecPath { get; set; }

        [Option("--bare")]
        public bool Bare { get; }

        [Option("--git-dir=<path>")]
        [DirectoryExists]
        public string GitDir { get; set; }

        protected override int OnExecute(CommandLineApplication app)
        {
            // this shows help even if the --help option isn't specified
            app.ShowHelp();
            return 1;
        }

        public override List<string> CreateArgs()
        {
            var args = new List<string>();
            if (GitDir != null)
            {
                args.Add("--git-dir=" + GitDir);
            }

            return args;
        }

        private static string GetVersion()
            => typeof(GitCommand).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
    }
}
