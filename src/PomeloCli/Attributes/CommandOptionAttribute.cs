using System;
using Microsoft.Extensions.CommandLineUtils;

namespace PomeloCli.Attributes {
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    public sealed class CommandOptionAttribute : Attribute {
        public CommandOptionAttribute(String template, CommandOptionType optionType) {
            Template = template;
            OptionType = optionType;
        }

        public String Template { get; }

        public CommandOptionType OptionType { get; }

        public String Description { get; set; }

        public CommandOption ConvertRaw() {
            return new CommandOption(Template, OptionType) {
                Description = Description
            };
        }
    }
}