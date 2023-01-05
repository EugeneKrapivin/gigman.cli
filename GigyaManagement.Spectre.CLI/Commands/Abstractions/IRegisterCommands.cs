using Spectre.Console.Cli;

namespace GigyaManagement.Spectre.CLI.Commands.Abstractions;

public interface IRegisterCommands
{
    IConfigurator RegisterCommand(IConfigurator configurator);
}
