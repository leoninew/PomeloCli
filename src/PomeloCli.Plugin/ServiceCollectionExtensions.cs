using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PomeloCli.Plugin.Commands;
using PomeloCli.Plugin.Runtime;

namespace PomeloCli.Plugin {
    public static class ServiceCollectionExtensions {
        public static IServiceCollection AddPluginSupport(this IServiceCollection services, IConfiguration configuration) {
            services.Configure<PluginOptions>(configuration.GetSection("Plugin").Bind);
            services.AddTransient<IPluginProvider, PluginProvider>();
            services.AddTransient<IPluginResolver, PluginResolver>();

            services.LoadPlugins(configuration);

            services.AddTransient<ICommand, PluginCommand>();
            services.AddTransient<ICommand, PluginInstallCommand>();
            services.AddTransient<ICommand, PluginUninstallCommand>();
            services.AddTransient<ICommand, PluginListCommand>();
            services.AddTransient<ICommand, PluginMigrateCommand>();

            return services;
        }

        private static void LoadPlugins(this IServiceCollection services, IConfiguration configuration) {
            using var scope = services.Clone().BuildServiceProvider().CreateScope();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Object>>();
            var pluginProvider = scope.ServiceProvider.GetRequiredService<IPluginProvider>();
            var pluginResolver = scope.ServiceProvider.GetRequiredService<IPluginResolver>();
            var pluginOptions = scope.ServiceProvider.GetRequiredService<IOptions<PluginOptions>>();

            if (pluginOptions.Value.Disable || Boolean.TrueString.Equals(configuration["NO_PLUGIN"], StringComparison.OrdinalIgnoreCase)) {
                logger.LogDebug("plugin loading skipped");
            }
            else {
                logger.LogDebug("plugin loading start");
                foreach (var plugin in pluginProvider.FindAll()) {
                    logger.LogDebug("read package '{Name}:{Version}'", plugin.Name, plugin.Version);
                    var before = services.Where(x => x.ServiceType == typeof(ICommand)).ToArray();
                    pluginResolver.Loading(plugin, services, configuration);

                    var after = services.Where(x => x.ServiceType == typeof(ICommand)).ToArray();
                    var append = after.Except(before);
                    foreach (var item in append) {
                        logger.LogDebug("add command '{FullName}'", item.ImplementationType?.FullName);
                    }
                }

                logger.LogDebug("plugin loading complete");
            }
        }

        private static IServiceCollection Clone(this IServiceCollection services) {
            var clone = new ServiceCollection();
            foreach (var descriptor in services) {
                ((ICollection<ServiceDescriptor>)clone).Add(descriptor);
            }

            return clone;
        }
    }
}