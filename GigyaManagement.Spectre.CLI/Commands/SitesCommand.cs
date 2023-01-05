using GigyaManagement.CLI.Services.Context;

using Mediator;

using Spectre.Console;
using GigyaManagement.CLI.Handlers;
using Spectre.Console.Cli;
using GigyaManagement.Spectre.CLI.Commands.Abstractions;

namespace GigyaManagement.Spectre.CLI.Commands;

internal sealed class SiteCommandRegistrar : IRegisterCommands
{
    public IConfigurator RegisterCommand(IConfigurator configurator)
    {
        configurator.AddBranch("site", site =>
        {
            site.SetDescription("Commands scoped for site manipulation");

            site.AddCommand<ListSitesCommand>("list")
                .WithDescription("Lists the sites under the current workspace (workspace is defined by the context)");
            site.AddCommand<PullSiteCommand>("pull")
                .WithDescription("Pulls a site from Gigya CIAM into a project on disk");
            site.AddCommand<ApplyToSiteCommand>("apply")
                .WithDescription("Apply current project to a site in CIAM. [bold red underline]Warning[/]: this operation will actively overwrite your current configurations in CIAM");
        });

        return configurator;
    }
}

internal sealed class ListSitesCommand : AsyncCommand
{
    private readonly WorkspaceContext _partnerContext;
    private readonly IMediator _mediator;

    public ListSitesCommand(WorkspaceContext partnerContext, IMediator mediator)
    {
        _partnerContext = partnerContext;
        _mediator = mediator;
    }

    public override Task<int> ExecuteAsync(CommandContext context)
    {
        var sitesWorkspace = Path.Combine(_partnerContext.Workspace, "_sites");
        if (!Directory.Exists(sitesWorkspace) || !Directory.GetDirectories(sitesWorkspace).Any())
        {
            Console.Error.WriteLine("There are no targetApikey in current workspace");
            return Task.FromResult(-1);
        }
        var sites = Directory.GetDirectories(sitesWorkspace)
            .Select(x => Path.GetFileName(x))
            .Select(x => $"* {x}");

        AnsiConsole.WriteLine(string.Join("\r\n", sites));

        return Task.FromResult(0);
    }
}

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

internal sealed class PullSiteCommand : AsyncCommand<PullSiteCommand.Settings>
{
    private readonly IMediator _mediator;

    public PullSiteCommand(IMediator mediator)
    {
        _mediator = mediator;
    }


    public class Settings : CommandSettings
    {
        [CommandArgument(0, "<apikey>")]
        public string APIkey { get; set; }

        [CommandArgument(1, "<site name>")]
        public string SiteName { get; set; }

        [CommandArgument(2, "<environment>")]
        public string Environment { get; set; }
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        try 
        { 
            var scaffoldResult = await _mediator.Send(new ScrapeSiteRequest
            {
                ApiKey = settings.APIkey,
                IsTemplate = false,
                Environment = settings.Environment,
                SiteName = settings.SiteName
            });

            AnsiConsole.WriteLine($"[green] created at [underline]{scaffoldResult.ProjectPath}[/][/]");

            return 0;
        }
        catch(Exception ex)
        {
            AnsiConsole.WriteException(ex, ExceptionFormats.ShortenEverything);
            return -1;
        }
    }
}