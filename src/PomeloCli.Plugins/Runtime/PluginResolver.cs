using System;
using System.IO;
using System.Linq;
using System.Reflection;
using McMaster.NETCore.Plugins;
using Microsoft.Extensions.DependencyInjection;

namespace PomeloCli.Plugins.Runtime;

public interface IPluginResolver
{
    Boolean Loading(Package package, IServiceCollection services);
}

class PluginResolver : IPluginResolver
{
    private readonly IPackageManager _packageManager;

    public PluginResolver(IPackageManager packageManager)
    {
        _packageManager = packageManager;
    }

    public Boolean Loading(Package package, IServiceCollection services)
    {
        var preferences = _packageManager.GetPreferences();
        var assemblyPath = package.Locate(preferences.PackageDir);
        if (assemblyPath == null || File.Exists(assemblyPath) == false)
        {
            return false;
        }

        // https://github.com/natemcmaster/DotNetCorePlugins/blob/main/docs/what-are-shared-types.md
        var pluginLoader = PluginLoader.CreateFromAssemblyFile(assemblyPath,
            // new[] { typeof(ICommand), typeof(Command), typeof(Command<>) }); //,
            config => config.PreferSharedTypes = true);
        var assembly = pluginLoader.LoadDefaultAssembly();
        var extensions = assembly.GetExportedTypes()
            .Where(x => x.Name == "ServiceCollectionExtensions");

        var invoked = false;
        foreach (var extension in extensions)
        {
            var method = extension.GetMethod("AddCommands", BindingFlags.Public | BindingFlags.Static);
            if (method != null)
            {
                method.Invoke(null, new Object[] { services });
                invoked = true;
            }
        }

        return invoked;
    }
}