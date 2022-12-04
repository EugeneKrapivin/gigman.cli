using GigyaManagement.CLI.Handlers;

using Mediator;

using System.CommandLine;

namespace GigyaConfigCLI.Factories;

public class TemplateCommandFactory : ICommandFactory
{
    private readonly IMediator _mediator;

    public TemplateCommandFactory(IMediator mediator)
    {
        _mediator = mediator;
    }

    public Command CreateCommand()
    {
        var command = new Command("template", "attempts to scrape an existing site")
        {
            CreateTemplateSubCommand()
        };
       
        return command;
    }

    private Command CreateTemplateSubCommand()
    {
        var apiKeyOption = new Option<string>(
            new[] { "--apikey", "-k" },
            "the target apikey for scaffolding");

        var templateSubCommand = new Command("scaffold", "scaffold target site for templating")
        {
            apiKeyOption
        };

        templateSubCommand.SetHandler(async (string apikey) => 
        {
            var scaffoldResult = await _mediator.Send(new ScrapeTemlateRequest
            {
                ApiKey = apikey
            });

            // TODO: printout scaffold project

        }, apiKeyOption);

        return templateSubCommand;
    }
}
