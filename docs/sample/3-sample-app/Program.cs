using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using PomeloCli;

class Program
{
    static async Task<int> Main(string[] args)
    {
        var services = new ServiceCollection()
            .AddTransient<ICommand, EchoCommand>()
            .AddTransient<ICommand, HeadCommand>()
            .BuildServiceProvider();

        var application = ApplicationFactory.ConstructFrom(services);
        return await application.ExecuteAsync(args);
    }
}
