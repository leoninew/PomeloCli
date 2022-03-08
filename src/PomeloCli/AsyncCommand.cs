using System;
using System.Threading.Tasks;

namespace PomeloCli {
    public abstract class AsyncCommand : Command {
        public Task<Int32> ExecuteAsync() {
            BindingParameters();
            return OnExecuteAsync();
        }

        protected virtual Task<Int32> OnExecuteAsync() {
            return Task.FromResult(0);
        }
    }
}