using GigyaManagement.CLI.Services.Context;
using GigyaManagement.Core;

using Spectre.Console;
using Spectre.Console.Cli;

using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace GigyaManagement.Spectre.CLI.Commands;

public class ContextCommandRegistrar : IRegisterCommands
{
    public IConfigurator RegisterCommand(IConfigurator configurator)
    {
        configurator.AddBranch("context", context =>
        {
            context.SetDescription("Commands scoped for execution context");

            context.AddCommand<ListContextsCommand>("list")
                .WithDescription("Prints out a list of all configured contexts.");
            context.AddCommand<CurrentContextCommand>("current")
                .WithDescription("Prints out the current selected context.");
            context.AddCommand<CreateContextCommand>("create")
                .WithDescription("Creates a new context. Note, newly created contexts are not automatically set as current.");
            context.AddCommand<SetContextCommand>("set")
                .WithDescription("Select an existing context as the current context.");
        });

        return configurator;
    }
}

public sealed class ListContextsCommand : Command
{
    private readonly IContextService _contextService;

    public ListContextsCommand(IContextService contextService)
    {
        _contextService = contextService;
    }

    public override int Execute(CommandContext context)
    {
        var contexts = _contextService.GetAllContexts();
        
        AnsiConsole.Write(contexts.AsJsonPanel("Contexts"));

        return 0;
    }
}

public sealed class CurrentContextCommand : Command
{
    private readonly IContextService _contextService;

    public CurrentContextCommand(IContextService contextService)
    {
        _contextService = contextService;
    }

    public override int Execute(CommandContext context)
    {
        var current = _contextService.GetCurrentContext();

        if (current is null)
        {
            AnsiConsole.MarkupLine($"[red]Context is not set[/]");
        }
        else
        {
            AnsiConsole.Write(current.AsJsonPanel("Current context"));
        }

        return 0;
    }
}

public sealed class CreateContextCommand : Command<CreateContextCommand.Settings>
{
    private readonly IContextService _contextService;

    public CreateContextCommand(IContextService contextService)
    {
        _contextService = contextService;
    }

    public class Settings : CommandSettings
    {
        [CommandArgument(0, "<context name>")]
        [Required]
        public required string Name { get; init; }

        [Required]
        [CommandArgument(1, "<userkey>")]
        public required string UserKey { get; init; }

        [Required]
        [CommandArgument(2, "<secret>")] 
        public required string Secret { get; init; }

        [CommandArgument(3, "[workspace]")] 
        public string? Workspace { get; init; }
    }
    
    public override int Execute(CommandContext context, Settings settings)
    {
        var name = settings.Name!;
        var userkey = settings.UserKey!;
        var secret = settings.Secret!;
        var workspace = settings.Workspace;
        try
        {
            var workspaceContext = new WorkspaceContext
            {
                Name = name,
                UserKey = userkey,
                Secret = secret
            };

            if (!string.IsNullOrEmpty(workspace))
            {
                workspaceContext.Workspace = workspace;
            }

            var added = _contextService.CreateNewContext(workspaceContext);
            AnsiConsole.Write(added.AsJsonPanel($"""Added "{name}" context"""));
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteException(ex);
            return -1;
        }

        return 0;
    }
}

public sealed class SetContextCommand : Command<SetContextCommand.Settings>
{
    private readonly IContextService _contextService;

    public SetContextCommand(IContextService contextService)
    {
        _contextService = contextService;
    }

    public class Settings : CommandSettings
    {
        [CommandArgument(0, "[context name]")]
        public string Name { get; init; }
    }

    public override int Execute(CommandContext context, Settings settings)
    {
        var name = settings.Name;
        if (string.IsNullOrEmpty(name))
        {
            var availableContexts = _contextService.GetAllContexts().DefinedContexts.Select( x => x.Name);
            name = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                .Title("Select a context")
                .AddChoices(availableContexts));
        }
        try
        {
            var set = _contextService.SetContext(name);
            AnsiConsole.Write(set.AsJsonPanel($"""Setting "{name}" context as current"""));
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteException(ex);
            return -1;
        }

        return 0;
    }
}