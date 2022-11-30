using GigyaManagement.CLI.Services.Context;

using Microsoft.Extensions.Options;

using System.CommandLine;
using System.CommandLine.IO;
using System.Text.Json;

namespace GigyaConfigCLI.Factories;

public class ContextCommandFactory : ICommandFactory
{
    private readonly IContextService _contextService;

    public ContextCommandFactory(IContextService contextService)
    {
        _contextService = contextService;
    }
    public Command CreateCommand()
    {
        var command = new Command("context", "manage the tool execution context");
        command.AddAlias("ctx");

        command.AddCommand(AddListSubCommand());
        command.AddCommand(AddCurrentSubCommand());
        command.AddCommand(AddCreateSubCommand());
        command.AddCommand(AddSetSubCommand());

        return command;
    }

    private Command AddListSubCommand()
    {
        var command = new Command("list", "lists all existing contexts for user");
        command.SetHandler(() =>
        {
            var contexts = _contextService.GetAllContexts();
            Console.WriteLine(JsonSerializer.Serialize(contexts, new JsonSerializerOptions { WriteIndented = true }));
        });
        return command;
    }

    private Command AddCurrentSubCommand()
    {
        var command = new Command("current", "returns currently used context");

        command.SetHandler((context) =>
        {
            var current = _contextService.GetCurrentContext();

            context.Console.WriteLine($"Current context:");
            if (current is null)
            {
                context.Console.WriteLine($"Context is not set");
            }
            else
            {
                context.Console.WriteLine(JsonSerializer.Serialize(current, new JsonSerializerOptions { WriteIndented = true }));
            }
        });

        return command;
    }

    private Command AddCreateSubCommand()
    {
        var nameOption = new Option<string>(new[] { "--name", "-n" }) { IsRequired = true };
        var userKeyOption = new Option<string>(new[] { "--userkey", "-k" }) { IsRequired = true };
        var secretOption = new Option<string>(new[] { "--secret", "-s" }) { IsRequired = true };
        var workspaceOption = new Option<string>(new[] { "--workspace", "-w" }) { IsRequired = true };

        var command = new Command("create", "creates a new context. newly created contexts are not automatically set as added")
        {
            nameOption, userKeyOption, secretOption, workspaceOption
        };

        command.SetHandler(ctx =>
        {
            var name = ctx.BindingContext.ParseResult.GetValueForOption(nameOption);
            var userkey = ctx.BindingContext.ParseResult.GetValueForOption(userKeyOption);
            var secret = ctx.BindingContext.ParseResult.GetValueForOption(secretOption);
            var workspace = ctx.BindingContext.ParseResult.GetValueForOption(workspaceOption);
            try
            {
                var added = _contextService.CreateNewContext(new WorkspaceContext
                {
                    Name = name,
                    UserKey = userkey,
                    Secret = secret,
                    Workspace = workspace
                });
                ctx.Console.WriteLine($"Added \"{name}\" context:");
                ctx.Console.WriteLine(JsonSerializer.Serialize(added, new JsonSerializerOptions { WriteIndented = true }));
            }
            catch (Exception ex) 
            { 
                ctx.Console.Error.WriteLine(ex.ToString());
                ctx.ExitCode = -1;
            }
        });

        return command;
    }

    private Command AddSetSubCommand()
    {
        var nameOption = new Option<string>(new[] { "--name", "-n" })
        {
            IsRequired = true
        };

        var command = new Command("set", "sets a partner context")
        {
            nameOption
        };

        command.SetHandler((ctx) =>
        {
            var name = ctx.BindingContext.ParseResult.GetValueForOption(nameOption);
            try
            {
                var set = _contextService.SetContext(name);
                ctx.Console.WriteLine($"Setting \"{name}\" context as current");
                ctx.Console.WriteLine(JsonSerializer.Serialize(set, new JsonSerializerOptions { WriteIndented = true }));
            }
            catch (Exception ex)
            {
                ctx.ExitCode = -100;
                ctx.Console.Error.WriteLine(ex.Message);
            }
        });

        return command;
    }
}
