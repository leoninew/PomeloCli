using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Microsoft.Extensions.CommandLineUtils;
using PomeloCli.Attributes;
using PomeloCli.Commands;

namespace PomeloCli {
    public abstract class Command : ICommand {
        private const String DefaultHelpOption = "-?|-h|--help";

        protected Command() {
            CommandContainer = new CommandContainer();
        }

        protected ICommandContainer CommandContainer { get; }

        public void Configure(CommandLineApplication cmdApp) {
            CommandContainer.SetApplication(cmdApp);
            ConfigureHelp(cmdApp);
            ConfigureCommand(cmdApp);
            ConfigureArguments(cmdApp);
            ConfigureOptions(cmdApp);
        }

        public Int32 Execute() {
            BindingParameters();
            return OnExecute();
        }

        protected void BindingParameters() {
            BindingArguments();
            BindingOptions();
            try {
                Validator.ValidateObject(this, new ValidationContext(this), true);
            }
            catch (ValidationException ex) {
                throw new ArgumentException($"command '{GetType().FullName}' validate failed", ex);
            }
        }

        private void ConfigureHelp(CommandLineApplication cmdApp) {
            cmdApp.HelpOption(DeclareHelp() ?? DefaultHelpOption);
        }

        private void ConfigureCommand(CommandLineApplication cmdApp) {
            var declaration = DeclareCommand();
            if (declaration == null) {
                throw new ArgumentException($"command '{GetType().FullName}' declaration missing");
            }

            cmdApp.Name = declaration.Name;
            cmdApp.Description = declaration.Description;
        }

        private void ConfigureArguments(CommandLineApplication cmdApp) {
            var arguments = DeclareArguments();
            if (arguments != null) {
                foreach (var argument in arguments) {
                    cmdApp.Argument(argument.Name, argument.Description, argument.MultipleValues);
                }
            }
        }

        private void ConfigureOptions(CommandLineApplication cmdApp) {
            var options = DeclareOptions();
            if (options != null) {
                foreach (var option in options) {
                    cmdApp.Option(option.Template, option.Description, option.OptionType);
                }
            }
        }

        protected virtual String DeclareHelp() {
            return DefaultHelpOption;
        }

        protected virtual CommandDeclaration DeclareCommand() {
            var attr = GetType().GetCustomAttribute<CommandAttribute>();
            if (attr == null) {
                return null;
            }

            Validator.ValidateObject(attr, new ValidationContext(attr), true);
            return new CommandDeclaration(attr.Name, attr.Description);
        }

        protected virtual IEnumerable<CommandArgument> DeclareArguments() {
            var type = GetType();
            var attrs = type.GetCustomAttributes<CommandArgumentAttribute>(false);
            foreach (var attr in attrs) {
                yield return attr.ConvertRaw();
            }

            var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var prop in props) {
                var attr = prop.GetCustomAttribute<CommandArgumentAttribute>();
                if (attr != null) {
                    ValidateArgument(prop, attr);
                    yield return attr.ConvertRaw();
                }
            }
        }

        protected virtual IEnumerable<CommandOption> DeclareOptions() {
            var type = GetType();
            var attrs = type.GetCustomAttributes<CommandOptionAttribute>(false);
            foreach (var attr in attrs) {
                yield return attr.ConvertRaw();
            }

            var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var prop in props) {
                var attr = prop.GetCustomAttribute<CommandOptionAttribute>();
                if (attr != null) {
                    ValidateOptions(prop, attr);
                    yield return attr.ConvertRaw();
                }
            }
        }

        private void ValidateArgument(PropertyInfo prop, CommandArgumentAttribute attr) {
            if (attr.MultipleValues) {
                // prop type must be string collection
                if (prop.PropertyType != typeof(IEnumerable<String>)
                    && prop.PropertyType != typeof(IList<String>)
                    && prop.PropertyType != typeof(String[])
                    && prop.PropertyType != typeof(List<String>)) {
                    throw new InvalidProgramException("property type must be string[] or List<string>");
                }
            }
            else if (prop.PropertyType != typeof(String)) {
                throw new InvalidProgramException("property type must be string");
            }
        }

        private void ValidateOptions(PropertyInfo prop, CommandOptionAttribute attr) {
            if (attr.OptionType == CommandOptionType.NoValue) {
                if (prop.PropertyType != typeof(Boolean)) {
                    throw new InvalidProgramException("property type must be boolean");
                }
            }
            else if (attr.OptionType == CommandOptionType.SingleValue) {
                if (prop.PropertyType != typeof(String)) {
                    throw new InvalidProgramException("property type must be string");
                }
            }
            else // if (attr.OptionType == CommandOptionType.MultipleValue)
            {
                // prop type must be string collection
                if (prop.PropertyType != typeof(IEnumerable<String>)
                    && prop.PropertyType != typeof(IList<String>)
                    && prop.PropertyType != typeof(String[])
                    && prop.PropertyType != typeof(List<String>)) {
                    throw new InvalidProgramException("property type must be string[] or List<string>");
                }
            }
        }

        protected virtual Int32 OnExecute() {
            return 0;
        }

        private void BindingArguments() {
            var type = GetType();
            var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var prop in props) {
                if (prop.CanWrite == false) {
                    continue;
                }

                var attr = prop.GetCustomAttribute<CommandArgumentAttribute>();
                if (attr == null) {
                    continue;
                }

                var argument = CommandContainer.GetArgument(attr.Name);
                if (argument == null) {
                    continue;
                }

                Object value = argument.MultipleValues switch {
                    false => argument.Value,
                    _ => argument.Values
                };
                prop.SetValue(this, value);
            }
        }

        private void BindingOptions() {
            var type = GetType();
            var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var prop in props) {
                if (prop.CanWrite == false) {
                    continue;
                }

                var attr = prop.GetCustomAttribute<CommandOptionAttribute>();
                if (attr == null) {
                    continue;
                }

                var option = CommandContainer.GetOption(attr.Template);
                if (option == null) {
                    continue;
                }

                Object value = option.OptionType switch {
                    CommandOptionType.NoValue => option.HasValue(),
                    CommandOptionType.SingleValue => option.Value(),
                    _ => option.Values
                };

                prop.SetValue(this, value);
            }
        }
    }
}