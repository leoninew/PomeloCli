using System;
using System.Collections.Generic;

namespace PomeloCli.Plugin {
    public interface IPluginProvider {
        String GetPluginDir(Boolean createIfNotExist);
        String GetPluginCsproj(Boolean createIfNotExist);
        IEnumerable<Plugin> FindAll();
        void Save(Plugin plugin);
    }

    public class Plugin {
        public String Name { get; set; }
        public String Version { get; set; }
        public String Source { get; set; }
        public String Assembly { get; set; }
    }
}