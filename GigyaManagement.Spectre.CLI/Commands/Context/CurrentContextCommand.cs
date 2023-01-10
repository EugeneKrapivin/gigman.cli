using GigyaManagement.CLI.Services.Context;

using Spectre.Console;
using Spectre.Console.Cli;

namespace GigyaManagement.Spectre.CLI.Commands.ContextCommands;

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
