using System;
using System.IO;

namespace PomeloCli.Plugins.Runtime;

public class Package
{
    public string Name { get; private set; }

    public string Version { get; private set; }

    public string? Source { get; private set; }

    public Package(string name, string version)
        : this(name, version, null)
    {

    }

    public Package(string name, string version, string? source)
    {
        Name = name;
        Version = version;
        Source = source;
    }

    public string? Locate(String packageDir)
    {
        var libraryDir = Path.Combine(packageDir, Name.ToLower(), Version.ToLower());
        if (Directory.Exists(libraryDir) == false)
        {
            return null;
        }


        var tfm = TargetFrameworkMoniker.Current();
        var tfmDir = Path.Combine(libraryDir, "lib", tfm);
        if (Directory.Exists(tfmDir) == false)
        {
            return null;
        }

        var assembly = Name + ".dll";
        var assemblyFile = Path.Combine(tfmDir, assembly);
        if (File.Exists(assemblyFile) == false)
        {
            return null;
        }

        return assemblyFile;
    }
}