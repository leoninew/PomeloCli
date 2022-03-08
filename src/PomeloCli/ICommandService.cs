using Microsoft.Extensions.CommandLineUtils;

namespace PomeloCli {
    public interface ICommandService {
        CommandLineApplication ConfigureApplication();
    }
}