using System;
using System.ComponentModel.DataAnnotations;

namespace PomeloCli.Attributes {
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class CommandAttribute : Attribute {
        public CommandAttribute(String name) {
            Name = name;
        }

        [Required]
        [RegularExpression("^[a-z][a-z0-9_\\-\\.]+$")]
        public String Name { get; }

        public String Description { get; set; }
    }
}