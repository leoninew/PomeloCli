using System;

namespace PomeloCli.Commands {
    public class CommandDeclaration {
        public CommandDeclaration(String name, String description = null) {
            Name = name;
            Description = description;
        }

        public String Name { get; private set; }
        public String Description { get; private set; }
    }
}