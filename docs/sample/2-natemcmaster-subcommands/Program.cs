#nullable disable
using System;
using McMaster.Extensions.CommandLineUtils;
using SubcommandSample;

public class Program
{
    private static int Main(string[] args)
    {
        return CommandLineApplication.Execute<GitCommand>(args);
    }
}
