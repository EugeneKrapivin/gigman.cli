using GigyaManagement.CLI.Services.Context;

using Mediator;

using Spectre.Console;
using Spectre.Console.Cli;

using System.IO;

namespace GigyaManagement.Spectre.CLI.Commands.Solution;

internal sealed class ListSolutionsCommand : AsyncCommand
{
    private readonly WorkspaceContext _partnerContext;
    private readonly IMediator _mediator;

    public ListSolutionsCommand(WorkspaceContext partnerContext, IMediator mediator)
    {
        _partnerContext = partnerContext;
        _mediator = mediator;
    }

    public override Task<int> ExecuteAsync(CommandContext context)
    {
        var contextWorkspace = _partnerContext.Workspace;

        if (!Directory.Exists(contextWorkspace) || !Directory.GetDirectories(contextWorkspace).Any())
        {
            return Task.FromResult(-1);
        }
        var solutions = Directory.GetDirectories(contextWorkspace);

        AnsiConsole.MarkupLineInterpolated($"Solutions in workspace context '[link]{contextWorkspace}[/]'");

        var table = new Table();
        table.AddColumns("Solution name", "Path");

        foreach(var solution in solutions) {
            table.AddRow(Path.GetFileName(solution), solution);
        }

        AnsiConsole.Write(table);

        return Task.FromResult(0);
    }
}
