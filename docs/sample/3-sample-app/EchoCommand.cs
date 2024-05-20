#nullable disable
using System;
using System.Threading;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using PomeloCli;

[Command("echo", Description = "display a line of text")]
class EchoCommand : Command
{
    [Argument(0, "input")]
    public String Input { get; set; }

    [Option("-n|--newline", CommandOptionType.NoValue, Description = "do not output the trailing newline")]
    public Boolean? Newline { get; set; }

    protected override Task<int> OnExecuteAsync(CancellationToken cancellationToken)
    {
        if (Newline.HasValue)
        {
            Console.WriteLine(Input);
        }
        else
        {
            Console.Write(Input);
        }
        return Task.FromResult(0);
    }
}
