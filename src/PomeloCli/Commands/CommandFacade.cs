using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.CommandLineUtils;
using PomeloCli.Attributes;

namespace PomeloCli.Commands {
    class CommandFacade {
        private readonly ICommand _command;

        public CommandFacade(ICommand command) {
            _command = command;
        }

        public Int32 Execute() {
            BindingArguments();
            BindingOptions();
            ValidateCommand();
            return _command.Execute();
        }

        public Task<Int32> ExecuteAsync() {
            BindingArguments();
            BindingOptions();
            ValidateCommand();

            var asyncCommand = _command as IAsyncCommand;
            if (asyncCommand == null) {
                throw new InvalidProgramException();
            }
            return asyncCommand.ExecuteAsync();
        }

        private void BindingArguments() {
            var command = _command as Command;
            if (command == null) {
                return;
            }

            var type = _command.GetType();
            var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var prop in props) {
                if (prop.CanWrite == false) {
                    continue;
                }

                var attr = prop.GetCustomAttribute<CommandArgumentAttribute>();
                if (attr == null) {
                    continue;
                }

                var argument = command.CommandContainer.GetArgument(attr.Name);
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
            var command = _command as Command;
            if (command == null) {
                return;
            }

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

                var option = command.CommandContainer.GetOption(attr.Template);
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

        private void ValidateCommand() {
            try {
                Validator.ValidateObject(this, new ValidationContext(this), true);
            }
            catch (ValidationException ex) {
                throw new ArgumentException($"command '{GetType().FullName}' validate failed", ex);
            }
        }
    }
}
