using GigyaManagement.CLI.Services.Context;

using Spectre.Console;
using Spectre.Console.Cli;

namespace GigyaManagement.Spectre.CLI.Commands.ContextCommands;

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
