using GigyaManagement.Core.Handlers;

using Mediator;

using Spectre.Console;
using Spectre.Console.Cli;

namespace GigyaManagement.Spectre.CLI.Commands.Solution;

internal sealed class CreateSolutionCommand : AsyncCommand<CreateSolutionCommand.Settings>
{
    private readonly IMediator _mediator;

    public CreateSolutionCommand(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        var result = await _mediator.Send(new CreateSolutionRequest
        {
            SolutionName = settings.Name
        });

        if (result.Created)
        {
            AnsiConsole.MarkupLineInterpolated($"[green] Solution '{settings.Name}' created at[/]");
            AnsiConsole.Write(new TextPath(result.SolutionFolder!)
            {
                RootStyle = new Style(foreground: Color.Red),
                SeparatorStyle = new Style(foreground: Color.Green),
                StemStyle = new Style(foreground: Color.Blue),
                LeafStyle = new Style(foreground: Color.Yellow),
            });

            return 0;
        }
        else
        {
            AnsiConsole.MarkupLineInterpolated($"[red] Creation of solution '{settings.Name}' failed[/]");
            AnsiConsole.MarkupLineInterpolated($"Error: {result.Error}");

            return -1;
        }
    }

    internal sealed class Settings : CommandSettings
    {
        [CommandArgument(0, "<solution name>")]
        public string Name { get; set; }
    }
}
