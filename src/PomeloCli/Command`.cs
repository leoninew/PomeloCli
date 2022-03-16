using System;

namespace PomeloCli {
    public abstract class Command<T> : Command where T : Command {
    }
}