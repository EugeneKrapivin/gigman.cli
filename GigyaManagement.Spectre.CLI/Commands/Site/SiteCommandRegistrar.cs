using Spectre.Console.Cli;
using GigyaManagement.Spectre.CLI.Commands.Abstractions;

namespace GigyaManagement.Spectre.CLI.Commands.Sites;

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
