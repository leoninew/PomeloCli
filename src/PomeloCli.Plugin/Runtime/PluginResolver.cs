using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using McMaster.NETCore.Plugins;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;

namespace PomeloCli.Plugin.Runtime {
    class PluginResolver : IPluginResolver {
        private readonly ILogger _logger;
        private readonly IOptions<PluginOptions> _pluginOptions;

        public PluginResolver(ILogger<PluginResolver> logger,
            IOptions<PluginOptions> pluginOptions) {
            _logger = logger;
            _pluginOptions = pluginOptions;
        }

        public void Loading(Plugin plugin, IServiceCollection services, IConfiguration configuration) {
            var assemblyPath = GetPluginEntry(plugin);
            if (assemblyPath == null || File.Exists(assemblyPath) == false) {
                return;
            }

            var pluginLoader = PluginLoader.CreateFromAssemblyFile(assemblyPath,
                config => config.PreferSharedTypes = true);
            //sharedTypes: new[] { typeof(ICommand), typeof(IServiceCollection), typeof(ILogger<>) });

            var extensions = pluginLoader.LoadDefaultAssembly()
                .GetTypes()
                .Where(x => x.Name == "ServiceCollectionExtensions");

            var invoked = false;
            foreach (var extension in extensions) {
                var method = extension.GetMethod("ConfigureCommands",
                    BindingFlags.Public | BindingFlags.Static);
                if (method != null) {
                    method.Invoke(null, new Object[] {services});
                    invoked = true;
                }
            }

            if (invoked == false) {
                _logger.LogDebug("package {Name}:{Version} method 'ServiceCollectionExtensions.ConfigureCommands()' not found",
                    plugin.Name, plugin.Version);
            }
        }

        private String GetPluginEntry(Plugin plugin) {
            var packageDir = _pluginOptions.Value.GetPackageDir();
            if (packageDir == null || Directory.Exists(packageDir) == false) {
                _logger.LogDebug("package {Name}:{Version} package '{LibraryDir}' not found",
                    plugin.Name, plugin.Version, packageDir);
                return null;
            }
            
            var libraryDir = Path.Combine(packageDir, plugin.Name.ToLower(), plugin.Version.ToLower());
            if (Directory.Exists(libraryDir) == false) {
                _logger.LogDebug("package {Name}:{Version} library '{LibraryDir}' not found",
                    plugin.Name, plugin.Version, libraryDir);
                return null;
            }

#if NET5_0
            var tfm = "net5.0";
#else
            var tfm = "netcoreapp3.1";
#endif
            var tfmDir = Path.Combine(libraryDir, "lib", tfm);
            if (Directory.Exists(tfmDir) == false) {
                _logger.LogDebug("package {Name}:{Version} tfm '{TfmDir}' not found",
                    plugin.Name, plugin.Version, tfmDir);
                return null;
            }

            var assembly = plugin.Assembly ?? plugin.Name + ".dll";
            var assemblyFile = Path.Combine(tfmDir, assembly);
            if (File.Exists(assemblyFile)) {
                return assemblyFile;
            }

            _logger.LogDebug("package {Name}:{Version} dll '{AssemblyFile}' not found",
                plugin.Name, plugin.Version, assemblyFile);
            return null;
        }

        public async Task<Version> GetLatestAsync(String name) {
            var cache = new SourceCacheContext();
            var repository = Repository.Factory.GetCoreV3(_pluginOptions.Value.PackageSource);
            var resource = await repository.GetResourceAsync<FindPackageByIdResource>();

            var versions = await resource.GetAllVersionsAsync(
                name,
                cache,
                NuGet.Common.NullLogger.Instance,
                CancellationToken.None);

            return versions
                .Select(x => x.Version)
                .OrderByDescending(x => x)
                .FirstOrDefault();
        }
    }
}