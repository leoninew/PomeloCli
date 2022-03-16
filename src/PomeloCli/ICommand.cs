using System;
using System.Threading.Tasks;
using Microsoft.Extensions.CommandLineUtils;

namespace PomeloCli {
    public interface ICommand {
        void Configure(CommandLineApplication cmdApp);
        Int32 Execute();
    }
}