using System;

namespace PomeloCli {
    public abstract class Command<T> : Command where T : Command {
        protected override Int32 OnExecute() {
            return 0;
        }
    }
}