using System.Threading;
using System.Threading.Tasks;

namespace PomeloCli;

public interface ICommand
{
    Task<int> ExecuteAsync(CancellationToken cancellationToken);
}