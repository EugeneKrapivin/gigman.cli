using GigyaManagement.CLI.Services.Context;
using GigyaManagement.Core;

using Spectre.Console;
using Spectre.Console.Cli;

using System.Text.Json;

namespace GigyaManagement.Spectre.CLI.Commands.Context;

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
            var availableContexts = _contextService.GetAllContexts().DefinedContexts.Select(x => x.Name);

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
            AnsiConsole.WriteException(ex, ExceptionFormats.ShortenEverything);
            return -1;
        }

        return 0;
    }
}