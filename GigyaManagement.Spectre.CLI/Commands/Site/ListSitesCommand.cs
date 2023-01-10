using GigyaManagement.CLI.Services.Context;

using Mediator;

using Spectre.Console;
using Spectre.Console.Cli;

namespace GigyaManagement.Spectre.CLI.Commands.Sites;

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
