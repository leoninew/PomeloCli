using System;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using PomeloCli.DemoApp.Diagnosis;
using PomeloCli.Plugin;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.CommandLine;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace PomeloCli.DemoApp {
    class ApplicationHostedService : IHostedService {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly ICommandService _commandService;
        private readonly CommandLineConfigurationSource _configurationSource;
        private readonly IPluginResolver _pluginResolver;
        private readonly IDiagnosisService _diagnosisService;

        public ApplicationHostedService(
            ILogger<ApplicationHostedService> logger,
            IConfiguration configuration,
            CommandLineConfigurationSource configurationSource,
            ICommandService commandService,
            IPluginResolver pluginResolver,
            IDiagnosisService diagnosisService) {
            _logger = logger;
            _configuration = configuration;
            _configurationSource = configurationSource;
            _commandService = commandService;
            _pluginResolver = pluginResolver;
            _diagnosisService = diagnosisService;
        }

        public async Task StartAsync(CancellationToken cancellationToken) {
            var commandApp = _commandService.ConfigureApplication();
            ConfigureGlobalOptions(commandApp);
            await CheckUpgradeSafelyAsync();

            var exception = default(Exception);
            try {
                commandApp.Execute(_configurationSource.Args.ToArray());
            }
            catch (Exception ex) {
                _logger.LogError("command execute failed, args: {Args}", JsonSerializer.Serialize(_configurationSource.Args));
                exception = ex;
            }

            await DiagnosisReportSafelyAsync(exception);
            if(exception != null) {
                throw exception;
            }
        }

        private void ConfigureGlobalOptions(CommandLineApplication commandApp) {
            commandApp.Option("--no-diagnosis", "disable diagnosis report, equals to env PC_NO_DIAGNOSIS=true",
                CommandOptionType.NoValue);
            commandApp.Option("--no-upgrade", "disable upgrade check, equals to env PC_NO_UPGRADE=true",
                CommandOptionType.NoValue);
            commandApp.Option("--no-plugin", "disable plugin loading, equals to env PC_NO_PLUGIN=true",
                CommandOptionType.NoValue);
        }

        private async Task CheckUpgradeSafelyAsync() {
            var args = _configurationSource.Args.ToArray();
            // if declared in args or env value, skip upgrade check
            var skipUpgradeCheck = args.Contains("--no-upgrade", StringComparer.OrdinalIgnoreCase)
                || Boolean.TrueString.Equals(_configuration["NO_UPGRADE"], StringComparison.OrdinalIgnoreCase);
            if (skipUpgradeCheck) {
                return;
            }

            try {
                var start = DateTime.Now;
                var assemblyName = Assembly.GetEntryAssembly().GetName();
                var latestVersion = await _pluginResolver.GetLatestAsync(assemblyName.Name);
                if (latestVersion == default) {
                    return;
                }

                var currentVersion = VersionHelper.GetCurrentVersion();
                if (currentVersion < latestVersion) {
                    _logger.LogInformation("version {LatestVersion} available, contact {Name} maintainer to upgrade from {CurrentVersion}",
                        latestVersion, assemblyName.Name, currentVersion);
                }

                _logger.LogDebug("finish upgrade check, cost {TotalMilliseconds:f0} ms",
                    DateTime.Now.Subtract(start).TotalMilliseconds);
            }
            catch (Exception ex) {
                _logger.LogDebug("upgrade check failed, {Message}", ex.Message);
            }
        }

        private async Task DiagnosisReportSafelyAsync(Exception exception) {
            var args = _configurationSource.Args.ToArray();
            // if declared in args or env value, skip diagnosis report
            var skipDiagnosisReport = args.Contains("--no-diagnosis", StringComparer.OrdinalIgnoreCase)
                || Boolean.TrueString.Equals(_configuration["NO_DIAGNOSIS"], StringComparison.OrdinalIgnoreCase);
            if (skipDiagnosisReport) {
                return;
            }

            try {
                var start = DateTime.Now;
                var success = await _diagnosisService.ReportAsync(args, exception);
                if (success) {
                    _logger.LogDebug("finish diagnosis report, cost {TotalMilliseconds:f0} ms",
                        DateTime.Now.Subtract(start).TotalMilliseconds);
                }
            }
            catch (Exception ex) {
                _logger.LogDebug("diagnosis report failed, {Message}", ex.Message);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) {
            return Task.CompletedTask;
        }
    }
}