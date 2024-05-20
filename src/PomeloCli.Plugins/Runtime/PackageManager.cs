using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace PomeloCli.Plugins.Runtime;

interface IPackageManager
{
    Boolean IgnorePlugins();
    Preferences GetPreferences();
}

class PackageManager : IPackageManager
{
    public Boolean IgnorePlugins()
    {
        var assemblyName = GetAssemblyName();
        var variableName = $"{assemblyName.ToUpper()}_PLUGIN_IGNORE";
        var variableValue = Environment.GetEnvironmentVariable(variableName);
        return Boolean.TrueString.Equals(variableValue, StringComparison.OrdinalIgnoreCase);
    }

    public Preferences GetPreferences()
    {
        var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var assemblyDir = Path.Combine(userProfile, "." + GetAssemblyName().ToLower());
        var pluginCsprojFile = Path.Combine(assemblyDir, "Plugin.csproj");
        // TODO: make sure globalPackagesFolder in nuget.config is sample as packageDir bellow
        var packageDir = Path.Combine(assemblyDir, "nuget");
        var nugetConfigFile = Path.Combine(assemblyDir, "nuget.config");
        return new Preferences(assemblyDir, pluginCsprojFile, packageDir, nugetConfigFile);
    }

    private String GetAssemblyName()
    {
        var assembly = Assembly.GetEntryAssembly();
        Debug.Assert(assembly != null);
        var assemblyName = assembly.GetName().Name;
        Debug.Assert(assemblyName != null);
        return assemblyName;
    }
}