using GigyaManagement.CLI.Services.Context;

using Spectre.Console;
using Spectre.Console.Cli;

using System.ComponentModel.DataAnnotations;

namespace GigyaManagement.Spectre.CLI.Commands.ContextCommands;

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
            AnsiConsole.WriteException(ex, ExceptionFormats.ShortenEverything);
            return -1;
        }

        return 0;
    }
}
