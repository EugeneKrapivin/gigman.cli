using GigyaManagement.CLI.Handlers;
using GigyaManagement.Spectre.CLI.Commands.Abstractions;

using Mediator;

using Spectre.Console;
using Spectre.Console.Cli;

namespace GigyaManagement.Spectre.CLI.Commands;

internal sealed class TemplateCommandRegistrar : IRegisterCommands
{
    public IConfigurator RegisterCommand(IConfigurator configurator)
    {
        configurator.AddBranch("template", template =>
        {
            template.SetDescription("Commands handling with template management and creation");
            
            template.AddCommand<CreateTemplateCommand>("create")
                .WithDescription("Creates a template from an existing site");
        });

        return configurator;
    }
}

internal sealed class CreateTemplateCommand : AsyncCommand<CreateTemplateCommand.Settings>
{
    private readonly IMediator _mediator;

    public CreateTemplateCommand(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        var scaffoldResult = await _mediator.Send(new ScrapeTemlateRequest
        {
            ApiKey = settings.APIKey
        });

        AnsiConsole.MarkupLine($"Created at [link]{scaffoldResult.ProjectPath}[/]");

        return 0;
    }

    internal class Settings : CommandSettings
    {
        [CommandArgument(0, "<apikey>")]
        public string APIKey { get; set; }
    }


}

