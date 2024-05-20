using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using PomeloCli.Plugins.Commands;
using PomeloCli.Plugins.Runtime;

namespace PomeloCli.Plugins;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPluginSupport(this IServiceCollection services)
    {
        services.AddTransient<IPackageManager, PackageManager>();
        services.AddTransient<IPluginResolver, PluginResolver>();

        using var scope = services.Clone().BuildServiceProvider().CreateScope();
        var packageManager = scope.ServiceProvider.GetRequiredService<IPackageManager>();

        // TODO: skip load plugin if running 'plugin install' 
        var commandLineArgs = Environment.GetCommandLineArgs();
        var isInstallPlugin = commandLineArgs.Length > 2 && commandLineArgs[1] == "plugin" && commandLineArgs[2] == "install";
        if (isInstallPlugin == false)
        {
            var ignorePlugins = packageManager.IgnorePlugins();
            if (ignorePlugins == false)
            {
                var preferences = packageManager.GetPreferences();
                var sortedPackages = preferences.ResolvePackages()
                    .GroupBy(x => new { x.Name, x.Version })
                    .Select(g => g.OrderByDescending(x => Version.Parse(x.Version)).First());

                var pluginResolver = scope.ServiceProvider.GetRequiredService<IPluginResolver>();
                foreach (var package in sortedPackages)
                {
                    pluginResolver.Loading(package, services);
                }
            }
        }

        services.AddTransient<ICommand, PluginCommand>();
        services.AddTransient<ICommand, PluginListCommand>();
        services.AddTransient<ICommand, PluginInstallCommand>();
        services.AddTransient<ICommand, PluginUninstallCommand>();

        return services;
    }

    private static IServiceCollection Clone(this IServiceCollection services)
    {
        var clone = new ServiceCollection();
        foreach (var descriptor in services)
        {
            ((ICollection<ServiceDescriptor>)clone).Add(descriptor);
        }

        return clone;
    }
}