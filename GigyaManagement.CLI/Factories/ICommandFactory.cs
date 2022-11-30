using System.CommandLine;

namespace GigyaConfigCLI.Factories;

public interface ICommandFactory
{
    Command CreateCommand();
}
