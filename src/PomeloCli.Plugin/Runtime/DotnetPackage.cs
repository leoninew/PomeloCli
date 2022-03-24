using System;

namespace PomeloCli.Plugin.Runtime {
    class DotnetPackage {
        public String Name { get; private set; }
        public String Version { get; private set; }
        public String Source { get; private set; }

        public DotnetPackage(String name, String version, String source) {
            Name = name;
            Version = version;
            Source = source;
        }
    }
}