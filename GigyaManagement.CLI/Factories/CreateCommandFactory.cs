using System.CommandLine;

namespace GigyaConfigCLI.Factories;

public class CreateCommandFactory : ICommandFactory
{
    public Command CreateCommand()
    {
        var command = new Command("create", "creation of resources");

        var createSiteCommand = new Command("site", "create site from scaffold");
        
        command.AddCommand(createSiteCommand);

        return command;
    }
}