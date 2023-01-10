using Mediator;

using Spectre.Console;
using GigyaManagement.CLI.Handlers;
using Spectre.Console.Cli;
using GigyaManagement.Core.Services.LockService;

namespace GigyaManagement.Spectre.CLI.Commands.Sites;

internal sealed class PullSiteCommand : AsyncCommand<PullSiteCommand.Settings>
{
    private readonly IMediator _mediator;
    private readonly ILockService _lockService;

    public PullSiteCommand(IMediator mediator, ILockService lockService)
    {
        _mediator = mediator;
        _lockService = lockService;
    }


    public class Settings : CommandSettings
    {
        [CommandArgument(1, "<solution>")]
        public string SolutionName { get; set; }

        [CommandArgument(1, "<apikey>")]
        public string APIkey { get; set; }

        [CommandArgument(2, "<site name>")]
        public string SiteName { get; set; }

        [CommandArgument(3, "[environment]")]
        public string Environment { get; set; } = "prod";

        [CommandOption("--include-child-sites")]
        public bool IncludeChildSites { get; set; } = false;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        try
        {
            var pullResult = await GetSite(settings);

            AnsiConsole.MarkupLineInterpolated($"[green]Site '{settings.APIkey}' with publish profile '{settings.Environment}' was created at[/]");
            
            var path = new TextPath(pullResult.ProjectPath)
            {
                RootStyle = new Style(foreground: Color.Red),
                SeparatorStyle = new Style(foreground: Color.Green),
                StemStyle = new Style(foreground: Color.Blue),
                LeafStyle = new Style(foreground: Color.Yellow),
            };

            _lockService.LockRecursive(pullResult.ProjectPath);

            AnsiConsole.Write(path);

            PrintChildSites(pullResult);

            return 0;
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteException(ex, ExceptionFormats.ShortenEverything);
            return -1;
        }
    }

    private async Task<PullSiteResult> GetSite(Settings settings)
    {
        return settings.IncludeChildSites switch
        {
            false => await _mediator.Send(new PullSiteRequest
            {
                ApiKey = settings.APIkey,
                Environment = settings.Environment,
                SiteName = settings.SiteName,
                SolutionName = settings.SolutionName
            }),
            true => await _mediator.Send(new PullSiteGroupRequest
            {
                ApiKey = settings.APIkey,
                Environment = settings.Environment,
                SiteName = settings.SiteName,
                SolutionName = settings.SolutionName
            })
        };
    }

    private static void PrintChildSites(PullSiteResult pullResult)
    {
        if (pullResult.ChildSites?.Any() == true)
        {
            var childTable = new Table();

            // TODO: add path on the siteproject level as an internal parameter?
            childTable.AddColumns("name", "apikey");

            foreach (var site in pullResult.ChildSites)
            {
                var resource = site.SiteConfigResource.Resource;
                childTable.AddRow(new Text(resource.BaseDomain), new Text(site.Apikey));
            }

            AnsiConsole.Write(childTable);
        }
    }
}