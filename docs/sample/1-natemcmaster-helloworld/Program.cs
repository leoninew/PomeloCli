using System;
using McMaster.Extensions.CommandLineUtils;

public class Program
{
    private static int Main(string[] args)
    {
        return CommandLineApplication.Execute<EchoCommand>(args);
    }
}

class EchoCommand
{
    [Argument(0)]
    public String Input { get; set; }

    [Option("-n|--newline", CommandOptionType.NoValue, Description = "do not output the trailing newline")]
    public Boolean? Newline { get; set; }

    public int OnExecute()
    {
        if (Newline.HasValue)
        {
            Console.WriteLine(Input);
        }
        else
        {
            Console.Write(Input);
        }
        return 0;
    }
}
