using GigyaManagement.Spectre.CLI.Commands.Abstractions;

using Spectre.Console.Cli;

namespace GigyaManagement.Spectre.CLI.Commands.Template;

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

