using GigyaManagement.CLI.Handlers;

using Mediator;

using System.CommandLine;

namespace GigyaConfigCLI.Factories;

public class ScaffoldCommandFactory : ICommandFactory
{
    private readonly IMediator _mediator;

    public ScaffoldCommandFactory(IMediator mediator)
    {
        _mediator = mediator;
    }

    public Command CreateCommand()
    {
        var command = new Command("scaffold", "attempts to scrape an existing site");
        command.AddAlias("scfld");

        command.Add(CreateScaffoldTemplateCommand());
        
        // TODO: make as a switch on the scaffold command
        //command.Add(CreateScaffoldForManageCommand());

        return command;
    }

    private Command CreateScaffoldTemplateCommand()
    {
        var apiKeyOption = new Option<string>(
            new[] { "--apikey", "-k" },
            "the target apikey for scaffolding");

        var templateSubCommand = new Command("template", "scaffold target site for templating")
        {
            apiKeyOption
        };

        templateSubCommand.SetHandler(async (string apikey) => 
        {
            var scaffoldResult = await _mediator.Send(new ScrapeSiteRequest
            {
                ApiKey = apikey
            });

            // TODO: printout scaffold project

        }, apiKeyOption);

        return templateSubCommand;
    }

    private Command CreateScaffoldForManageCommand()
    {
        var apiKeyOption = new Option<string>(
            new[] { "apikey", "k" },
            "the target apikey for scaffolding");

        var manageSubCommand = new Command("manage", "scaffold an existing site for management")
        {
            apiKeyOption
        };

        return manageSubCommand;
    }
}
