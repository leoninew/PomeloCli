using System;
using System.Collections.Generic;
using System.Linq;
using PomeloCli.Attributes;
using PomeloCli.DemoApp.Configuration;

namespace PomeloCli.DemoApp.Commands
{
    [Command("set", Description = "show configuration information")]
    class ConfigurationSetCommand : Command<ConfigurationCommand>
    {
        [CommandArgument("settings", true)]
        public List<String> Settings { get; set; }

        public override int Execute()
        {
            var settings = Settings
                .Select(x => x.Split('=', 2))
                .Where(x => x.Length == 2)
                .Select(x => new KeyValuePair<String, String>(x[0].Replace("__", ":"), x[1]));
            ApplicationProfile.SaveConfiguration(settings);
            return base.Execute();
        }
    }
}