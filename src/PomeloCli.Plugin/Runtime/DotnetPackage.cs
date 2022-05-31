using System;

namespace PomeloCli.Plugin.Runtime {
    class DotnetPackage {
        public DotnetPackage(String name, String version, String source) {
            Name = name;
            Version = version;
            Source = source;
        }

        public String Name { get; }
        public String Version { get; }
        public String Source { get; }
    }
}