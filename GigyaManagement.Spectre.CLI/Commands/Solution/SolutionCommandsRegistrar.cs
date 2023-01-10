using GigyaManagement.Spectre.CLI.Commands.Abstractions;

using Spectre.Console.Cli;

namespace GigyaManagement.Spectre.CLI.Commands.Solution;

internal sealed class SolutionCommandsRegistrar : IRegisterCommands
{
    public IConfigurator RegisterCommand(IConfigurator configurator)
    {
        configurator.AddBranch("solution", solution =>
        {
            solution.AddCommand<CreateSolutionCommand>("create")
                .WithDescription("Creates a new CIAM solution in the current workspace");

            solution.AddCommand<ListSolutionsCommand>("list")
                .WithDescription("Lists all solutions in the current workspaces");
        });

        return configurator;
    }
}
