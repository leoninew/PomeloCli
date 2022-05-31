using System;
using System.Linq;
using PomeloCli.Attributes;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using PomeloCli.DemoApp.Configuration;

namespace PomeloCli.DemoApp.Commands
{
    [Command("list", Description = "show configuration information")]
    class ConfigurationListCommand : Command<ConfigurationCommand>
    {
        private readonly IConfiguration _configuration;

        public ConfigurationListCommand(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [CommandOption("-o|--output", CommandOptionType.SingleValue,
            Description = "output format, optional, one of: env|json")]
        public String Output { get; set; }

        [CommandOption("-c|--custom", CommandOptionType.NoValue,
            Description = "only show custom setting")]
        public Boolean Custom { get; set; }

        public override Int32 Execute()
        {
            var configuration = Custom ? ApplicationProfile.GetConfiguration() : _configuration;
            if (Output == "json")
            {
                var json = JsonConvert.SerializeObject(configuration.AsJson(), Formatting.Indented);
                Console.WriteLine(json);
            }
            else if (Output == "env")
            {
                var items = configuration.AsEnumerablePairs();
                foreach (var item in items)
                {
                    Console.WriteLine($"{item.Key.Replace(":", "__")}={item.Value}");
                }
            }
            else
            {
                var items = configuration.AsEnumerablePairs();
                foreach (var item in items)
                {
                    Console.WriteLine($"{item.Key}={item.Value}");
                }
            }

            return 0;
        }
    }
}