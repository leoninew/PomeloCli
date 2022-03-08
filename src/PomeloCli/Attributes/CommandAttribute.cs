using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PomeloCli.Attributes {
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class CommandAttribute : Attribute, IValidatableObject {
        public CommandAttribute(String name) {
            Name = name;
        }

        [Required]
        [RegularExpression("^[a-z][a-z0-9_\\-\\.]+$")]
        public String Name { get; }

        public String Description { get; set; }

        public Type Parent { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
            if (Parent != null) {
                var basicType = typeof(ICommand);
                if (Parent.IsInterface || Parent.IsAbstract || !basicType.IsAssignableFrom(Parent)) {
                    yield return new ValidationResult($"use parent type '{Parent.FullName} is not allowed", new[] { nameof(Parent) });
                }
            }
        }
    }
}