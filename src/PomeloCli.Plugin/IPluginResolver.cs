using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace PomeloCli.Plugin {
    public interface IPluginResolver {
        void Loading(Plugin plugin, IServiceCollection services, IConfiguration configuration);
        Task<Version> GetLatestAsync(String name);
    }
}