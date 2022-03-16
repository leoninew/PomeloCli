using System;
using System.Threading.Tasks;

namespace PomeloCli {
    public interface IAsyncCommand: ICommand {
        Task<Int32> ExecuteAsync();
    }
}