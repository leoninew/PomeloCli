using System;
using System.Threading.Tasks;
using PomeloCli.DemoApp.Commands;
using PomeloCli.Plugin;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PomeloCli.DemoApp.Diagnosis;

namespace PomeloCli.DemoApp {
    public class Program {
        public static async Task Main(String[] args) {
            using var host = CreateHostBuilder(args).Build();
            await host.StartAsync();
            await host.StopAsync();
        }

        private static IHostBuilder CreateHostBuilder(String[] args) {
            var environment = Environment.GetEnvironmentVariable("DOTNET_ENV") 
                ?? Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")
                ?? "Production";
            return Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration((context, builder) => {
                    builder.Sources.Clear();
                    builder.SetBasePath(AppContext.BaseDirectory);
                    builder.AddJsonFile("appsettings.json", false);
                    builder.AddJsonFile($"appsettings.{environment}.json", true);
                    builder.AddEnvironmentVariables("PC_");
                })
                .ConfigureLogging((context, builder) => {
                    builder.AddSimpleConsole(options => {
                        options.SingleLine = true;
                        options.IncludeScopes = false;
                        options.TimestampFormat = "[HH:mm:ss.fff] ";
                    });
                    if (String.Equals(environment, "debug")) {
                        builder.SetMinimumLevel(LogLevel.Debug);
                    }
                    else {
                        builder.AddFilter("System", LogLevel.Warning);
                        builder.AddFilter("Microsoft", LogLevel.Warning);
                    }
                })
                .ConfigureServices((context, services) => {
                    var configuration = context.Configuration;
                    services.AddSingleton(provider => new CommandLineConfigurationSource {
                        Args = args
                    });

                    services.AddHttpClient();
                    services.AddOptions<DiagnosisOptions>()
                        .Bind(configuration.GetSection("Diagnosis"))
                        .ValidateDataAnnotations();
                    services.AddTransient<IDiagnosisService, DiagnosisService>();

                    services.AddPluginSupport(context.Configuration);
                    
                    services.AddTransient<ICommandService, CommandService>();
                    services.AddHostedService<ApplicationHostedService>();
                    services.AddTransient<ICommand, VersionCommand>();
                });
        }
    }
}