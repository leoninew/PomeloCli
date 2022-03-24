[TOC]

## 快速开始

我们将通过以下步骤展示如何从头开始编写自己的 cli 应用和命令

### 1. 新建项目

创建一个 netcoreapp3.1 或 net5.0 类库项目，并引用依赖 *PomeloCli*

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

### 2. 添加内容

修改 *Program.cs*，替换成以下内容

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

> 完整的项目可以参考 [docs/1-get-start](docs/1-get-start)

### 3. 首次运行项目

我们查看一下自带的使用帮助

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

### 4. 使用依赖注入

继续引用 *Microsoft.Extensions.DependencyInjection* 

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

### 5. 使用子命令

接着修改 *Program.cs*，添加 docker 相关的命令，并使用依赖注入管理它们

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
            // Util.Cmd("docker", All ? "ps -a" : "ps"); //can execute under LINQPad
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

> 完整的项目可以参考 [docs/1-get-start](docs/2-dependency-inject)

### 6. 重新运行项目

查看命令 `docker ps` 的使用帮助

```bash
$ dotnet run -- docker ps -h


Usage:  docker ps [options]

Options:
  -?|-h|--help  Show help information
  -a|--all      Show all containers (default shows just running)
```

运行命令 `docker ps -a`

```bash
$ dotnet run --no-build -- docker ps -a
This is docker list command
```

如果在 LINQPad 下使用，可以将 `Console.WriteLine()` 语句替换为调用 `Util.Cmd()` 以查看效果。

![image-20220324154446718](docs/README.assets/image-20220324154446718.png)

### 7. 打包应用

为了可以将应用交付给用户，我们需要修改项目文件以支持打包为 dotnet 工具

```diff
   <PropertyGroup>
     <OutputType>Exe</OutputType>
     <TargetFramework>netcoreapp3.1</TargetFramework>
+    <PackAsTool>true</PackAsTool>
+    <ToolCommandName>mycli</ToolCommandName>
+    <PackageId>MyCli</PackageId>
   </PropertyGroup>
```

>  dotnet tool 的入门文档可以参考 [Tutorial: Create a .NET tool using the .NET CLI](https://docs.microsoft.com/en-us/dotnet/core/tools/global-tools-how-to-create)

然后使用 `dotnet pack` 命令即可以将控制台打包成 dotnet 工具

```bash
$ dotnet pack -o nupkgs
Microsoft (R) Build Engine version 17.1.0+ae57d105c for .NET
Copyright (C) Microsoft Corporation. All rights reserved.

  Determining projects to restore...
  Restored D:\Documents\mywork\PomeloCli\docs\3-pack-as-tool\MyCli.csproj (in 379 ms).
  MyCli -> D:\Documents\mywork\PomeloCli\docs\3-pack-as-tool\bin\Debug\netcoreapp3.1\MyCli.dll
  MyCli -> D:\Documents\mywork\PomeloCli\docs\3-pack-as-tool\bin\Debug\netcoreapp3.1\MyCli.dll
  Successfully created package 'D:\Documents\mywork\PomeloCli\docs\3-pack-as-tool\nupkgs\MyCli.1.0.0.nupkg'.
```

> 如果需要将 nupkg 推送到 nuget 服务器上，需要确保包标识(PackageId) 是唯一的。

接着就可以使用 `dotnet tool` 命令安装本地已经打包的 `nupkgs\MyCli.1.0.0.nupkg`

```bash
$ dotnet tool install mycli -g --add-source nupkgs --configfile disable_nuget.config
You can invoke the tool using the following command: mycli
Tool 'mycli' (version '1.0.0') was successfully installed.
```

> 文件 [docs/3-pack-as-tool/disable_nuget.config](docs/3-pack-as-tool/disable_nuget.config) 的作用是避免包标识冲突，可以阅读 [How to fix NU1212 for dotnet tool install](https://stackoverflow.com/questions/52527004/how-to-fix-nu1212-for-dotnet-tool-install) 以获取进一步的信息。

最后就可以使用命令 `mycli` 作为我们的 cli 工具入口了

```bash
$ mycli docker ps -a
This is docker list command
```


### 小结

至此你已经了解到以下内容

- 如何编写基本的命令(Command)
- 如何为命令添加参数(CommandArgument)和选项(CommandOption)
- 如何编写子命令
- 如何为命令及参数编写说明
- 如何编写基本的控制台程序管理自己的命令
- 如何查看命令的使用帮助
- 如何使用依赖注入管理自己的命令

有了以上能力，我们可以借助 *dotnet tool* 的能力将自己的应用打包成 cli 应用，而如何插件化以应对业务命令增长的需求，将在后面进一步介绍。

## 必要知识



## 参考文档

- [Creating a console app with Dependency Injection in .NET Core](https://siderite.dev/blog/creating-console-app-with-dependency-injection-in-/)