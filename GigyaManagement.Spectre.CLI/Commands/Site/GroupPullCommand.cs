using GigyaManagement.CLI.Handlers;

using Mediator;

using Spectre.Console;
using Spectre.Console.Cli;

namespace GigyaManagement.Spectre.CLI.Commands.Sites;

internal sealed class GroupPullCommand : AsyncCommand<GroupPullCommand.Settings>
{
    private readonly IMediator _mediator;

    public GroupPullCommand(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        var msg = new PullSiteGroupRequest
        {
            
            SiteName = settings.Name,
            ApiKey = settings.APIKey
        };

        try 
        { 
            var result = await _mediator.Send(msg);
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteException(ex, ExceptionFormats.ShortenEverything);
        }

        return 0;
    }

    internal class Settings : CommandSettings
    {
        [CommandArgument(0, "<name>")]
        public string Name { get; set; }

        [CommandArgument(1, "<apikey>")]
        public string APIKey { get; set; }
    }
}
