using GigyaManagement.CLI.Services.Context;

using Mediator;

using Spectre.Console;
using GigyaManagement.CLI.Handlers;
using Spectre.Console.Cli;

namespace GigyaManagement.Spectre.CLI.Commands.Sites;

internal sealed class ApplyToSiteCommand : AsyncCommand<ApplyToSiteCommand.Settings>
{
    private readonly WorkspaceContext _partnerContext;
    private readonly IMediator _mediator;

    public ApplyToSiteCommand(WorkspaceContext partnerContext, IMediator mediator)
    {
        _partnerContext = partnerContext;
        _mediator = mediator;
    }


    public class Settings : CommandSettings
    {
        [CommandArgument(0, "<solution>")]
        public string Solution { get; set; }

        [CommandArgument(1, "<environment>")]
        public string Environment { get; set; }

        [CommandArgument(2, "<apikey>")]
        public string APIkey { get; set; }

        [CommandOption("-s|--self")]
        public bool Self { get; set; }
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        var targetApikey = settings.APIkey;
        var sol = settings.Solution!;
        var env = settings.Environment!;
        var self = settings.Self;

        if (sol == null)
        {
            AnsiConsole.WriteLine("[bold red]solution name wasn't passed or not found in local directory[/]");
            return -1;
        }

        try
        {
            await _mediator.Send(new ApplySiteChangesRequest
            {
                Solution = sol,
                Environment = env,
                SelfApply = self,
                TargetApiKey = self ? null : targetApikey
            });

            AnsiConsole.WriteLine("[green]changes applied[/]");
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteException(ex, ExceptionFormats.ShortenEverything);
            return -1;
        }

        return 0;
    }
}
