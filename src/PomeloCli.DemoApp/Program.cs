using System;
using System.Threading.Tasks;
using PomeloCli.DemoApp.Commands;
using PomeloCli.DemoApp.Diagnosis;
using PomeloCli.Plugin;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace PomeloCli.DemoApp {
    public class Program {
        public static async Task Main(String[] args) {
            using var host = CreateHostBuilder(args).Build();
            await host.StartAsync();
            await host.StopAsync();
        }

        private static IHostBuilder CreateHostBuilder(String[] args) {
            var environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production";
            return Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration((context, builder) => {
                    builder.Sources.Clear();
                    builder.SetBasePath(AppContext.BaseDirectory);
                    builder.AddJsonFile("appsettings.json", false);
                    builder.AddJsonFile($"appsettings.{environment}.json", true);
                    builder.AddEnvironmentVariables("PC_");
                })
                .ConfigureLogging((context, builder) => {
                    builder.AddConfiguration(context.Configuration.GetSection("Logging"));
                    builder.AddSimpleConsole(options => {
                        options.SingleLine = true;
                        options.IncludeScopes = false;
                        options.TimestampFormat = "[HH:mm:ss.fff] ";
                    });
                })
                .ConfigureServices((context, services) => {
                    var configuration = context.Configuration;
                    services.AddSingleton(provider => new CommandLineConfigurationSource { Args = args });

                    // add plugin feature
                    services.AddPluginSupport(context.Configuration);

                    // add diagnostics feature
                    services.AddHttpClient();
                    services.AddOptions<DiagnosisOptions>()
                        .Bind(configuration.GetSection("Diagnosis"))
                        .ValidateDataAnnotations();
                    services.AddTransient<IDiagnosisService, DiagnosisService>();

                    // add command hosting feature
                    services.AddTransient<ICommandService, CommandService>();
                    services.AddHostedService<ApplicationHostedService>();

                    // add internal command
                    services.AddTransient<ICommand, VersionCommand>();
                    services.AddTransient<ICommand, ConfigurationCommand>();
                    services.AddTransient<ICommand, ConfigurationListCommand>();
                    services.AddTransient<ICommand, ConfigurationSetCommand>();
                });
        }
    }
}