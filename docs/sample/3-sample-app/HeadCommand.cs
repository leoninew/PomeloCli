#nullable disable
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using PomeloCli;

[Command("head", Description = "Print the first 10 lines of each FILE to standard output")]
class HeadCommand : Command
{
    [Required]
    [Argument(0)]
    public String Path { get; set; }

    [Option("-n|--line", CommandOptionType.SingleValue, Description = "print the first NUM lines instead of the first 10")]
    public Int32 Line { get; set; } = 10;

    protected override Task<int> OnExecuteAsync(CancellationToken cancellationToken)
    {
        if (!File.Exists(Path))
        {
            throw new FileNotFoundException($"file '{Path}' not found");
        }

        var lines = File.ReadLines(Path).Take(Line);
        foreach (var line in lines)
        {
            Console.WriteLine(line);
        }
        return Task.FromResult(0);
    }
}