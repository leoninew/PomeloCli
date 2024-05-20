using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using PomeloCli.Plugins;
using PomeloCli.Host.Commands;

namespace PomeloCli.Host;

class Program
{
    static async Task<int> Main(string[] args)
    {
        var services = new ServiceCollection()
            .AddTransient<ICommand, ConfigCommand>()
            .AddTransient<ICommand, VersionCommand>()
            .AddPluginSupport()
            .BuildServiceProvider();

        var applicationFactory = new ApplicationFactory(services);
        var application = applicationFactory.ConstructRootApp();

        return await application.ExecuteAsync(args);
    }
}