using GigyaManagement.Spectre.CLI.Commands.Abstractions;
using GigyaManagement.Spectre.CLI.Commands.Context;

using Spectre.Console.Cli;

namespace GigyaManagement.Spectre.CLI.Commands.ContextCommands;

public class ContextCommandRegistrar : IRegisterCommands
{
    public IConfigurator RegisterCommand(IConfigurator configurator)
    {
        configurator.AddBranch("context", context =>
        {
            context.SetDescription("Commands scoped for execution context");

            context.AddCommand<ListContextsCommand>("list")
                .WithDescription("Prints out a list of all configured contexts.");
            context.AddCommand<CurrentContextCommand>("current")
                .WithDescription("Prints out the current selected context.");
            context.AddCommand<CreateContextCommand>("create")
                .WithDescription("Creates a new context. Note, newly created contexts are not automatically set as current.");
            context.AddCommand<SetContextCommand>("set")
                .WithDescription("Select an existing context as the current context.");
        });

        return configurator;
    }
}
