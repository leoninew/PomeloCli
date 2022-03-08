using System;
using Microsoft.Extensions.CommandLineUtils;

namespace PomeloCli.Attributes {
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    public sealed class CommandArgumentAttribute : Attribute {
        public CommandArgumentAttribute(String name, Boolean multipleValues) {
            Name = name;
            MultipleValues = multipleValues;
        }

        public String Name { get; }

        public Boolean MultipleValues { get; }

        public String Description { get; set; }

        public CommandArgument ConvertRaw() {
            return new CommandArgument {
                Name = Name,
                Description = Description,
                MultipleValues = MultipleValues
            };
        }
    }
}