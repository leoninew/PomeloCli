using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using PomeloCli;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// pomelo-cli load plugin by this method, see
    /// <see cref="PomeloCli.Plugins.Runtime.PluginResolver.Loading()" />
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddCommands(this IServiceCollection services)
    {
        return services
            .AddTransient<ICommand, EchoCommand>()
            .AddTransient<ICommand, HeadCommand>();
    }
}