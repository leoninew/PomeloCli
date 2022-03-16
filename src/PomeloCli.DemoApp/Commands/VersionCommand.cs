using System;
using PomeloCli.Attributes;
using PomeloCli.Commands;
using PomeloCli.DemoApp.Diagnosis;

namespace PomeloCli.DemoApp.Commands {
    [Command("version")]
    class VersionCommand : Command {
        public override int Execute() {
            var version = VersionHelper.GetCurrentVersion();
            Console.WriteLine(version);
            return 0;
        }
    }
}