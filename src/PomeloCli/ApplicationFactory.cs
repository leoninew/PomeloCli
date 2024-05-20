using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;

namespace PomeloCli;

public class ApplicationFactory
{
    private readonly IServiceProvider _services;

    public ApplicationFactory(IServiceProvider services)
    {
        _services = services;
    }

    public static CommandLineApplication ConstructFrom(IServiceProvider services)
    {
        var applicationFactory = new ApplicationFactory(services);
        return applicationFactory.ConstructRootApp();
    }

    public CommandLineApplication ConstructRootApp()
    {
        var application = new CommandLineApplication();
        application.HelpOption();
        application.Conventions
            .UseDefaultConventions()
            .UseConstructorInjection(_services);
        ConfigureRootApp(application);
        return application;
    }

    public void ConfigureRootApp(CommandLineApplication rootApp)
    {
        var rootCommands = GetRootCommands();
        foreach (var rootCommand in rootCommands)
        {
            var subApp = ConstructChildApp(rootApp, rootCommand.GetType());
            Debug.Assert(subApp != null);
            ConfigureChildApp(subApp, rootCommand);
        }
    }

    private IEnumerable<ICommand> GetRootCommands()
    {
        var commands = _services.GetServices<ICommand>();
        var genericCommandType = typeof(Command<>);
        return commands.Where(x =>
        {
            var commandType = x.GetType();
            return commandType.IsSubclassOfRawGeneric(genericCommandType) == false;
        });
    }

    /// <summary>
    /// reflection version for `CommandLineApplication.Command<TModel>()`
    /// </summary>
    /// <param name="rootApp"></param>
    /// <param name="modelType"></param>
    /// <returns></returns>
    private CommandLineApplication? ConstructChildApp(CommandLineApplication rootApp, Type modelType)
    {
        var commandAttr = modelType.GetCustomAttribute<CommandAttribute>();
        Debug.Assert(commandAttr != null);

        var commandName = commandAttr.Name;
        Debug.Assert(commandName != null);


        // we would find and invoke method2 which is generic but without TModel
        // method-1: public CommandLineApplication Command(string name, Action<CommandLineApplication> configuration)
        // method-2: public CommandLineApplication<TModel> Command<TModel>(string name, Action<CommandLineApplication<TModel>> configuration) where TModel : class


        // TODO: use more elegant way
        var methodInfo = typeof(CommandLineApplication).GetMethods()
            .SingleOrDefault(x => x.Name == "Command" && x.IsGenericMethod);
        Debug.Assert(methodInfo != null);

        var result = methodInfo.MakeGenericMethod(modelType).Invoke(rootApp, new Object?[] { commandName, null });
        return result as CommandLineApplication;
    }

    private void ConfigureChildApp(CommandLineApplication childApp, ICommand command)
    {
        var descendantCommands = GetDescendantCommands(command);
        foreach (var descendantCommand in descendantCommands)
        {
            var descendantApp = ConstructChildApp(childApp, descendantCommand.GetType());
            Debug.Assert(descendantApp != null);

            ConfigureChildApp(descendantApp, descendantCommand);
        }
    }

    private IEnumerable<Command> GetDescendantCommands(ICommand command)
    {
        var commands = _services.GetServices<ICommand>();
        var commandType = typeof(Command<>).MakeGenericType(command.GetType());
        return commands.OfType<Command>().Where(x =>
        {
            var currentCommandType = x.GetType();
            return currentCommandType.IsSubclassOf(commandType);
        });
    }
}