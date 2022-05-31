using Microsoft.Extensions.DependencyInjection;
using PomeloCli.DemoPlugin.Commands;

namespace PomeloCli.DemoPlugin {
    public static class ServiceCollectionExtensions {
        public static IServiceCollection ConfigureCommands(this IServiceCollection services) {
            services.AddTransient<ICommand, EchoCommand>();
            services.AddTransient<ICommand, DockerCommand>();
            services.AddTransient<ICommand, DockerPsCommand>();
            services.AddTransient<ICommand, DockerImagesCommand>();
            services.AddTransient<ICommand, DockerLogsCommand>();
            return services;
        }
    }
}