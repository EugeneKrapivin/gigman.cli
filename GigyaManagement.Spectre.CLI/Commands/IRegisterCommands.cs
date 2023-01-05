using Spectre.Console.Cli;

namespace GigyaManagement.Spectre.CLI.Commands;

public interface IRegisterCommands
{
    IConfigurator RegisterCommand(IConfigurator configurator);
}
