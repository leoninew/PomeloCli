using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using McMaster.Extensions.CommandLineUtils;

namespace PomeloCli.Plugins.Runtime;

class Preferences
{
    public String AssemblyDir { get; private set; }
    public String PluginsCsprojFile { get; private set; }
    public String PackageDir { get; private set; }
    public String NugetConfigFile { get; private set; }

    public Preferences(string assemblyDir, string pluginCsprojFile, string packageDir, string nugetConfigFile)
    {
        AssemblyDir = assemblyDir;
        PluginsCsprojFile = pluginCsprojFile;
        PackageDir = packageDir;
        NugetConfigFile = nugetConfigFile;
    }

    public IEnumerable<Package> ResolvePackages()
    {
        if (Directory.Exists(AssemblyDir) == false)
        {
            yield break;
        }
        if (File.Exists(PluginsCsprojFile) == false)
        {
            yield break;
        }

        var document = XDocument.Load(PluginsCsprojFile);
        Debug.Assert(document != null);
        Debug.Assert(document.Root != null);

        // TODO: handle multi <ItemGroup> elements
        var packageReferenceGroup = document.Root.Elements()
            .FirstOrDefault(x => x.Name == "ItemGroup" && x.Element("None") == null);

        if (packageReferenceGroup != null)
        {
            var packageReferences = packageReferenceGroup.Elements("PackageReference");
            foreach (var item in packageReferences)
            {
                var name = item.Attribute("Include")?.Value;
                Debug.Assert(name != null);
                var version = item.Attribute("Version")?.Value;
                Debug.Assert(version != null);
                yield return new Package(name, version);
            }
        }

        var nupkgReferenceGroup = document.Root.Elements()
            .FirstOrDefault(x => x.Name == "ItemGroup" && x.Element("None") != null);
        if (nupkgReferenceGroup != null)
        {
            var nupkgReferences = nupkgReferenceGroup.Elements("None");
            foreach (var item in nupkgReferences)
            {
                var name = item.Attribute("Include")?.Value;
                Debug.Assert(name != null);
                var version = item.Attribute("Version")?.Value;
                Debug.Assert(version != null);
                yield return new Package(name, version);
            }
        }
    }


    public async Task InitializeAsync()
    {
        if (Directory.Exists(AssemblyDir) == false)
        {
            Directory.CreateDirectory(AssemblyDir);
        }
        if (File.Exists(PluginsCsprojFile) == false)
        {
            await File.WriteAllTextAsync(PluginsCsprojFile, Resources.PluginsCsproj);
        }
        if (File.Exists(NugetConfigFile) == false)
        {
            await File.WriteAllTextAsync(NugetConfigFile, Resources.NugetConfig);
        }

        var hasInitialized = Directory.Exists(Path.Combine(AssemblyDir, "obj")) && Directory.Exists(Path.Combine(AssemblyDir, "nuget"));
        if (hasInitialized == false)
        {
            await Console.Out.WriteLineAsync("  Preparing the necessary dependencies...");
#if NETCOREAPP3_1
            // dotnet add package Microsoft.Extensions.DependencyInjection -v 6.0.1
            AddDotnetPackage("Microsoft.Extensions.DependencyInjection", "6.0.1", true);
#else
            // dotnet add package Microsoft.Extensions.DependencyInjection -v 8.0.0
            AddDotnetPackage("Microsoft.Extensions.DependencyInjection", "8.0.0", true);
#endif
            // dotnet add package McMaster.Extensions.CommandLineUtils -v 4.1.1
            AddDotnetPackage("McMaster.Extensions.CommandLineUtils", "4.1.1", true);

            await Console.Out.WriteLineAsync("  Cleaning up workspace...");
            // dotnet remove package Microsoft.Extensions.DependencyInjection
            RemoveDotnetPackage("Microsoft.Extensions.DependencyInjection", true);
            // dotnet remove package McMaster.Extensions.CommandLineUtils
            RemoveDotnetPackage("McMaster.Extensions.CommandLineUtils", true);

            CommandExecutor.Start("dotnet", ArgumentEscaper.EscapeAndConcatenate(new[] { "restore" }),
                workingDir: AssemblyDir, quiet: false);
        }
    }

    private void AddDotnetPackage(String name, String version, Boolean quiet)
    {
        CommandExecutor.Start("dotnet", ArgumentEscaper.EscapeAndConcatenate(new[] { "add", "package", name, "-v", version }),
            workingDir: AssemblyDir, quiet: quiet);
    }

    private void RemoveDotnetPackage(String name, Boolean quiet)
    {
        CommandExecutor.Start("dotnet", ArgumentEscaper.EscapeAndConcatenate(new[] { "remove", "package", name }),
            workingDir: AssemblyDir, quiet: quiet);
    }

    // for manually operate
    public void AddNupkgPackage(String name, String version, params KeyValuePair<String, String>[] others)
    {
        var document = XDocument.Load(PluginsCsprojFile);
        Debug.Assert(document != null);
        Debug.Assert(document.Root != null);

        var nupkgReferenceGroup = document.Root.Elements()
            .FirstOrDefault(x => x.Name == "ItemGroup" && x.Element("None") != null);
        if (nupkgReferenceGroup == null)
        {
            nupkgReferenceGroup = new XElement("ItemGroup");
            document.Root.Add(nupkgReferenceGroup);
        }

        // <None Include="sampleplugin" Version="1.0.0" nupkg="....\SamplePlugin.1.0.0.nupkg" />
        var nupkgReference = nupkgReferenceGroup.Elements("None")
            .Where(x => x.Attribute("Include")?.Value == name)
            .FirstOrDefault();
        if (nupkgReference != null)
        {
            nupkgReference.SetAttributeValue("Version", version);
        }
        else
        {
            nupkgReference = new XElement("None",
                new XAttribute("Include", name),
                new XAttribute("Version", version));
            nupkgReferenceGroup.Add(nupkgReference);
        }

        foreach (var item in others)
        {
            nupkgReference.SetAttributeValue(item.Key, item.Value);
        }

        document.Save(PluginsCsprojFile);
    }
}