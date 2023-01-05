using GigyaManagement.CLI;
using GigyaManagement.CLI.Services.Context;
using GigyaManagement.CLI.Services.Template.ProjectModels.Resources;

using Spectre.Cli;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Spectre.Console;
using GigyaManagement.Core;

namespace GigyaManagement.Spectre.CLI.Commands;

public class ContextCommandRegistrar : IRegisterCommands
{
    public IConfigurator RegisterCommand(IConfigurator configurator)
    {
        configurator.AddBranch("workspaceContext", context =>
        {
            context.SetDescription("Commands scoped for execution workspaceContext");

            context.AddCommand<ListContextsCommand>("list")
                .WithDescription("Prints out a list of all configured contexts.");
            context.AddCommand<CurrentContextCommand>("current")
                .WithDescription("Prints out the current selected workspaceContext.");
            context.AddCommand<CreateContextCommand>("create")
                .WithDescription("Creates a new workspaceContext. Note, newly created contexts are not automatically set as current.");
            context.AddCommand<SetContextCommand>("set")
                .WithDescription("Select an existing workspaceContext as the current workspaceContext.");
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
        var content = JsonSerializer.Serialize(contexts, GlobalUsings.JsonSerializerOptions);
        
        AnsiConsole.WriteLine(content);

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

        AnsiConsole.WriteLine($"Current workspaceContext:");
        if (current is null)
        {
            AnsiConsole.WriteLine($"Context is not set");
        }
        else
        {
            var content = JsonSerializer.Serialize(current, GlobalUsings.JsonSerializerOptions);
            
            AnsiConsole.WriteLine(content);
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
        [CommandArgument(0, "<workspaceContext name>")]
        public string Name { get; set; }
        
        [CommandArgument(1, "<userkey>")]
        public string UserKey { get; set; }

        [CommandArgument(2, "<secret>")] 
        public string Secret { get; set; }

        [CommandArgument(3, "<workspace>")] 
        public string Workspace { get; set; }
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
                Secret = secret,
            };

            if (!string.IsNullOrEmpty(workspace))
            {
                workspaceContext.Workspace = workspace;
            }

            var added = _contextService.CreateNewContext(workspaceContext);
            AnsiConsole.WriteLine($"Added \"{name}\" workspaceContext:");
            AnsiConsole.WriteLine(JsonSerializer.Serialize(added, GlobalUsings.JsonSerializerOptions));
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
        [CommandArgument(0, "<workspaceContext name>")]
        public string Name { get; set; }
    }

    public override int Execute(CommandContext context, Settings settings)
    {
        var name = settings.Name!;
        try
        {
            var set = _contextService.SetContext(name);
            AnsiConsole.WriteLine($"Setting \"{name}\" context as current");
            AnsiConsole.WriteLine(JsonSerializer.Serialize(set, GlobalUsings.JsonSerializerOptions));
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteException(ex);
            return -1;
        }

        return 0;
    }
}