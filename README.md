[TOC]

## 快速开始

### 步骤

1. 创建一个 netcoreapp3.1 或 net5.0 类库项目，并引用依赖 *PomeloCli*

```bash
$ dotnet new console --framework netcoreapp3.1
$ dotnet add package PomeloCli --version 1.2.2
```

如果使用 IDE，引用依赖 *PomeloCli* 后项目文件(csproj)包含以下内容

```xml
  <ItemGroup>
    <PackageReference Include="PomeloCli" Version="1.2.2" />
  </ItemGroup>
```

2. 修改 *Program.cs*，替换成以下内容

```c#
using System;
using PomeloCli;
using PomeloCli.Attributes;

namespace test
{
    class Program
    {
        static void Main(string[] args)
        {
            var commands = new[]{
                new EchoCommand()
            };
            new CommandService(commands)
                .ConfigureApplication()
                .Execute(args.Length > 0 ? args: new[] { "echo", "Hello PomeloCli" });
        }
    }

    [Command("echo")]
    class EchoCommand : Command
    {
        [CommandArgument("input", false)]
        public String Input { get; set; }

        public override int Execute()
        {
            Console.WriteLine(Input);
            return 0;
        }
    }
}
```

你可以看到当前实现包含了一个命令 `EchoCommand`

- 特性 `[Command("echo")]` 提供了入口说明
- 属性 `Input` 及相应特性 `[CommandArgument("input", false)]` 表示可以接受参数
- 方法 `public override int Execute()` 展示了命令的内部逻辑，它会原样打印输入的字符串

3. 运行项目，查看使用帮助

```bash
$ dotnet run -- -h


Usage:  [options] [command]

Options:
  -?|-h|--help  Show help information

Commands:
  echo

Use " [command] --help" for more information about a command.
```

运行 `echo` 命令

```bash
$ dotnet run
hello PomeloCli

$ dotnet run --no-build -- echo "the first cli demo"
the first cli demo
```

4. 继续引用 *Microsoft.Extensions.DependencyInjection* 以使用依赖注入

```bash
$ dotnet add package Microsoft.Extensions.DependencyInjection --version 5.0.2
```

如果使用 IDE，引用依赖 *PomeloCli* 后项目文件(csproj)包含以下内容

```xml
  <ItemGroup>
    <PackageReference Include="Microsoft.Extensions.DependencyInjection" Version="5.0.2" />
    <PackageReference Include="PomeloCli" Version="1.2.2" />
  </ItemGroup>
```

5. 接着修改 *Program.cs*，添加 docker 相关的命令，并使用依赖注入管理它们

```c#
using System;
using PomeloCli;
using PomeloCli.Attributes;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;

namespace test
{
    class Program
    {
        static void Main(string[] args)
        {
            var services = new ServiceCollection()
                .AddTransient<ICommand, DockerCommand>()
                .AddTransient<ICommand, DockerPsCommand>()
                .AddTransient<ICommandService, CommandService>()
                .BuildServiceProvider();

            var commandService = services.GetRequiredService<ICommandService>();
            commandService.ConfigureApplication()
                .Execute(args);
        }
    }

    [Command("docker", Description = "A self-sufficient runtime for containers")]
    class DockerCommand : Command
    {
    }

    [Command("ps")]
    class DockerPsCommand : Command<DockerCommand>
    {
        [CommandOption("-a|--all", CommandOptionType.NoValue, Description = "Show all containers (default shows just running)")]
        public Boolean All { get; set; }

        public override int Execute()
        {
            // Util.Cmd("docker", "ps"); //can execute under LINQPad
            Console.WriteLine("This is docker list command");
            return 0;
        }
    }
}
```

你可以注意到命令 `DockerPsCommand` 的定义方式与之前的 `EchoCommand` 及当前的 `DockerCommand` 不一样

- `DockerPsCommand` 继承自泛型 `Command<T>`，是命令 `DockerCommand` 的子命令；
- 命令特性 `[Command]` 及参数特性 `[CommandOption]` 都有可选的属性 `Description` 以描述用法
- 父命令一般是空命令

6. 运行项目，查看命令 `docker ps` 的使用帮助

```bash
$ dotnet run -- docker ps -h


Usage:  docker ps [options]

Options:
  -?|-h|--help  Show help information
  -a|--all      Show all containers (default shows just running)
```

运行命令 `docker ps -a`

```bash
$ dotnet run -- docker ps -a
This is docker list command
```

如果在 LINQPad 下使用，可以将 `Console.WriteLine()` 语句替换为 `Util.Cmd("docker", "ps")` 以检查效果。

### 小结

至此你已经了解到以下内容

- 如何编写基本的命令(Command)
- 如何为命令添加参数(CommandArgument)和选项(CommandOption)
- 如何编写子命令
- 如何为命令及参数编写说明
- 如何编写基本的控制台程序管理自己的命令
- 如何查看命令的使用帮助
- 如何使用依赖注入管理自己的命令

## 参考

- [Creating a console app with Dependency Injection in .NET Core](https://siderite.dev/blog/creating-console-app-with-dependency-injection-in-/)