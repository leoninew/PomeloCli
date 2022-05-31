using System.ComponentModel.DataAnnotations;
using System.Reflection;
using PomeloCli.Attributes;
using Xunit;

namespace PomeloCli.Tests.Attributes {
    public class CommandAttributeTest {
        [Fact]
        public void Validate_ShouldFail_ApplyInvalidName() {
            var commandAttr = typeof(InvalidCommand2).GetCustomAttribute<CommandAttribute>();

            Assert.NotNull(commandAttr);
            var exception = Assert.Throws<ValidationException>(() =>
                Validator.ValidateObject(commandAttr, new ValidationContext(commandAttr), true));
            Assert.Equal(exception.ValidationResult.MemberNames, new[] { "Name" });
        }

        [Fact]
        public void Validate_ShouldSucceed_ApplyParentCommand() {
            var commandAttr = typeof(ParentCommand).GetCustomAttribute<CommandAttribute>();

            Assert.NotNull(commandAttr);
            Validator.ValidateObject(commandAttr, new ValidationContext(commandAttr), false);
        }

        [Fact]
        public void Validate_ShouldSucceed_ApplyChildCommand() {
            var commandAttr = typeof(ChildCommand).GetCustomAttribute<CommandAttribute>();

            Assert.NotNull(commandAttr);
            Validator.ValidateObject(commandAttr, new ValidationContext(commandAttr), false);
        }
    }


    [Command("test")]
    class ParentCommand : Command {
    }

    [Command("test")]
    class ChildCommand : Command<ParentCommand> {
    }

    [Command("123")]
    class InvalidCommand2 {
    }
}