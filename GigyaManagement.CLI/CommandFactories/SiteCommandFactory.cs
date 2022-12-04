using GigyaManagement.CLI.Handlers;
using GigyaManagement.CLI.Services.Context;

using Mediator;

using System.CommandLine;
using System.CommandLine.IO;
using System.Xml.Schema;

namespace GigyaConfigCLI.Factories;

public class SiteCommandFactory : ICommandFactory
{
    private readonly WorkspaceContext _partnerContext;
    private readonly IMediator _mediator;

    public SiteCommandFactory(WorkspaceContext partnerContext, IMediator mediator)
    {
        _partnerContext = partnerContext;
        _mediator = mediator;
    }
    public Command CreateCommand()
    {
        var command = new Command("site", "manage sites");

        var listCommand = CreateListCommand();
        var applyCommand = CreateApplyCommand();
        var scrapeCommand = CreatePullCommand();

        command.Add(listCommand);
        command.Add(applyCommand);
        command.Add(scrapeCommand);

        return command;
    }

    private Command CreatePullCommand()
    {
        var scrapeCommand = new Command("pull", "pull an existing targetApikey form remote to local");
        
        var apiKeyOption = new Option<string>(new[] { "--apikey", "-k" }, "target apikey") { IsRequired = true };
        var solutionNameOption = new Option<string>(new[] { "--name", "-n" }, "name under which the targetApikey would be stored in the workspace") { IsRequired = true };
        var envOption = new Option<string>(new[] { "--env", "-e" }, () => "dev", "which environment the targetApikey is deployed at");
        
        scrapeCommand.Add(apiKeyOption);
        scrapeCommand.Add(solutionNameOption);
        scrapeCommand.Add(envOption);

        scrapeCommand.SetHandler(async (string apikey, string solutionName, string env) =>
        {
            var scaffoldResult = await _mediator.Send(new ScrapeSiteRequest
            {
                ApiKey = apikey,
                IsTemplate = false,
                Environment = env,
                SolutionName = solutionName
            });
        }, apiKeyOption, solutionNameOption, envOption);
        return scrapeCommand;
    }

    private Command CreateApplyCommand()
    {
        var applyCommand = new Command("apply", "apply local targetApikey configurations to remote");
        
        var solutionNameOption = new Option<string?>(new[] { "--solution", "-s" }, () =>
        {
            var curDir = Directory.GetCurrentDirectory();
            var sol = Directory.GetFiles("site.solution.json");
            if (sol.Any())
            {
                return Path.GetDirectoryName(curDir);
            }
            return null;

        }, "solution application will take effect on") { IsRequired = true };
        var envNameOption = new Option<string>(new[] { "--env", "-e" }, "from which environment to take the configuration") { IsRequired = true };
        var targetApiKey = new Option<string>(
            new[] { "--target", "-t" }, 
            "changes will be applied from the selected env on to the target apikey") 
            { 
                IsRequired = false 
            };
        var selfArgument = new Option<bool>(new string[] { "--self" }, () => true, "apply changes on same env") { Arity = ArgumentArity.ZeroOrOne };

        applyCommand.Add(targetApiKey);
        applyCommand.Add(envNameOption);
        applyCommand.Add(solutionNameOption);
        applyCommand.Add(selfArgument);


        applyCommand.SetHandler(async (ctx) =>
        {
            var targetApikey = ctx.BindingContext.ParseResult.GetValueForOption(targetApiKey);
            var sol = ctx.BindingContext.ParseResult.GetValueForOption(solutionNameOption)!;
            var env = ctx.BindingContext.ParseResult.GetValueForOption(envNameOption)!;
            var self = ctx.BindingContext.ParseResult.GetValueForOption(selfArgument)!;

            if (sol == null)
            {
                ctx.ExitCode = -1;
                ctx.Console.Error.WriteLine("solution name wasn't passed or not found in local directory");
                return;
            }

            try 
            { 
                await _mediator.Send(new ApplySiteChangesRequest
                {
                    Solution = sol,
                    Environment = env,
                    SelfApply = self,
                    TargetApiKey = self ? null : targetApikey
                });

                ctx.Console.WriteLine("changes applied");
            } 
            catch (Exception ex)
            {
                ctx.ExitCode = -1;
                ctx.Console.Error.WriteLine(ex.Message);
            }
        });

        return applyCommand;
    }

    private Command CreateListCommand()
    {
        var listCommand = new Command("list", "list locally scrapped sites");
        listCommand.SetHandler((ctx) =>
        {
            var sitesWorkspace = Path.Combine(_partnerContext.Workspace, "_sites");
            if (!Directory.Exists(sitesWorkspace) || !Directory.GetDirectories(sitesWorkspace).Any())
            {
                ctx.Console.Error.WriteLine("There are no targetApikey in current workspace");
                ctx.ExitCode = -1;
                return;
            }
            var sites = Directory.GetDirectories(sitesWorkspace)
                .Select(x => Path.GetFileName(x))
                .Select(x => $"* {x}");

            ctx.Console.WriteLine(string.Join("\r\n", sites));
        });
        return listCommand;
    }
}
